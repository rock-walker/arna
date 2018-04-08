using AP.ViewModel.Booking;
using AP.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AP.ViewModel.Workshop
{
    public class WorkshopAccountViewModel
    {
        public Guid ID { get; set; }

        [Required]
        [StringLength(70)]
        public string Name { get; set; }

        [Range(1, 200)]
        public int AnchorNumber { get; set; }

        public decimal PayHour { get; set; }

        [RegularExpression("\\d{9}")]
        public int Unp { get; set; }
        
        [StringLength(512)]
        public string Description { get; set; }
        
        [StringLength(256)]
        public string Slug { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime RegisterDate { get; set; }

        [StringLength(12)]
        public string GooglePlaceId { get; set; }

        public bool IsPublished { get; set; }

        public IEnumerable<DayTimetableViewModel> WorkshopWeekTimetable { get; set; }
        public IEnumerable<WorkshopCategoryViewModel> WorkshopCategories { get; set; }
        public IEnumerable<AutobrandViewModel> WorkshopAutobrands { get; set; }
        public ContactViewModel Contact { get; set; }
        public AddressViewModel Address { get; set; }
        public LocationViewModel Location { get; set; }
    }
}
