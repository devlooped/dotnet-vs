namespace System
{
    static class StringExtensions
    {
        public static string GetNormalizedString(this string value, int length = 20)
        {
            if (length == 0)
                return string.Empty;
            else if (string.IsNullOrEmpty(value))
                return new string(' ', length);
            else if (value.Length < length)
                return string.Concat(value, new String(' ', length - value.Length));
            else if (value.Length >= length)
                return value.Substring(0, length - 4) + "... ";

            return value;
        }
    }
}
