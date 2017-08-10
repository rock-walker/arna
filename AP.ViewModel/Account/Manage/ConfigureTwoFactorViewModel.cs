using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AP.ViewModel.Account.Manage
{
    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; set; }
    }
}
