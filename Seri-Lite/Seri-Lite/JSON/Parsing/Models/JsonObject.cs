using Seri_Lite.JSON.Parsing.Enums;
using Seri_Lite.JSON.Parsing.Exceptions;
using System;
using System.Collections.Generic;

namespace Seri_Lite.JSON.Parsing.Models
{
    public class JsonObject : JsonToken
    {
        public override JsonTokenType TokenType => JsonTokenType.OBJECT;

        private readonly Dictionary<string, JsonProperty> _properties = new();

        public JsonObject() : base() { }
        public JsonObject(JsonToken parent) : base(parent) { }
        public JsonObject(IEnumerable<JsonProperty> properties) : this(null, properties) { }
        public JsonObject(JsonToken parent, IEnumerable<JsonProperty> properties) : base(parent)
        {
            AddProperties(properties);
        }

        public IEnumerable<JsonProperty> GetProperties() => _properties.Values;

        public JsonProperty GetProperty(string name) => PropertyExists(name) ? _properties[name] : null;

        public JsonToken GetToken(string name) => GetProperty(name)?.Value;

        public JsonObject GetObject(string name) => GetToken(name)?.AsObject();

        public JsonArray GetArray(string name) => GetToken(name)?.AsArray();

        public JsonPrimitive GetPrimitive(string name) => GetToken(name)?.AsPrimitive();

        public bool PropertyExists(string name) => _properties.ContainsKey(name);

        public void AddProperty(JsonProperty property)
        {
            try { _properties.Add(property.Name, property); }
            catch (ArgumentException) { throw new PropertyAlreadyExistsException(); }
        }

        public void AddProperties(IEnumerable<JsonProperty> properties)
        {
            foreach (var property in properties) AddProperty(property);
        }
    }
}
