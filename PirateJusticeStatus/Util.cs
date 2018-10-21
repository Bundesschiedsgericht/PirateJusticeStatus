namespace System
{
    public static class Util
    {
		public static bool HasContent(this string value)
		{
			return !value.IsNullOrEmpty();
		}

		public static bool IsNullOrEmpty(this string value)
		{
			return value == null || value == string.Empty;
		}
    }
}
