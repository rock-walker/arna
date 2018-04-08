using System.Reflection;

namespace AP.Core.Extensions
{
    public static class StringExtensions
    {
        public static T TryParseValue<T>(this string s) where T : struct
        {
            var method = typeof(T).GetMethod(
                "TryParse",
                new[] { typeof(string), typeof(T).MakeByRefType() }
                );
            T result = default(T);
            var parameters = new object[] { s, result };
            method.Invoke(null, parameters);
            return (T)parameters[1];
        }
    }
}
