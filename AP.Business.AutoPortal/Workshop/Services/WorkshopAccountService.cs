using AP.Business.AutoPortal.Workshop.Contracts;
using System;
using AP.ViewModel.Workshop;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AP.Core.Model.User;
using AP.Repository.Workshop.Contracts;
using AutoMapper;
using AP.EntityModel.AutoDomain;
using EntityFramework.DbContextScope.Interfaces;
using AP.Business.Domain.Common;
using AP.Infrastructure.Messaging;
using AP.Business.Workshop.Contracts;
using AP.EntityModel.Booking;
using AP.Business.AutoPortal.Events;
using System.Linq;
using System.Collections.Generic;
using AP.Infrastructure.Utils;
using AP.Shared.Security.Contracts;
using AP.Shared.Geo.Contracts;

namespace AP.Business.AutoPortal.Workshop.Services
{
    public class WorkshopAccountService : IWorkshopAccountService
    {
        private readonly IAddressService _addressService;
        private readonly IAccountService accountService;
        private readonly IWorkshopFilterRepository filterRepo;
        private readonly IWorkshopAccountRepository _workshopAccountRepository;
        private readonly IGeoLocator geoLocator;
        private readonly IDbContextScopeFactory _dbContextScope;
        private readonly IEventBus eventBus;

        public WorkshopAccountService(UserManager<ApplicationUser> userManager,
            IWorkshopAccountRepository accountRepo,
            IDbContextScopeFactory scopeFactory,
            IAccountService accountService,
            IAddressService addressService,
            IWorkshopFilterRepository filterRepo,
            IGeoLocator geoLocator,
            IEventBus eventBus)
        {
            _addressService = addressService;
            this.filterRepo = filterRepo;
            this.accountService = accountService;
            _workshopAccountRepository = accountRepo;
            _dbContextScope = scopeFactory;
            this.eventBus = eventBus;
            this.geoLocator = geoLocator;
        }

        public async Task<Guid> Add(WorkshopAccountViewModel workshopViewModel, ApplicationUser user)
        {
            var workshopData = Mapper.Map<WorkshopData>(workshopViewModel);

            workshopData.UserID = user.Id;
            var workshopId = Guid.Empty;
            AnchorType anchor;
            using (var dbContext = _dbContextScope.Create())
            {
                //check if the same instance is already exist: verify by NAME
                var address = geoLocator.DecodeAddress(workshopViewModel.GooglePlaceId);
                var cityId = _addressService.GetCityByName(workshopData.Address.City.Ru);
                if (cityId == null)
                {
                    throw new ArgumentException(string.Format("the city {0} hasn't been added to database", workshopData.Address.City.Ru));
                }

                workshopData.Address.CityID = cityId;
                if (workshopData.IsPublished)
                {
                    workshopData.IsPublished = false;
                }

                anchor = new AnchorType
                {
                    Quantity = workshopData.AnchorNumber,
                    Price = workshopData.PayHour,
                    Name = "Hoist",
                    Description = "For cars",
                };

                workshopData.Anchors.Add(anchor);
                workshopData.Slug = HandleGenerator.Generate(12);
                workshopData.AccessCode = HandleGenerator.Generate(6);

                workshopId = _workshopAccountRepository.Add(workshopData);

                dbContext.SaveChanges();
            }

            PublishWorkshopEvent<WorkshopCreated>(workshopData);
            if (anchor != null)
            {
                PublishAnchorCreated(workshopId, anchor);
            }

            await accountService.AddClaim(user, ApplicationClaims.Accomplished);

            return workshopId;
        }

        public async Task CreateAnchor(Guid workshopId, AnchorTypeViewModel anchorView)
        {
            var anchorTypeData = Mapper.Map<AnchorType>(anchorView);

            using (var factory = _dbContextScope.Create())
            {
                //anchorTypeData.WorkshopID = workshopId;
                await _workshopAccountRepository.CreateAnchor(anchorTypeData);
            }
            var workshop = filterRepo.FindById(workshopId);
            if (workshop.WasEverPublished)
            {
                PublishAnchorCreated(workshopId, anchorTypeData);
            }
        }

        public void Update(WorkshopAccountViewModel workshopViewModel)
        {
            var wsUpdated = Mapper.Map<WorkshopData>(workshopViewModel);
            AnchorType hoists = null;
            using (var dbContext = _dbContextScope.Create())
            {
                var wsCurrent = filterRepo.FindById(wsUpdated.ID);
                if (wsCurrent == null)
                {
                    throw new ArgumentException(string.Format("Provided ID {0} doesn't exist in DB", wsUpdated.ID));
                }

                _workshopAccountRepository.LoadAnchors(wsCurrent);
                //TODO: temp decision with Address until Google PathID will be integrated
                _workshopAccountRepository.LoadAddress(wsCurrent);

                if (wsCurrent.Address != null && wsCurrent.Address.City.Ru != wsUpdated.Address.City.Ru)
                {
                    //verify if new city exists in DB
                    var cityId = _addressService.GetCityByName(wsUpdated.Address.City.Ru);
                    if (cityId == null)
                    {
                        throw new ArgumentException(string.Format("the city {0} hasn't been added to database", wsUpdated.Address.City.Ru));
                    }

                    wsUpdated.Address.CityID = cityId;
                }

                var categories = wsUpdated.WorkshopCategories;
                if (categories != null)
                {
                    foreach (var c in categories)
                    {
                        c.WorkshopID = wsUpdated.ID;
                    }
                }

                var autobrands = wsUpdated.WorkshopAutobrands;
                if (autobrands != null)
                {
                    foreach (var w in autobrands)
                    {
                        w.WorkshopID = wsUpdated.ID;
                    }
                }

                if (wsCurrent.AnchorNumber != wsUpdated.AnchorNumber ||
                    wsCurrent.PayHour != wsUpdated.PayHour)
                {
                    //TODO: means that we have only Hoist anchors
                    hoists = wsCurrent.Anchors.FirstOrDefault();
                    if (hoists != null)
                    {
                        hoists.Quantity = wsUpdated.AnchorNumber;
                        hoists.Price = wsUpdated.PayHour;
                    }
                }

                if (wsUpdated.RegisterDate == null ||
                    wsUpdated.RegisterDate == DateTime.MinValue)
                {
                    wsUpdated.RegisterDate = wsCurrent.RegisterDate;
                }

                wsUpdated.WasEverPublished = wsCurrent.WasEverPublished;

                _workshopAccountRepository.Update(wsUpdated, wsCurrent);
                dbContext.SaveChanges();
            }

            PublishWorkshopEvent<WorkshopUpdated>(wsUpdated);
            if (wsUpdated.WasEverPublished && hoists != null)
            {
                PublishAnchorUpdated(wsUpdated.ID, hoists);
            }
        }

        public void Publish(Guid workshopId)
        {
            UpdatePublished(workshopId, true);
        }

        public void Unpublish(Guid workshopId)
        {
            UpdatePublished(workshopId, false);
        }

        private void UpdatePublished(Guid workshopId, bool isPublished)
        {
            using (var context = _dbContextScope.Create())
            {
                var workshop = filterRepo.FindById(workshopId);
                if (workshop == null)
                {
                    throw new KeyNotFoundException(string.Format("workshop Id {0} doesn't exist", workshopId));
                }
                workshop.IsPublished = isPublished;
                if (isPublished && !workshop.WasEverPublished)
                {
                    // This flags prevents any further seat type deletions.
                    workshop.WasEverPublished = true;
                    context.SaveChanges();

                    // We always publish events *after* saving to store.
                    // Publish all anchors that were created before.
                    _workshopAccountRepository.LoadAnchors(workshop);
                    foreach (var seat in workshop.Anchors)
                    {
                        PublishAnchorCreated(workshop.ID, seat);
                    }
                }
                else
                {
                    context.SaveChanges();
                }

                if (isPublished)
                {
                    eventBus.Publish(new WorkshopPublished { SourceId = workshop.ID });
                }
                else
                {
                    eventBus.Publish(new WorkshopUnpublished { SourceId = workshop.ID });
                }
            }
        }

        private void PublishWorkshopEvent<T>(WorkshopData workshop)
            where T : WorkshopEvent, new()
        {
            eventBus.Publish(new T()
            {
                SourceId = workshop.ID,
                Owner = new Owner
                {
                    //use this values from Identity
                    Phone = "101",//workshop.OwnerName,
                    Email = "test@gmail.com"//workshop.OwnerEmail,
                },
                Name = workshop.Name,
                Description = workshop.Description,
                Location = workshop.Location.ToString(),
                Slug = workshop.Slug,
                RegisterDate = workshop.RegisterDate
            });
        }

        //TODO: 2 events AnchorUpdated and AnchorCreated are really identical;
        // the logic could be joined.
        private void PublishAnchorCreated(Guid workshopId, AnchorType anchor)
        {
            eventBus.Publish(new AnchorCreated
            {
                WorkshopId = workshopId,
                SourceId = anchor.ID,
                Name = anchor.Name,
                Description = anchor.Description,
                Price = anchor.Price,
                Quantity = anchor.Quantity,
            });
        }

        private void PublishAnchorUpdated(Guid workshopId, AnchorType anchor)
        {
            eventBus.Publish(new AnchorUpdated
            {
                WorkshopID = workshopId,
                SourceId = anchor.ID,
                Name = anchor.Name,
                Description = anchor.Description,
                Price = anchor.Price,
                Quantity = anchor.Quantity,
            });
        }
    }
}
