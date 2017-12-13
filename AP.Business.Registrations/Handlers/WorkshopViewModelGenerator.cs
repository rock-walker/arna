namespace AP.Business.Registrations.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Infrastructure.EventSourcing;
    using Infrastructure.Messaging;
    using Infrastructure.Messaging.Handling;
    using Registration.Commands;
    using Registration.Events;
    using AP.Repository.Context;
    using AP.Business.AutoPortal.Events;
    using AP.EntityModel.ReadModel;
    using AP.Business.Workshop.Contracts;
    using AP.Business.Model.Registration;
    using Microsoft.Extensions.Logging;
    using AP.Core.Database;
    using EntityFramework.DbContextScope.Interfaces;

    /// <summary>
    /// Generates a read model that is queried by <see cref="WorkshopDao"/>.
    /// </summary>
    public class WorkshopViewModelGenerator : AmbientContext<WorkshopRegistrationDbContext>,
        IEventHandler<WorkshopCreated>,
        IEventHandler<WorkshopUpdated>,
        IEventHandler<WorkshopPublished>,
        IEventHandler<WorkshopUnpublished>,
        IEventHandler<AnchorCreated>,
        IEventHandler<AnchorUpdated>,
        IEventHandler<AvailableAnchorsChanged>,
        IEventHandler<AnchorsReserved>,
        IEventHandler<AnchorsReservationCancelled>
    {
        private readonly IDbContextScopeFactory contextFactory;
        private readonly ICommandBus bus;
        private readonly ILogger<WorkshopViewModelGenerator> logger;

        public WorkshopViewModelGenerator(IDbContextScopeFactory contextFactory, ICommandBus bus,
            IAmbientDbContextLocator locator, ILogger<WorkshopViewModelGenerator> logger) : base(locator)
        {
            this.contextFactory = contextFactory;
            this.bus = bus;
            this.logger = logger;
        }

        public void Handle(WorkshopCreated @event)
        {
            using (var repository = this.contextFactory.Create())
            {
                var dto = DbContext.Find<WorkshopView>(@event.SourceId);
                if (dto != null)
                {
                    logger.LogWarning(
                        "Ignoring ConferenceCreated event for conference with ID {0} as it was already created.",
                        @event.SourceId);
                }
                else
                {
                    DbContext.Set<WorkshopView>().Add(
                        new WorkshopView(
                            @event.SourceId,
                            @event.Slug,
                            @event.Name,
                            @event.Description,
                            @event.Location,
                            @event.Tagline,
                            @event.TwitterSearch,
                            @event.RegisterDate,
                            Enumerable.Empty<AnchorType>()));

                    repository.SaveChanges();
                }
            }
        }

        public void Handle(WorkshopUpdated @event)
        {
            using (var repository = this.contextFactory.Create())
            {
                var confDto = DbContext.Find<WorkshopView>(@event.SourceId);
                if (confDto != null)
                {
                    confDto.Code = @event.Slug;
                    confDto.Description = @event.Description;
                    confDto.Location = @event.Location;
                    confDto.Name = @event.Name;
                    confDto.StartDate = @event.RegisterDate;
                    confDto.Tagline = @event.Tagline;
                    confDto.TwitterSearch = @event.TwitterSearch;

                    repository.SaveChanges();
                }
                else
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Failed to locate Conference read model for updated conference with id {0}.", @event.SourceId));
                }
            }
        }

        public void Handle(WorkshopPublished @event)
        {
            using (var repository = this.contextFactory.Create())
            {
                var dto = DbContext.Find<WorkshopView>(@event.SourceId);
                if (dto != null)
                {
                    dto.IsPublished = true;

                    DbContext.Save(dto);
                }
                else
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Failed to locate Conference read model for published conference with id {0}.", @event.SourceId));
                }

                repository.SaveChanges();
            }
        }

        public void Handle(WorkshopUnpublished @event)
        {
            using (var repository = this.contextFactory.Create())
            {
                var dto = DbContext.Find<WorkshopView>(@event.SourceId);
                if (dto != null)
                {
                    dto.IsPublished = false;

                    DbContext.Save(dto);

                    repository.SaveChanges();
                }
                else
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Failed to locate Conference read model for unpublished conference with id {0}.", @event.SourceId));
                }
            }
        }

        public void Handle(AnchorCreated @event)
        {
            using (var repository = this.contextFactory.Create())
            {
                var dto = DbContext.Find<AnchorType>(@event.SourceId);
                if (dto != null)
                {
                    logger.LogWarning(
                        "Ignoring AnchorCreated event for anchor type with ID {0} as it was already created.",
                        @event.SourceId);
                }
                else
                {
                    dto = new AnchorType(
                        @event.SourceId,
                        @event.WorkshopId,
                        @event.Name,
                        @event.Description,
                        @event.Price,
                        @event.Quantity);

                    bus.Send(
                        new AddAnchors
                            {
                                WorkshopID = @event.WorkshopId,
                                AnchorType = @event.SourceId,
                                Quantity = @event.Quantity
                            });

                    DbContext.Save(dto);
                    repository.SaveChanges();
                }
            }
        }

        public void Handle(AnchorUpdated @event)
        {
            using (var repository = this.contextFactory.Create())
            {
                var dto = DbContext.Find<AnchorType>(@event.SourceId);
                if (dto != null)
                {
                    dto.Description = @event.Description;
                    dto.Name = @event.Name;
                    dto.Price = @event.Price;

                    // Calculate diff to drive the anchor availability.
                    // Is it appropriate to have this here?
                    var diff = @event.Quantity - dto.Quantity;

                    dto.Quantity = @event.Quantity;

                    DbContext.Save(dto);

                    if (diff > 0)
                    {
                        this.bus.Send(
                            new AddAnchors
                                {
                                    WorkshopID = @event.WorkshopID, 
                                    AnchorType = @event.SourceId, 
                                    Quantity = diff,
                                });
                    }
                    else
                    {
                        bus.Send(
                            new AddAnchors
                                {
                                    WorkshopID = @event.WorkshopID,
                                    AnchorType = @event.SourceId,
                                    Quantity = Math.Abs(diff),
                                });
                    }

                    repository.SaveChanges();
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format("Failed to locate Anchor Type read model being updated with id {0}.", @event.SourceId));
                }
            }
        }

        public void Handle(AvailableAnchorsChanged @event)
        {
            this.UpdateAvailableQuantity(@event, @event.Anchors);
        }

        public void Handle(AnchorsReserved @event)
        {
            this.UpdateAvailableQuantity(@event, @event.AvailableAnchorsChanged);
        }

        public void Handle(AnchorsReservationCancelled @event)
        {
            this.UpdateAvailableQuantity(@event, @event.AvailableAnchorsChanged);
        }

        private void UpdateAvailableQuantity(IVersionedEvent @event, IEnumerable<AnchorQuantity> anchors)
        {
            using (var repository = this.contextFactory.Create())
            {
                var anchorDtos = DbContext.Set<AnchorType>().Where(x => x.WorkshopID == @event.SourceId).ToList();
                if (anchorDtos.Count > 0)
                {
                    // This check assumes events might be received more than once, but not out of order
                    var maxAnchorsAvailabilityVersion = anchorDtos.Max(x => x.AnchorsAvailabilityVersion);
                    if (maxAnchorsAvailabilityVersion >= @event.Version)
                    {
                        logger.LogWarning(
                            "Ignoring availability update message with version {1} for anchor types with conference id {0}, last known version {2}.",
                            @event.SourceId,
                            @event.Version,
                            maxAnchorsAvailabilityVersion);
                        return;
                    }

                    foreach (var anchor in anchors)
                    {
                        var anchorDto = anchorDtos.FirstOrDefault(x => x.ID == anchor.AnchorType);
                        if (anchorDto != null)
                        {
                            anchorDto.AvailableQuantity += anchor.Quantity;
                            anchorDto.AnchorsAvailabilityVersion = @event.Version;
                        }
                        else
                        {
                            // TODO should reject the entire update?
                            logger.LogWarning(
                                "Failed to locate Anchor Type read model being updated with id {0}.", anchor.AnchorType);
                        }
                    }

                    repository.SaveChanges();
                }
                else
                {
                    logger.LogWarning(
                        "Failed to locate Anchor Types read model for updated anchor availability, with conference id {0}.",
                        @event.SourceId);
                }
            }
        }
    }
}
