﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AP.ViewModel.Common
{
    public class AddressViewModel
    {
        public string Street { get; set; }
        public string Building { get; set; }
        public CityViewModel City { get; set; }
    }
}