using System;
using System.Collections.Generic;
using System.Text;

namespace AP.Business.Model.Enums
{
    public enum BookingStatus : short
    {
        Ordered,
        DrivingTo,
        Arrived,
        InService,
        Completed,
        Canceled
    }
}
