using AP.EntityModel.AutoDomain;
using AP.Server.Application;
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
        [SetUp]
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
                WorkshopWeekTimetable = null
            };

            var result = Mapper.Map<WorkshopData, WorkshopViewModel>(workshopData);
            Assert.AreEqual("cto alexa", workshopData.Name);
        }

        [Test]
        public void WorkshopAccountMapper()
        {
            var address = new AddressViewModel
            {
                
            };

            var workshopAccountVm = new WorkshopAccountViewModel
            {
                Name = "sss",
                Address = address 
            };

            var result = Mapper.Map<WorkshopData>(workshopAccountVm);
            Assert.AreEqual(workshopAccountVm.Name, result.Name);
        }
    }
}
