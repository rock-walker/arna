using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AP.ViewModel.Account
{
    public class RegisterMobileViewModel : RegisterViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone")]
        public string Phone { get; set; }
    }
}
