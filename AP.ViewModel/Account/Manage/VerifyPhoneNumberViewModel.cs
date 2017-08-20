using System;
using System.ComponentModel.DataAnnotations;

namespace AP.ViewModel.Account.Manage
{
    public class VerifyPhoneNumberViewModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string Phone { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}
