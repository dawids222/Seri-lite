using Seri_Lite.JSON.Parsing.Exceptions;
using Seri_Lite.JSON.Parsing.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Seri_Lite.JSON.Parsing.Readers
{
    public class JsonReader : IJsonReader
    {
        public JsonToken Read(string value)
        {
            value = Sanitize(value);
            return InnerRead(value);
        }

        private static string Sanitize(string value)
        {
            value = RemoveWhitespaces(value);
            return value;
        }
        private static string RemoveWhitespaces(string value) => Regex.Replace(value, @"(""[^""\\]*(?:\\.[^""\\]*)*"")|\s+", "$1");

        private JsonToken InnerRead(string value, JsonToken parent = null)
        {
            if (IsObject(value)) return ParseObject(value, parent);
            if (IsArray(value)) return ParseArray(value, parent);
            return ParsePrimitive(value, parent);
        }

        private static bool IsObject(string value) => value.StartsWith('{') && value.EndsWith('}');
        private static bool IsArray(string value) => value.StartsWith('[') && value.EndsWith(']');

        private JsonObject ParseObject(string value, JsonToken parent)
        {
            var result = new JsonObject(parent);
            var properties = new List<JsonProperty>();
            var v = GetSubstringBetweenFurtherest(value, '{', '}');
            var splited = v.Split(',');
            foreach (var s in splited)
            {
                var name = GetSubstringBetweenClosest(s, '"');
                var token = InnerRead(s[(s.IndexOf(':') + 1)..], result);
                properties.Add(new JsonProperty(name, token));
            }
            result.AddProperties(properties);
            return result;
        }

        private JsonArray ParseArray(string value, JsonToken parent)
        {
            var result = new JsonArray(parent);
            var v = GetSubstringBetweenFurtherest(value, '[', ']');
            var splited = v.Split(',');
            var tokens = new List<JsonToken>();
            foreach (var s in splited)
            {
                tokens.Add(InnerRead(s, result));
            }
            result.AddTokens(tokens);
            return result;
        }

        private static JsonPrimitive ParsePrimitive(string value, JsonToken parent)
        {
            if (IsNull(value)) { return new JsonPrimitive(parent); }
            if (IsString(value)) { return new JsonPrimitive(parent, GetString(value)); }
            if (IsDouble(value)) { return new JsonPrimitive(parent, GetDouble(value)); }
            if (IsInteger(value)) { return new JsonPrimitive(parent, GetInteger(value)); }
            if (IsBoolean(value)) { return new JsonPrimitive(parent, GetBoolean(value)); }
            throw new JsonReadingException(value);
        }

        private static bool IsNull(string value) => value.ToLower() == "null";
        private static bool IsString(string value) => value.Count(c => c == '"') == 2 && value.StartsWith('"') && value.EndsWith('"');
        private static bool IsDouble(string value) => (value.Contains('.') || value.Contains(',')) && double.TryParse(value.Replace('.', ','), out var _);
        private static bool IsInteger(string value) => int.TryParse(value, out var _);
        private static bool IsBoolean(string value) => bool.TryParse(value, out var _);

        private static string GetString(string value) => GetSubstringBetweenClosest(value, '"');
        private static double GetDouble(string value) => double.Parse(value.Replace('.', ','));
        private static int GetInteger(string value) => int.Parse(value);
        private static bool GetBoolean(string value) => bool.Parse(value);

        private static string GetSubstringBetweenClosest(string value, char character)
            => GetSubstringBetweenClosest(value, character, character);

        private static string GetSubstringBetweenClosest(string value, char start, char end)
            => Regex.Match(value, $@"\{start}([^{end}]*)\{end}").Groups[1].Value;

        private static string GetSubstringBetweenFurtherest(string value, char character)
            => GetSubstringBetweenFurtherest(value, character, character);

        private static string GetSubstringBetweenFurtherest(string value, char start, char end)
            => Regex.Match(value, $@"\{start}(.*[^{end}]*)\{end}").Groups[1].Value;
    }
}
