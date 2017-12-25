using AP.Business.Model.Common;
using AP.ViewModel.Common;
using AutoMapper;

namespace AP.Server.ViewMapping
{
    public class CommonProfile : Profile
    {
        public CommonProfile()
        {
            CreateMap<CategoryViewModel, CategoryModel>().ReverseMap();
        }
    }
}
