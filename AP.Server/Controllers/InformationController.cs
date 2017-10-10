using System.Collections.Generic;
using System.Threading.Tasks;
using AP.ViewModel.Common;
using AP.Shared.Category;
using Microsoft.AspNetCore.Mvc;
using AP.ViewModel.Workshop;
using AP.Business.Model.Enums;
using AP.Business.Domain.Common;
using Microsoft.AspNetCore.Authorization;

namespace AP.Application.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
	public class InfoController : Controller
	{
	    private readonly ICategoryService _menu;
        private readonly IAutobrandService _autobrands;

        public InfoController(ICategoryService menu, IAutobrandService autobrands)
        {
            _menu = menu;
            _autobrands = autobrands;
        }

        [HttpGet]
		public async Task<IEnumerable<MenuViewModel>> GetHierarchical()
		{
		    return await _menu.GetHierarchical();
		}

        [HttpGet]
		public async Task<IEnumerable<MenuViewModel>> GetTopLevel()
		{
		    return await _menu.GetTopLevel();
		}

        [HttpGet]
        public async Task<IEnumerable<AutobrandViewModel>> GetAutobrands(CarClassification autoType)
        {
            return await _autobrands.Get(autoType);
        }
	}
}