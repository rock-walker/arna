using AP.Business.Model.Common;
using AP.EntityModel.AutoDomain;
using AP.Server.Application;
using AP.ViewModel.Booking;
using AP.ViewModel.Common;
using AP.ViewModel.Workshop;
using AutoMapper;
using NUnit.Framework;
using System;

namespace AP.UnitTest.AutoMapperTests
{
    [TestFixture]
    public class WorkshopMappings
    {
        [OneTimeSetUp]
        public void Startup()
        {
            //NOTE: Initialize() method should be used only once
            Mapper.Initialize(cfg => cfg.AddProfiles(new[] { "AP.Server" }));

            //Mapper.Initialize(cfg => cfg.CreateMap<LocationViewModel, GeoMarker>());
            //.ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.Mobile))
            //.ForMember(dest => dest.ID, option => option.Ignore()));

            //Mapper.Configuration.AssertConfigurationIsValid();
        }

        [Test]
        public void WorkshopMapper()
        {
            var workshopData = new WorkshopData
            {
                Name = "cto alexa",
                Unp = 123456789,
                AvgRate = 0,
                PayHour = 23.5000M,
                LocationID = Guid.Empty,
                Location = null,
                WorkshopAutobrands = null,
                WorkshopCategories = null,
                WorkshopWeekTimetable = null,
                RegisterDate = DateTime.Now,
                Slug = "test"
            };

            var result = Mapper.Map<WorkshopViewModel>(workshopData);
            Assert.AreEqual("cto alexa", workshopData.Name);
        }

        [Test]
        public void WorkshopAccountMapper()
        {
            var address = new AddressViewModel
            {
                Apartment = 12,
                Building = "22",
                City = new CityViewModel
                {
                    Ru = "Минск",
                    Country = new CountryViewModel
                    {
                        Shortname = "BY"
                    }
                }
            };

            var timetable = new[]
            {
                new DayTimetableModel
                {
                    Start = TimeSpan.FromHours(9),
                    DinnerStart = TimeSpan.FromHours(13),
                    Day = DayOfWeek.Monday
                }
            };
            var contact = new ContactViewModel
            {
                Mobile = "sdfsdfs;sdffsdf"
            };

            var location = new LocationViewModel
            {
                Lat = 34.4444,
                Lng = 45.5555
            };

            var workshopAccountVm = new WorkshopAccountViewModel
            {
                ID = Guid.Empty,
                Name = "sss",
                Address = address,
                WorkshopWeekTimetable = timetable,
                Contact = contact,
                Location = location
            };

            var result = Mapper.Map<WorkshopData>(workshopAccountVm);
            Assert.AreEqual(workshopAccountVm.Name, result.Name);
        }
    }
}
