using AP.Core.Model.User;
using System.ComponentModel.DataAnnotations;

namespace AP.ViewModel.Account
{
    public class RegisterMobileViewModel : RegisterViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [EnumDataType(typeof(Roles))]
        public Roles Role { get; set; }
    }
}
