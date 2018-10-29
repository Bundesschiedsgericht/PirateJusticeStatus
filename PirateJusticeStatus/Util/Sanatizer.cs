using System;
namespace PirateJusticeStatus.Util
{
    public static class Sanatizer
    {
		public static string Sanatize(this string value)
		{
			return value
				.Replace("\"", string.Empty)
				.Replace("<", string.Empty)
				.Replace(">", string.Empty);
		}

		public static int TryParseInt(this string value, int defaultValue, int minValue, int maxValue)
		{
			int result = value.TryParseInt(defaultValue);

            if (result < minValue)
			{
				return defaultValue;
			}
            else if (result > maxValue)
			{
				return defaultValue;
			}
			else
			{
				return result;
			}
		}

        public static int TryParseInt(this string value, int defaultValue)
		{
			int result = defaultValue;
			int.TryParse(value, out result);
			return result;
		}

		public static T TryParseEnum<T>(this string value, T defaultValue) where T : struct, Enum
		{
			T result = defaultValue;

			if (Enum.TryParse<T>(value, out result))
			{
				return result;
			}
			else
			{
				return defaultValue;
			}
		}
    }
}
