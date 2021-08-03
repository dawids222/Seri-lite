using Seri_Lite.JSON.Parsing.Enums;
using System;

namespace Seri_Lite.JSON.Parsing.Models
{
    public class JsonPrimitive : JsonToken
    {
        private readonly JsonPrimitiveType _type;
        private readonly object _value;

        public JsonPrimitive() : this(null, null) { }
        public JsonPrimitive(JsonToken parent) : this(parent, null) { }
        public JsonPrimitive(string value) : this((object)value) { }
        public JsonPrimitive(int value) : this((object)value) { }
        public JsonPrimitive(double value) : this((object)value) { }
        public JsonPrimitive(bool value) : this((object)value) { }
        public JsonPrimitive(DateTime value) : this((object)value) { }
        private JsonPrimitive(object value) : this(null, value) { }
        public JsonPrimitive(JsonToken parent, string value) : this(parent, (object)value) { }
        public JsonPrimitive(JsonToken parent, int value) : this(parent, (object)value) { }
        public JsonPrimitive(JsonToken parent, double value) : this(parent, (object)value) { }
        public JsonPrimitive(JsonToken parent, bool value) : this(parent, (object)value) { }
        public JsonPrimitive(JsonToken parent, DateTime value) : this(parent, (object)value) { }
        private JsonPrimitive(JsonToken parent, object value) : base(parent)
        {
            _value = value;
            if (value is null) { _type = JsonPrimitiveType.NULL; }
            if (value is string) { _type = JsonPrimitiveType.STRING; }
            if (value is double) { _type = JsonPrimitiveType.DOUBLE; }
            if (value is int) { _type = JsonPrimitiveType.INTEGER; }
            if (value is bool) { _type = JsonPrimitiveType.BOOLEAN; }
            if (value is DateTime) { _type = JsonPrimitiveType.DATE_TIME; }
        }

        public JsonPrimitiveType Type => _type;
        public object Value => _value;

        public bool IsNull => _type == JsonPrimitiveType.NULL;
        public bool IsString => _type == JsonPrimitiveType.STRING;
        public bool IsDouble => _type == JsonPrimitiveType.DOUBLE;
        public bool IsInteger => _type == JsonPrimitiveType.INTEGER;
        public bool IsBoolean => _type == JsonPrimitiveType.BOOLEAN;
        public bool IsDateTime => _type == JsonPrimitiveType.DATE_TIME;

        public object AsNull() => null;
        public string AsString() => _value as string;
        public double? AsDouble() => IsDouble ? (double)_value : null;
        public int? AsInteger() => IsInteger ? (int)_value : null;
        public bool? AsBoolean() => IsBoolean ? (bool)_value : null;
        public DateTime? AsDateTime() => IsDateTime ? (DateTime)_value : null;
    }
}
