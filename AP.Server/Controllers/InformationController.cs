using System.Collections.Generic;
using System.Threading.Tasks;
using AP.ViewModel.Common;
using Microsoft.AspNetCore.Mvc;
using AP.ViewModel.Workshop;
using AP.Business.Model.Enums;
using AP.Business.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using AP.Business.Domain.Common.Category;

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
        public IEnumerable<CategoryViewModel> GetHierarchical()
        {
            return _menu.GetHierarchical();
        }

        [HttpGet]
        public IEnumerable<CategoryViewModel> GetTopLevel()
        {
            return _menu.GetTopLevel();
        }

        [HttpGet]
        public async Task<IEnumerable<AutobrandViewModel>> GetAutobrands(CarClassification autoType)
        {
            return await _autobrands.Get(autoType);
        }
    }
}