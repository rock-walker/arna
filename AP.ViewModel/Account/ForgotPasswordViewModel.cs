using System.ComponentModel.DataAnnotations;

namespace AP.ViewModel.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
