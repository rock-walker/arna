using System;
using System.Globalization;

namespace AP.Core.Locale
{
	public static class CultureInfoHelper
	{
		public static CultureInfo GetCultureInfoSafe(string name)
		{
			try
			{
				return CultureInfo.CurrentUICulture;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
