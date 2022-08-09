using System;

namespace LibLite.Seri.Lite.JSON.Parsing.Exceptions
{
    public class JsonReadingException : Exception
    {
        private const string MESSAGE = "Unable to parse '{0}' token";

        public JsonReadingException(string token) : base(string.Format(MESSAGE, token)) { }
    }
}
