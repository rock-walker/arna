﻿using AP.ViewModel.Workshop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AP.Business.AutoDomain.Workshop.Contracts
{
    public interface IWorkshopService
    {
        IEnumerable<WorkshopViewModel> GetById(IEnumerable<Guid> id);
        Task<IEnumerable<WorkshopShortViewModel>> GetByLocation(double latitude, double longitude, double distance);
        Task<IEnumerable<WorkshopShortViewModel>> GetAll();
    }
}
