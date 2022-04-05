using Seri_Lite.JSON.Enums;
using Seri_Lite.JSON.Parsing.Readers;
using Seri_Lite.JSON.Serialization.Converters;
using Seri_Lite.JSON.Serialization.Property;
using System;

namespace Seri_Lite.JSON
{
    // TODO: saparate code into at least 2 files
    public class JsonConverter : IConverter
    {
        private readonly JsonSerializer _serializer;
        private readonly JsonDeserializer _deserializer;

        public JsonConverter(
            NullPropertyBehaviour nullPropertyBehaviour,
            IJsonReader jsonReader,
            IPropertyNameResolver propertyNameResolver)
        {
            _serializer = new JsonSerializer(propertyNameResolver, nullPropertyBehaviour);
            _deserializer = new JsonDeserializer(jsonReader);
        }

        public T Deserialize<T>(string value) => _deserializer.Deserialize<T>(value);
        public object Deserialize(Type type, string value) => _deserializer.Deserialize(type, value);

        public string Serialize(object value) => _serializer.Serialize(value);
    }
}
