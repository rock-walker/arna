﻿using AP.EntityModel.Common;
using System.Linq;
using AP.Business.Model.Common;
using AP.ViewModel.Workshop;

namespace AP.EntityModel.Mappers
{
    public static class CategoryMapper
    {
        public static CategoryModel MapTo(this CategoryData source)
        {
            return new CategoryModel
            {
                Id = source.Id,
                Parent = source.Parent,
                Sub = source.SubCategories != null
                    ? source.SubCategories.Select(MapTo)
                    : null,
                Title = source.Title,
                Link = source.Link
            };
        }

        public static WorkshopCategoryViewModel MapFrom(this CategoryData source)
        {
            return new WorkshopCategoryViewModel
            {
                CategoryID = source.Id,
                Title = source.Title
            };
        }
    }
}
