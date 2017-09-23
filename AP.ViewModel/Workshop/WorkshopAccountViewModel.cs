using AP.Business.Model.Common;
using AP.ViewModel.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AP.ViewModel.Workshop
{
    public class WorkshopAccountViewModel
    {
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

        public IEnumerable<DayScheduleModel> WeekSchedule { get; set; }
        public IEnumerable<CategoryModel> Categories { get; set; }
        public ContactViewModel Contact { get; set; }
        public AddressViewModel Address { get; set; }
        public LocationViewModel Location { get; set; }
    }
}
