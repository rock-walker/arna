using System.Collections.Generic;
using System.Threading.Tasks;
using AP.DataContract;
using AP.Shared.Category;
using Microsoft.AspNetCore.Mvc;

namespace AP.Application.Controllers
{
	public class CategoryController : Controller
	{
	    private readonly ICategoryService _menu;

		[("hierarchical")]
		public async Task<IEnumerable<MenuItem>> GetHierarchical()
		{
		    return await _menu.GetHierarchical();
		}

		public async Task<IEnumerable<MenuItem>> GetTopLevel()
		{
		    return await _menu.GetTopLevel();
		}

		public string Get(int id)
		{
			return "value";
		}

		public void Post([FromBody]string value)
		{
		}

		public void Put(int id, [FromBody]string value)
		{
		}

		public void Delete(int id)
		{
		}
	}
}