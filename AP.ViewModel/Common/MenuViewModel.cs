using System.Collections.Generic;

namespace AP.ViewModel.Common
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public int Parent { get; set; }
        public IEnumerable<CategoryViewModel> Sub { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
    }
}
