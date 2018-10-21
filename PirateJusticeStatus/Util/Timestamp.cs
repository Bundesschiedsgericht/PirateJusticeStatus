using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace System
{
	public static class Timestamp
	{
		public static DateTime Default { get { return new DateTime(1990, 1, 1); } }

		public static string Format(this DateTime value)
		{
			return value.ToString("yyyy-MM-dd HH:mm:ss");
		}

        public static DateTime ParseTimestamp(this string value)
		{
			DateTime timestamp;

			if (DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out timestamp))
			{
				return timestamp;
			}
			else
			{
				return Timestamp.Default;
			}
		}
	}
}
