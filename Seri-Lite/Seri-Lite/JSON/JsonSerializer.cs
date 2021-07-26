using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Seri_Lite.JSON
{
    public class JsonSerializer : ISerializer
    {
        public T Deserialize<T>(string value)
        {
            throw new NotImplementedException();
        }

        public string Serialize(object value)
        {
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

        private static bool IsPrimitive(Type type) => type.IsPrimitive || type == typeof(Decimal) || type == typeof(String) || type == typeof(DateTime);
        private static bool IsCollection(Type type) => typeof(ICollection).IsAssignableFrom(type);

        private static string SerializePrimitive(PropertyInfo property, object value)
        {
            try
            {
                var v = property.GetValue(value); // TODO: adapt for string/number/bool
                return $"\"{property.Name}\":\"{v}\"";
            }
            catch (Exception)
            {
                return value.ToString();
            }
        }
        private static string SerializePrimitive(object value)
        {
            return $"\"{value}\"";
        }
        private string SerializeCollection(PropertyInfo property, object value)
        {
            var v = property.GetValue(value) as ICollection;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"\"{property.Name}\":[");

            var last = GetLastElementOf(v);
            foreach (var x in v)
            {
                var serialized = Serialize(x);
                stringBuilder.Append(serialized);
                if (x != last)
                {
                    stringBuilder.Append(",");
                }
            }

            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }
        private string SerializeCollection(object value)
        {
            var v = value as ICollection;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[");

            var last = GetLastElementOf(v);
            foreach (var x in v)
            {
                var serialized = Serialize(x);
                stringBuilder.Append(serialized);
                if (x != last)
                {
                    stringBuilder.Append(",");
                }
            }

            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }
        private string SerializeObject(PropertyInfo property, object value) // TODO: adapt for infinite reference loop
        {
            var v = property.GetValue(value);
            var serialized = Serialize(v);
            return $"\"{property.Name}\":{serialized}";
        }
        private string SerializeObject(object value) // TODO: adapt for infinite reference loop
        {
            var stringBuilder = new StringBuilder();
            var properties = value.GetType().GetProperties();
            var last = properties.LastOrDefault();
            stringBuilder.Append("{");
            foreach (var property in properties)
            {
                // TODO: add ignore NULL?
                // TODO: add case settings
                stringBuilder.Append(Serialize(property, value));
                if (property != last) { stringBuilder.Append(","); }
            }
            stringBuilder.Append("}");
            return stringBuilder.ToString(); ;
        }

        // TODO: move to extension
        private object GetLastElementOf(ICollection collection)
        {
            var enumerator = collection.GetEnumerator();
            object current = null;
            while (enumerator.MoveNext())
            {
                current = enumerator.Current;
            }
            return current;
        }
    }
}
