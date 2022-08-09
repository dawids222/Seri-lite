using LibLite.Seri.Lite.JSON.Parsing.Enums;

namespace LibLite.Seri.Lite.JSON.Parsing.Models
{
    public abstract class JsonToken
    {
        public abstract JsonTokenType TokenType { get; }
        public JsonToken Parent { get; }
        public JsonToken Root => GetRoot();

        protected JsonToken() { }
        protected JsonToken(JsonToken parent)
        {
            Parent = parent;
        }

        public bool IsObject => TokenType == JsonTokenType.OBJECT;
        public bool IsArray => TokenType == JsonTokenType.ARRAY;
        public bool IsPrimitive => TokenType == JsonTokenType.PRIMITIVE;

        public JsonObject AsObject() => this as JsonObject;
        public JsonArray AsArray() => this as JsonArray;
        public JsonPrimitive AsPrimitive() => this as JsonPrimitive;

        private JsonToken GetRoot()
        {
            var parent = this;
            while (parent.Parent is not null)
            {
                parent = parent.Parent;
            }
            return parent;
        }
    }
}
