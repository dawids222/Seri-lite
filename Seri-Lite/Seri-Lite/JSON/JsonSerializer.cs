using Seri_Lite.JSON.Enums;
using Seri_Lite.JSON.Serialization.Property;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Seri_Lite.JSON
{
    public class JsonSerializer : ISerializer
    {
        private IPropertyNameResolver _propertyNameResolver;

        public JsonSerializer(
            IPropertyNameResolver propertyNameResolver)
        {
            _propertyNameResolver = propertyNameResolver ??
                throw new ArgumentNullException(nameof(propertyNameResolver));
        }

        public T Deserialize<T>(string value)
        {
            throw new NotImplementedException();
        }

        public string Serialize(object value)
        {
            if (value is null) return "null"; // TODO: add abstract null handling mechanism
            if (value is int) { return value.ToString(); }
            if (value is string) { return $"\"{value}\""; }

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
            var serialized = Serialize(objectValue);
            var propertyName = _propertyNameResolver.Resolve(property);
            return $"\"{propertyName}\":{serialized}";
        }
        private string SerializeObject(object value) // TODO: adapt for infinite reference loop
        {
            var stringBuilder = new StringBuilder();
            var properties = value.GetType().GetProperties();
            var last = properties.LastOrDefault();
            stringBuilder.Append('{');
            foreach (var property in properties)
            {
                // TODO: add ignore NULL?
                stringBuilder.Append(Serialize(property, value));
                if (property != last) { stringBuilder.Append(','); }
            }
            stringBuilder.Append('}');
            return stringBuilder.ToString(); ;
        }
    }
}
