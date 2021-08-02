namespace Seri_Lite.JSON.Parsing.Models
{
    public abstract class JsonToken
    {
        public JsonToken Parent { get; }
        public JsonToken Root => GetRoot();

        protected JsonToken() { }
        protected JsonToken(JsonToken parent)
        {
            Parent = parent;
        }

        public bool IsObject => this is JsonObject;
        public bool IsArray => this is JsonArray;
        public bool IsPrimitive => this is JsonPrimitive;

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
