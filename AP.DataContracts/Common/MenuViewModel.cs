using System.Collections.Generic;

namespace AP.ViewModel.Common
{
    public class MenuViewModel
    {
        public int Id { get; set; }
        public int Parent { get; set; }
        public IEnumerable<MenuViewModel> Sub { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
    }
}
