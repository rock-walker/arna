using System.Collections.Generic;
using System.Threading.Tasks;
using AP.ViewModel.Common;
using AP.Shared.Category;
using Microsoft.AspNetCore.Mvc;

namespace AP.Application.Controllers
{
    [Route("api/[controller]")]
	public class CategoryController : Controller
	{
	    private readonly ICategoryService _menu;

        public CategoryController(ICategoryService menu)
        {
            _menu = menu;
        }

        [Route("hierarchical")]
		public async Task<IEnumerable<MenuViewModel>> GetHierarchical()
		{
		    return await _menu.GetHierarchical();
		}

        [Route("firstlevel")]
		public async Task<IEnumerable<MenuViewModel>> GetTopLevel()
		{
		    return await _menu.GetTopLevel();
		}

        [HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}
	}
}