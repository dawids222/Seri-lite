namespace LibLite.Seri.Lite.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Changes first letter to upper or lower case depending on the input.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string InvertFirstLetter(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            var first = value[0];
            return char.IsUpper(first)
                ? char.ToLower(first) + value[1..]
                : char.ToUpper(first) + value[1..];
        }
    }
}
