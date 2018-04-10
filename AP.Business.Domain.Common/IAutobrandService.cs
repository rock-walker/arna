using AP.Business.Model.Enums;
using AP.ViewModel.Workshop;
using System.Collections.Generic;

namespace AP.Business.Domain.Common
{
    public interface IAutobrandService
    {
        IEnumerable<AutobrandViewModel> Get(CarClassification autoType);
    }
}
