using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.EntityModel.Common
{
	public class Category : DomainModels.BaseCategory
	{
		public string Title { get; set; }
		public string Link { get; set; }

        [NotMapped]
        public IEnumerable<Category> SubCategories { get; set; }
    }
}