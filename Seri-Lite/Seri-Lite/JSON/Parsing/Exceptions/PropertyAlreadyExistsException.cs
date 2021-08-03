using System;

namespace Seri_Lite.JSON.Parsing.Exceptions
{
    public class PropertyAlreadyExistsException : Exception
    {
        private const string MESSAGE = "JSON object already contains a property with this name";

        public PropertyAlreadyExistsException() : base(MESSAGE) { }
    }
}
