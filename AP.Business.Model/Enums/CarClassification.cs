using System;

namespace AP.Business.Model.Enums
{
    [Flags]
    public enum CarClassification
    {
        Standart = 0x01,
        Truck = 0x02,
        Bus = 0x04
    }
}
