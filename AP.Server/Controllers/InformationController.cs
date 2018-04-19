using System.Collections.Generic;
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
    [Route("api/[controller]")]
    public class InfoController : Controller
    {
        private readonly ICategoryService _menu;
        private readonly IAutobrandService autobrands;

        public InfoController(ICategoryService menu, IAutobrandService autobrands)
        {
            _menu = menu;
            this.autobrands = autobrands;
        }

        [HttpGet("workshop-categories")]
        public IEnumerable<CategoryViewModel> GetWorkshopCategories()
        {
            const int root = 1; //means "carservice" menu type;
            return _menu.GetHierarchical(root);
        }

        [HttpGet("get-top-level")]
        public IEnumerable<CategoryViewModel> GetTopLevel()
        {
            return _menu.GetTopLevel();
        }

        [HttpGet("manufacturers")]
        public IEnumerable<AutobrandViewModel> GetAutobrands(CarClassification autoType)
        {
            return autobrands.Get(autoType);
        }
    }
}