﻿using Seri_Lite.JSON.Parsing.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Seri_Lite.JSON.Parsing.Models
{
    public class JsonArray : JsonToken
    {
        public override JsonTokenType TokenType => JsonTokenType.ARRAY;

        private readonly List<JsonToken> _tokens = new();

        public JsonArray() : this(null, Array.Empty<JsonToken>()) { }
        public JsonArray(JsonToken parent) : this(parent, Array.Empty<JsonToken>()) { }
        public JsonArray(IEnumerable<JsonToken> tokens) : this(null, tokens) { }
        public JsonArray(JsonToken parent, IEnumerable<JsonToken> tokens) : base(parent)
        {
            _tokens.AddRange(tokens);
        }

        public int Count => _tokens.Count;
        public bool IsEmpty => Count == 0;

        public bool HasOnlyObjects => !IsEmpty && GetTokens().All(t => t.IsObject);
        public bool HasOnlyArrays => !IsEmpty && GetTokens().All(t => t.IsArray);
        public bool HasOnlyPrimitives => !IsEmpty && GetTokens().All(t => t.IsPrimitive);
        public bool HasMixedTokens => !IsEmpty && GetTokens().Select(t => t.TokenType).Distinct().Count() > 1;

        public IEnumerable<JsonToken> GetTokens() => _tokens;
        public IEnumerable<JsonObject> GetObjects() => _tokens.Cast<JsonObject>();
        public IEnumerable<JsonArray> GetArrays() => _tokens.Cast<JsonArray>();
        public IEnumerable<JsonPrimitive> GetPrimitives() => _tokens.Cast<JsonPrimitive>();

        public JsonToken GetToken(int index) => GetTokens().ElementAt(index);
        public JsonObject GetObject(int index) => GetObjects().ElementAt(index);
        public JsonArray GetArray(int index) => GetArrays().ElementAt(index);
        public JsonPrimitive GetPrimitive(int index) => GetPrimitives().ElementAt(index);
    }
}
