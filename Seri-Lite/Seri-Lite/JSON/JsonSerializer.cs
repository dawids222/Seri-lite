using Seri_Lite.JSON.Enums;
using Seri_Lite.JSON.Parsing.Models;
using Seri_Lite.JSON.Parsing.Readers;
using Seri_Lite.JSON.Serialization.Property;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Seri_Lite.JSON
{
    // TODO: saparate code into at least 2 files
    public class JsonSerializer : ISerializer
    {
        private readonly IJsonReader _jsonReader;
        private readonly NullPropertyBehaviour _nullPropertyBehaviour;
        private readonly IPropertyNameResolver _propertyNameResolver;

        public JsonSerializer()
        {
            _nullPropertyBehaviour = NullPropertyBehaviour.SERIALIZE;
            _propertyNameResolver = new InheritCasePropertyNameResolver();
            _jsonReader = new JsonReader();
        }

        public JsonSerializer(
            NullPropertyBehaviour nullPropertyBehaviour,
            IJsonReader jsonReader,
            IPropertyNameResolver propertyNameResolver)
        {
            _nullPropertyBehaviour = nullPropertyBehaviour;
            _jsonReader = jsonReader ??
                throw new ArgumentNullException(nameof(jsonReader));
            _propertyNameResolver = propertyNameResolver ??
                throw new ArgumentNullException(nameof(propertyNameResolver));
        }

        public T Deserialize<T>(string value)
        {
            var token = _jsonReader.Read(value);
            return (T)InnerDeserialize(typeof(T), token);
        }

        public object InnerDeserialize(Type type, JsonToken token)
        {
            if (token.IsObject) return DeserializeObject(type, token.AsObject());
            if (token.IsArray) return DeserializeArray(type, token.AsArray());
            if (token.IsPrimitive) return DeserializePrimitive(type, token.AsPrimitive());
            throw new Exception(); // TODO: implement dedicated exception
        }

        private object DeserializeObject(Type type, JsonObject obj)
        {
            var instance = Activator.CreateInstance(type);
            foreach (var prop in type.GetProperties())
            {
                var token = obj.GetToken(prop.Name);
                object val;
                if (token is null) { val = null; }
                else if (token.IsPrimitive) { val = InnerDeserialize(prop.PropertyType, token.AsPrimitive()); }
                else if (token.IsArray) { val = InnerDeserialize(prop.PropertyType, token.AsArray()); }
                else { val = InnerDeserialize(prop.PropertyType, token.AsObject()); }
                prop.SetValue(instance, val);
            }
            return instance;
        }

        private object DeserializeArray(Type type, JsonArray array)
        {
            var values = new List<dynamic>();
            foreach (var token in array.GetTokens())
            {
                //var t = type.GetGenericArguments()[0];
                dynamic val;
                if (token is null) { val = null; }
                else if (token.IsPrimitive) { val = InnerDeserialize(null, token.AsPrimitive()); }
                else if (token.IsArray) { val = InnerDeserialize(null, token.AsArray()); }
                else { val = InnerDeserialize(null, token.AsObject()); }
                values.Add((dynamic)val);
            }

            if (type.IsAssignableTo(typeof(Array)))
            {
                return values.ToArray();
            }
            else if (type.IsAssignableTo(typeof(ICollection)))
            {
                var instance = Activator.CreateInstance(type);
                var add = type.GetMethod("AddRange");
                add.Invoke(instance, new object[] { values });
                return instance;
            }

            return null; // TODO: throw?
        }

        private object DeserializePrimitive(Type type, JsonPrimitive primitive)
        {
            return primitive.Value;
        }

        public string Serialize(object value)
        {
            if (value is null) return "null";
            if (value is int) { return value.ToString(); }
            if (value is string)
            {
                value = value
                    .ToString()
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"");
                return $"\"{value}\"";
            }

            var stringBuilder = new StringBuilder();

            var type = value.GetType();
            if (IsPrimitive(type))
            {
                var serialized = SerializePrimitive(value);
                stringBuilder.Append(serialized);
            }
            else if (IsCollection(type))
            {
                var serialized = SerializeCollection(value);
                stringBuilder.Append(serialized);
            }
            else
            {
                var serialized = SerializeObject(value);
                stringBuilder.Append(serialized);
            }

            return stringBuilder.ToString();
        }

        private string Serialize(PropertyInfo property, object value)
        {
            var stringBuilder = new StringBuilder();

            if (IsPrimitive(property.PropertyType))
            {
                var serialized = SerializePrimitive(property, value);
                stringBuilder.Append(serialized);
            }
            else if (IsCollection(property.PropertyType))
            {
                var serialized = SerializeCollection(property, value);
                stringBuilder.Append(serialized);
            }
            else
            {
                var serialized = SerializeObject(property, value);
                stringBuilder.Append(serialized);
            }

            return stringBuilder.ToString();
        }

        private static bool IsPrimitive(Type type)
            => type.IsPrimitive ||
               type == typeof(Decimal) ||
               type == typeof(String) ||
               type == typeof(DateTime);

        private static bool IsCollection(Type type)
            => typeof(ICollection).IsAssignableFrom(type);

        private string SerializePrimitive(PropertyInfo property, object value)
        {
            var primitiveValue = property.GetValue(value);
            var serialized = SerializePrimitive(primitiveValue);
            var propertyName = _propertyNameResolver.Resolve(property);
            return $"\"{propertyName}\":{serialized}";
        }

        private static string SerializePrimitive(object value)
        {
            if (value is null) { return "null"; }
            var type = GetPrimitiveType(value);
            return SerializePrimitive(value, type);
        }

        private static string SerializePrimitive(object value, PrimitiveType type)
        {
            return type switch
            {
                PrimitiveType.STRING => $"\"{value}\"",
                PrimitiveType.BOOLEAN => value.ToString().ToLower(),
                PrimitiveType.NUMERIC => value.ToString().Replace(",", "."),
                _ => throw new NotImplementedException(),
            };
        }

        private static PrimitiveType GetPrimitiveType(object value)
        {
            var type = value.GetType();
            if (type == typeof(String)) { return PrimitiveType.STRING; }
            if (type == typeof(Boolean)) { return PrimitiveType.BOOLEAN; }
            return PrimitiveType.NUMERIC;
        }

        private string SerializeCollection(PropertyInfo property, object value)
        {
            var collectionValue = (ICollection)property.GetValue(value);
            var stringBuilder = new StringBuilder();
            var propertyName = _propertyNameResolver.Resolve(property);
            var serialized = SerializeCollection(collectionValue);
            stringBuilder.Append($"\"{propertyName}\":{serialized}");
            return stringBuilder.ToString();
        }

        private string SerializeCollection(object value)
        {
            if (value is null) { return "null"; }

            var collectionValue = (ICollection)value;
            var stringBuilder = new StringBuilder();

            var enumerator = collectionValue.GetEnumerator();
            stringBuilder.Append('[');
            while (enumerator.MoveNext())
            {
                var serialized = Serialize(enumerator.Current);
                stringBuilder.Append(serialized);
                stringBuilder.Append(',');
            }
            stringBuilder.Replace(",", "", stringBuilder.Length - 1, 1);
            stringBuilder.Append(']');

            return stringBuilder.ToString();
        }

        private string SerializeObject(PropertyInfo property, object value) // TODO: adapt for infinite reference loop
        {
            var objectValue = property.GetValue(value);
            if (ShouldIgnoreProperty(objectValue)) { return ""; }
            var serialized = Serialize(objectValue);
            var propertyName = _propertyNameResolver.Resolve(property);
            return $"\"{propertyName}\":{serialized}";
        }

        private bool ShouldIgnoreProperty(object value)
            => _nullPropertyBehaviour == NullPropertyBehaviour.IGNORE &&
               value is null;

        private string SerializeObject(object value) // TODO: adapt for infinite reference loop
        {
            var stringBuilder = new StringBuilder();
            var properties = value.GetType().GetProperties();
            var last = properties.LastOrDefault();
            stringBuilder.Append('{');
            foreach (var property in properties)
            {
                var objectValue = property.GetValue(value);
                if (ShouldIgnoreProperty(objectValue)) { break; }
                stringBuilder.Append(Serialize(property, value));
                if (property != last) { stringBuilder.Append(','); }
            }
            stringBuilder.Append('}');
            return stringBuilder.ToString(); ;
        }
    }
}
