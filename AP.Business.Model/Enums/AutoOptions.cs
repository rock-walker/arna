using System;

namespace AP.Business.Model.Enums
{
    [Flags]
    public enum AutoOptions
    {
        AirCondition = 0x01,
        ABS = 0x02,
        HydroSteeringWheel = 0x04
    }
}
