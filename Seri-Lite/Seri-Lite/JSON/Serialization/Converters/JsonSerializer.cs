using Seri_Lite.Extensions;
using Seri_Lite.JSON.Enums;
using Seri_Lite.JSON.Serialization.Property;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Seri_Lite.JSON.Serialization.Converters
{
    internal class JsonSerializer
    {
        private readonly NullPropertyBehaviour _nullPropertyBehaviour;
        private readonly IPropertyNameResolver _propertyNameResolver;

        public JsonSerializer(
            IPropertyNameResolver propertyNameResolver,
            NullPropertyBehaviour nullPropertyBehaviour)
        {
            _propertyNameResolver = propertyNameResolver ?? throw new ArgumentNullException(nameof(propertyNameResolver));
            _nullPropertyBehaviour = nullPropertyBehaviour;
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

            var type = value.GetType();
            if (type.IsJsonPrimitive())
            {
                return SerializePrimitive(value);
            }
            else if (IsCollection(type))
            {
                return SerializeCollection(value);
            }
            else
            {
                return SerializeObject(value);
            }
        }

        private string Serialize(PropertyInfo property, object value)
        {
            if (property.PropertyType.IsJsonPrimitive())
            {
                return SerializePrimitive(property, value);
            }
            else if (IsCollection(property.PropertyType))
            {
                return SerializeCollection(property, value);
            }
            else
            {
                return SerializeObject(property, value);
            }
        }

        private static bool IsCollection(Type type)
            => typeof(IEnumerable).IsAssignableFrom(type);

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
                PrimitiveType.STRING or
                PrimitiveType.GUID => $"\"{value}\"",
                PrimitiveType.BOOLEAN => value.ToString().ToLower(),
                PrimitiveType.NUMERIC => value.ToString().Replace(",", "."),
                PrimitiveType.DATE_TIME => $"\"{DateTime.Parse(value.ToString()):yyyy-MM-ddTHH:mm:ss}\"",
                _ => throw new NotImplementedException(),
            };
        }

        private static PrimitiveType GetPrimitiveType(object value)
        {
            var type = value.GetType();
            if (type == typeof(String)) { return PrimitiveType.STRING; }
            if (type == typeof(Boolean)) { return PrimitiveType.BOOLEAN; }
            if (type == typeof(DateTime)) { return PrimitiveType.DATE_TIME; }
            if (type == typeof(Guid)) { return PrimitiveType.GUID; }
            return PrimitiveType.NUMERIC;
        }

        private string SerializeCollection(PropertyInfo property, object value)
        {
            var collectionValue = (IEnumerable)property.GetValue(value);
            var propertyName = _propertyNameResolver.Resolve(property);
            var serialized = SerializeCollection(collectionValue);
            return $"\"{propertyName}\":{serialized}";
        }

        private string SerializeCollection(object value)
        {
            if (value is null) { return "null"; }

            if (value is IDictionary) { return SerializeDictionary(value); }

            var collectionValue = (IEnumerable)value;
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

        private string SerializeDictionary(object value)
        {
            var dictionary = (IDictionary)value;
            var stringBuilder = new StringBuilder();
            stringBuilder.Append('{');
            foreach (var key in dictionary.Keys)
            {
                var keyType = key.GetType();
                if (!keyType.IsJsonPrimitive())
                {
                    throw new Exception($"{keyType.FullName} is unsupport IDictionary key type.");
                }

                var serializedValue = Serialize(dictionary[key]);
                stringBuilder.Append($"\"{key}\"");
                stringBuilder.Append($":");
                stringBuilder.Append($"{serializedValue}");
                stringBuilder.Append(',');
            }
            stringBuilder.Replace(",", "", stringBuilder.Length - 1, 1);
            stringBuilder.Append('}');
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
