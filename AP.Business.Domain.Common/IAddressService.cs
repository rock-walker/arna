﻿using System;

namespace AP.Business.Domain.Common
{
    public interface IAddressService
    {
        Guid? GetCityByName(string name);
        Guid? AddCity(string cityName, string countryShort);
    }
}
