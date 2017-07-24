using System.Collections.Generic;

namespace AP.Business.Model.Common
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public int Parent { get; set; }
        public IEnumerable<CategoryModel> Sub { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
    }
}
