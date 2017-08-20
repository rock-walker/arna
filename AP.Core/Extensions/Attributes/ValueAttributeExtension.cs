using AP.Core.Model.Attributes;
using AP.Core.Model.User;
using System.Reflection;

namespace AP.Core.Extensions.Attributes
{
    public static class ValueAttributeExtension
    {
        public static string GetValue(this Roles source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            ValueAttribute[] attributes = (ValueAttribute[])fi.GetCustomAttributes(
                typeof(ValueAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Value;
            else return source.ToString();
        }
    }
}
