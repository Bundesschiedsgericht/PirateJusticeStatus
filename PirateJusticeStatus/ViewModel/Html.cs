using System;
namespace PirateJusticeStatus.ViewModel
{
    public static class Html
    {
		public static string ConvertToHtml(this string value)
		{
			return value
				.Replace("ö", "&ouml;")
				.Replace("ä", "&auml;")
				.Replace("ü", "&uuml;");
		}
    }
}
