using System;

namespace AP.Core.Model.Attributes
{
    public class ValueAttribute : Attribute
    {
        public string Value { get; set; }
        public ValueAttribute(string value)
        {
            Value = value;
        }
    }
}
