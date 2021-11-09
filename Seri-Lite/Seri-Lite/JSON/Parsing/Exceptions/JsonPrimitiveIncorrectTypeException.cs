using System;

namespace Seri_Lite.JSON.Parsing.Exceptions
{
    public class JsonPrimitiveIncorrectTypeException : Exception
    {
        private const string MESSAGE = "JsonPrimitive can not be of type: {0}";

        public Type InvalidType { get; private set; }

        public JsonPrimitiveIncorrectTypeException(Type invalidType) : base(string.Format(MESSAGE, invalidType))
        {
            InvalidType = invalidType;
        }
    }
}
