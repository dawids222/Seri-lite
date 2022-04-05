using Seri_Lite.JSON.Enums;
using Seri_Lite.JSON.Parsing.Readers;
using Seri_Lite.JSON.Serialization.Property;
using System;

namespace Seri_Lite.JSON
{
    public class JsonConverterBuilder
    {
        private IJsonReader _jsonReader = new JsonReader();
        private NullPropertyBehaviour _nullPropertyBehaviour = NullPropertyBehaviour.SERIALIZE;
        private IPropertyNameResolver _propertyNameResolver = new InheritCasePropertyNameResolver();

        public JsonConverter Build()
        {
            return new JsonConverter(_nullPropertyBehaviour, _jsonReader, _propertyNameResolver);
        }

        public JsonConverterBuilder SetJsonReader(IJsonReader jsonReader) => ReturnThis(() => _jsonReader = jsonReader);
        public JsonConverterBuilder SetNullPropertyBehaviour(NullPropertyBehaviour nullPropertyBehaviour) => ReturnThis(() => _nullPropertyBehaviour = nullPropertyBehaviour);
        public JsonConverterBuilder SetPropertyNameResolver(IPropertyNameResolver propertyNameResolver) => ReturnThis(() => _propertyNameResolver = propertyNameResolver);

        public JsonConverterBuilder ReturnThis(Action action)
        {
            action.Invoke();
            return this;
        }
    }
}
