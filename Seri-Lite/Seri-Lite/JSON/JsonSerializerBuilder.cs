using Seri_Lite.JSON.Enums;
using Seri_Lite.JSON.Parsing.Readers;
using Seri_Lite.JSON.Serialization.Property;
using System;

namespace Seri_Lite.JSON
{
    public class JsonSerializerBuilder
    {
        private IJsonReader _jsonReader = new JsonReader();
        private NullPropertyBehaviour _nullPropertyBehaviour = NullPropertyBehaviour.SERIALIZE;
        private IPropertyNameResolver _propertyNameResolver = new InheritCasePropertyNameResolver();

        public JsonSerializer Build()
        {
            return new JsonSerializer(_nullPropertyBehaviour, _jsonReader, _propertyNameResolver);
        }

        public JsonSerializerBuilder SetJsonReader(IJsonReader jsonReader) => ReturnThis(() => _jsonReader = jsonReader);
        public JsonSerializerBuilder SetNullPropertyBehaviour(NullPropertyBehaviour nullPropertyBehaviour) => ReturnThis(() => _nullPropertyBehaviour = nullPropertyBehaviour);
        public JsonSerializerBuilder SetPropertyNameResolver(IPropertyNameResolver propertyNameResolver) => ReturnThis(() => _propertyNameResolver = propertyNameResolver);

        public JsonSerializerBuilder ReturnThis(Action action)
        {
            action.Invoke();
            return this;
        }
    }
}
