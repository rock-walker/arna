using AP.Core.Model.User;
using System;
using System.ComponentModel.DataAnnotations;

namespace AP.ViewModel.Account.Manage
{
    public class VerifyPhoneNumberViewModel : JwtResponse
    {
        [Required]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string Phone { get; set; }
    }
}
