using AP.Business.Model.Enums;
using AP.ViewModel.Workshop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AP.Business.Domain.Common
{
    public interface IAutobrandService
    {
        Task<IEnumerable<AutobrandViewModel>> Get(CarClassification autoType);
    }
}
