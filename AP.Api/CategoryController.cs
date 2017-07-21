using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AP.DataContract;
using AP.Shared.Categories;

namespace AP.Application.Controllers
{
	public class CategoryController : ApiController
	{
	    private readonly ICategoriesService _menu;
		[ActionName("hierarchical")]
		public async Task<IEnumerable<MenuItem>> GetHierarchical()
		{
		    return await _menu.GetHierarchical();
		}

		// GET api/category
		public async Task<IEnumerable<MenuItem>> GetTopLevel()
		{
		    return await _menu.GetTopLevel();
		}

		// GET api/values/5
		public string Get(int id)
		{
			return "value";
		}

		// POST api/values
		public void Post([FromBody]string value)
		{
		}

		// PUT api/values/5
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		public void Delete(int id)
		{
		}
	}
}