using Seri_Lite.JSON.Parsing.Enums;
using Seri_Lite.JSON.Parsing.Exceptions;
using System;

namespace Seri_Lite.JSON.Parsing.Models
{
    public class JsonPrimitive : JsonToken
    {
        public override JsonTokenType TokenType => JsonTokenType.PRIMITIVE;

        private readonly JsonPrimitiveType _valueType;
        private readonly object _value;

        public JsonPrimitive() : this(null, null) { }
        public JsonPrimitive(JsonToken parent) : this(parent, null) { }
        public JsonPrimitive(string value) : this((object)value) { }
        public JsonPrimitive(int value) : this((object)value) { }
        public JsonPrimitive(double value) : this((object)value) { }
        public JsonPrimitive(bool value) : this((object)value) { }
        public JsonPrimitive(DateTime value) : this((object)value) { }
        public JsonPrimitive(Guid value) : this((object)value) { }
        public JsonPrimitive(object value) : this(null, value) { }
        public JsonPrimitive(JsonToken parent, string value) : this(parent, (object)value) { }
        public JsonPrimitive(JsonToken parent, int value) : this(parent, (object)value) { }
        public JsonPrimitive(JsonToken parent, double value) : this(parent, (object)value) { }
        public JsonPrimitive(JsonToken parent, bool value) : this(parent, (object)value) { }
        public JsonPrimitive(JsonToken parent, DateTime value) : this(parent, (object)value) { }
        public JsonPrimitive(JsonToken parent, Guid value) : this(parent, (object)value) { }
        private JsonPrimitive(JsonToken parent, object value) : base(parent)
        {
            _value = value;
            if (value is null) { _valueType = JsonPrimitiveType.NULL; }
            else if (value is string) { _valueType = JsonPrimitiveType.STRING; }
            else if (value is double) { _valueType = JsonPrimitiveType.DOUBLE; }
            else if (value is int) { _valueType = JsonPrimitiveType.INTEGER; }
            else if (value is bool) { _valueType = JsonPrimitiveType.BOOLEAN; }
            else if (value is DateTime || value is Guid)
            {
                _valueType = JsonPrimitiveType.STRING;
                _value = value.ToString();
            }
            else throw new JsonPrimitiveIncorrectTypeException(value.GetType());
        }

        public JsonPrimitiveType ValueType => _valueType;
        public object Value => _value;

        public bool IsNull => _valueType == JsonPrimitiveType.NULL;
        public bool IsString => _valueType == JsonPrimitiveType.STRING;
        public bool IsDouble => _valueType == JsonPrimitiveType.DOUBLE;
        public bool IsInteger => _valueType == JsonPrimitiveType.INTEGER;
        public bool IsBoolean => _valueType == JsonPrimitiveType.BOOLEAN;

        public bool CanBeDateTime => IsString && DateTime.TryParse(AsString(), out _);
        public bool CanBeGuid => IsString && Guid.TryParse(AsString(), out _);

        public object AsNull() => null;
        public string AsString() => _value as string;
        public double? AsDouble() => IsDouble ? (double)_value : null;
        public int? AsInteger() => IsInteger ? (int)_value : null;
        public bool? AsBoolean() => IsBoolean ? (bool)_value : null;
        public DateTime? AsDateTime() => CanBeDateTime ? DateTime.Parse(AsString()) : null;
        public Guid? AsGuid() => CanBeGuid ? Guid.Parse(AsString()) : null;
    }
}
