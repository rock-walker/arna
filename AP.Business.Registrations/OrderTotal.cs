namespace AP.Business.Registration
{
    using AP.Business.Model.Registration;
    using System.Collections.Generic;

    public struct OrderTotal
    {
        public ICollection<OrderLine> Lines { get; set; }

        public decimal Total { get; set; }
    }
}