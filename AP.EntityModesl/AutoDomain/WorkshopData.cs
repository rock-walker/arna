﻿using AP.Core.Model.User;
using AP.EntityModel.Booking;
using AP.EntityModel.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AP.EntityModel.Common.DomainModels;

namespace AP.EntityModel.AutoDomain
{
    public class WorkshopData
    {
        public WorkshopData()
        {
            Anchors = new List<AnchorType>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [StringLength(70, MinimumLength = 3)]
        public string Name { get; set; }

        [Range(1, 200)]
        public int AnchorNumber { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName="money")]
        public decimal PayHour { get; set; }
        public float AvgRate { get; set; }

        [RegularExpression("\\d{9}")]
        public int Unp { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        [StringLength(12)]
        public string Slug { get; set; }
        [Required]
        [StringLength(6)]
        public string AccessCode { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime RegisterDate { get; set; }

        public bool IsPublished { get; set; }
        public bool WasEverPublished { get; set; }

        public Guid? ContactID { get; set; }
        public Guid? AddressID { get; set; }
        public Guid LocationID { get; set; }
        public Guid? LogoID { get; set; }
        public Guid? UserID { get; set; }


        public ContactData Contact { get; set; }
        public AddressData Address { get; set; }
        public GeoMarker Location { get; set; }
        public AvatarImage Logo { get; set; }
        //public ApplicationUser User { get; set; }
        public ICollection<WorkshopCategoryData> WorkshopCategories { get; set; }
        public ICollection<WorkshopAutoBrand> WorkshopAutobrands { get; set; }
        public ICollection<WorkshopDayTimetableData> WorkshopWeekTimetable { get; set; }

        [ForeignKey("WorkshopID")]
        public ICollection<AnchorType> Anchors { get; set; }
    }
}
