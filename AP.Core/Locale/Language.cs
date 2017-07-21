namespace AP.Core.Locale
{
	public static class Language
	{
		public static int TwoLetterIsoLanguageName2Code(string code)
		{
			switch (code.Trim().ToLower())
			{
				case "en": return 1033;
				case "de": return 1031;
				case "fr": return 1036;
				case "it": return 1040;
				case "ja": return 1041;
				case "ko": return 1042;
				case "es": return 3082;
				case "zh": return 2052;
			}

			return 0;
		}
	}
}
