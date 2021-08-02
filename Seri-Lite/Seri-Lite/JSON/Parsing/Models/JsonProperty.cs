namespace Seri_Lite.JSON.Parsing.Models
{
    public class JsonProperty : JsonToken
    {
        public string Name { get; }
        public JsonToken Value { get; }

        public JsonProperty(string name, JsonToken value)
        {
            Name = name;
            Value = value;
        }
    }
}
