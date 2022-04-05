using Seri_Lite.Extensions;
using Seri_Lite.JSON.Parsing.Models;
using Seri_Lite.JSON.Parsing.Readers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Seri_Lite.JSON.Serialization.Converters
{
    internal class JsonDeserializer
    {
        private readonly IJsonReader _jsonReader;

        public JsonDeserializer(IJsonReader jsonReader)
        {
            _jsonReader = jsonReader ?? throw new ArgumentNullException(nameof(jsonReader));
        }

        public T Deserialize<T>(string value)
        {
            return (T)Deserialize(typeof(T), value);
        }

        public object Deserialize(Type type, string value)
        {
            var token = _jsonReader.Read(value);
            return Deserialize(type, token);
        }

        private object Deserialize(Type type, JsonToken token)
        {
            if (token.IsObject) return DeserializeObject(type, token.AsObject());
            if (token.IsArray) return DeserializeArray(type, token.AsArray());
            if (token.IsPrimitive) return DeserializePrimitive(type, token.AsPrimitive());
            throw new Exception(); // TODO: implement dedicated exception
        }

        private object DeserializeObject(Type type, JsonObject obj)
        {
            if (type == typeof(object)) { return obj; }

            if (typeof(IDictionary).IsAssignableFrom(type)) { return DeserializeDictionary(type, obj); }

            var instance = Activator.CreateInstance(type);
            foreach (var prop in type.GetProperties())
            {
                var token = GetTokenCaseInsensitive(obj, prop.Name);
                object val;
                if (token is null) { val = null; }
                else if (token.IsPrimitive) { val = Deserialize(prop.PropertyType, token.AsPrimitive()); }
                else if (token.IsArray) { val = Deserialize(prop.PropertyType, token.AsArray()); }
                else { val = Deserialize(prop.PropertyType, token.AsObject()); }
                prop.SetValue(instance, val);
            }
            return instance;
        }

        private object DeserializeDictionary(Type type, JsonObject obj)
        {
            var instance = (IDictionary)Activator.CreateInstance(type);
            var keyType = instance.GetType().GetGenericArguments()[0];
            if (!keyType.IsJsonPrimitive())
            {
                throw new Exception($"{keyType.FullName} is unsupport IDictionary key type.");
            }
            var valueType = instance.GetType().GetGenericArguments()[1];
            foreach (var prop in obj.GetProperties())
            {
                var converter = TypeDescriptor.GetConverter(keyType);
                var key = converter.ConvertFromString(prop.Name);
                var value = Deserialize(valueType, prop.Value);
                instance.Add(key, value);
            }

            return instance;
        }

        private JsonToken GetTokenCaseInsensitive(JsonObject obj, string name)
        {
            var token = obj.GetToken(name);
            if (token is null)
            {
                var invertedName = name.InvertFirstLetter();
                token = obj.GetToken(invertedName);
            }
            return token;
        }

        private object DeserializeArray(Type type, JsonArray array)
        {
            if (type == typeof(object)) { return array; }

            var values = new List<dynamic>();

            var elementType = type.IsAssignableTo(typeof(Array))
                    ? type.GetElementType()
                    : type.GetGenericArguments()[0];

            foreach (var token in array.GetTokens())
            {
                dynamic val;
                if (token is null) { val = null; }
                else if (token.IsPrimitive) { val = Deserialize(elementType, token.AsPrimitive()); }
                else if (token.IsArray) { val = Deserialize(elementType, token.AsArray()); }
                else { val = Deserialize(elementType, token.AsObject()); }
                values.Add((dynamic)val);
            }

            if (type.IsAssignableTo(typeof(Array)))
            {
                var instance = Array.CreateInstance(type.GetElementType(), values.Count);
                for (var i = 0; i < values.Count; i++)
                {
                    instance.SetValue(values[i], i);
                }
                return instance;
            }
            else if (type.IsAssignableTo(typeof(IEnumerable)))
            {
                var instance = Activator.CreateInstance(type);
                var add = type.GetMethod("Add");
                if (add is null) { /* TODO: throw an exception? */ }
                foreach (var value in values)
                {
                    add.Invoke(instance, new object[] { value });
                }
                return instance;
            }

            return null; // TODO: throw?
        }
        private object DeserializePrimitive(Type type, JsonPrimitive primitive)
        {
            if (type == typeof(Guid)) { return primitive.AsGuid(); }
            if (type == typeof(DateTime)) { return primitive.AsDateTime(); }
            if (type == typeof(float)) { return (float?)primitive.AsDouble(); }
            return primitive.Value;
        }
    }
}
