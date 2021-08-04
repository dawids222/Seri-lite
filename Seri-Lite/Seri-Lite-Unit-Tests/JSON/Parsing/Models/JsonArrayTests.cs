using NUnit.Framework;
using Seri_Lite.JSON.Parsing.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Seri_Lite_Unit_Tests.JSON.Parsing.Models
{
    [TestFixture]
    public class JsonArrayTests
    {
        private JsonArray _jsonArray;

        [SetUp]
        public void SetUp()
        {
            _jsonArray = new JsonArray();
        }

        [Test]
        public void IsObject_ReturnsFalse()
        {
            Assert.IsFalse(_jsonArray.IsObject);
        }

        [Test]
        public void IsArray_ReturnsTrue()
        {
            Assert.IsTrue(_jsonArray.IsArray);
        }

        [Test]
        public void IsPrimitive_ReturnsFalse()
        {
            Assert.IsFalse(_jsonArray.IsPrimitive);
        }

        [Test]
        public void AsObject_ReturnsNull()
        {
            var jsonObject = _jsonArray.AsObject();

            Assert.IsNull(jsonObject);
        }

        [Test]
        public void AsArray_ReturnsJsonArray()
        {
            var jsonArray = _jsonArray.AsArray();

            Assert.IsNotNull(jsonArray);
        }

        [Test]
        public void AsPrimitive_ReturnsNull()
        {
            var jsonPrimitive = _jsonArray.AsPrimitive();

            Assert.IsNull(jsonPrimitive);
        }

        [Test]
        public void Parent_HasNoParent_ReturnsNull()
        {
            Assert.IsNull(_jsonArray.Parent);
        }

        [Test]
        public void Parent_HasParent_ReturnsParent()
        {
            var parent = new JsonArray();
            var child = new JsonArray(parent);

            Assert.AreEqual(parent, child.Parent);
        }

        [Test]
        public void Root_HasNoParent_ReturnsItself()
        {
            Assert.AreEqual(_jsonArray, _jsonArray.Root);
        }

        [Test]
        public void Root_HasParent_ReturnsRoot()
        {
            var root = new JsonArray();
            var parent = new JsonArray(root);
            var child = new JsonArray(parent);

            Assert.AreEqual(root, child.Root);
        }

        [Test]
        public void Count_ReturnsNumberOfContainedTokens()
        {
            var primitives = new JsonPrimitive[]
            {
                new JsonPrimitive(),
                new JsonPrimitive(),
            };

            _jsonArray = new JsonArray(primitives);

            Assert.AreEqual(primitives.Length, _jsonArray.Count);
        }

        [TestCaseSource(typeof(HasOnlyTestBundleSource))]
        public void HasOnly_ReturnsValueDependentOnTheSituation(HasOnlyTestBundle bundle)
        {
            _jsonArray = new JsonArray(bundle.Tokens);

            Assert.AreEqual(bundle.HasOnly.Objects, _jsonArray.HasOnlyObjects);
            Assert.AreEqual(bundle.HasOnly.Arrays, _jsonArray.HasOnlyArrays);
            Assert.AreEqual(bundle.HasOnly.Primitives, _jsonArray.HasOnlyPrimitives);
            Assert.AreEqual(bundle.HasOnly.Mixed, _jsonArray.HasMixedTokens);
        }

        class HasOnlyTestBundleSource : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new HasOnlyTestBundle(
                    new JsonToken[] { new JsonObject() },
                    new HasOnlyTest(Objects: true));
                yield return new HasOnlyTestBundle(
                    new JsonToken[] { new JsonArray() },
                    new HasOnlyTest(Arrays: true));
                yield return new HasOnlyTestBundle(
                    new JsonToken[] { new JsonPrimitive() },
                    new HasOnlyTest(Primitives: true));
                yield return new HasOnlyTestBundle(
                    Array.Empty<JsonToken>(),
                    new HasOnlyTest());
                yield return new HasOnlyTestBundle(
                    new JsonToken[]
                    {
                        new JsonObject(),
                        new JsonArray(),
                        new JsonPrimitive(),
                    },
                    new HasOnlyTest(Mixed: true));
            }
        }

        public record HasOnlyTestBundle(
            IEnumerable<JsonToken> Tokens,
            HasOnlyTest HasOnly);

        public record HasOnlyTest(
            bool Objects = false,
            bool Arrays = false,
            bool Primitives = false,
            bool Mixed = false);

        [TestCaseSource(typeof(GetMultipleTestBundleSource))]
        public void GetMultiple_ReturnsValueDependentOnTheSituation(GetMultipleTestBundle bundle)
        {
            var tokens = bundle.Tokens;
            _jsonArray = new JsonArray(tokens);

            Assert.AreEqual(tokens, _jsonArray.GetTokens());
            Assert.AreEqual(bundle.Expected.Objects, _jsonArray.GetObjects().Count());
            Assert.AreEqual(bundle.Expected.Arrays, _jsonArray.GetArrays().Count());
            Assert.AreEqual(bundle.Expected.Primitives, _jsonArray.GetPrimitives().Count());
        }

        public class GetMultipleTestBundleSource : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new GetMultipleTestBundle(
                    Array.Empty<JsonToken>(),
                    new GetMultipleTest());
                yield return new GetMultipleTestBundle(
                    new JsonToken[] { new JsonObject() },
                    new GetMultipleTest(Objects: 1));
                yield return new GetMultipleTestBundle(
                    new JsonToken[] { new JsonArray() },
                    new GetMultipleTest(Arrays: 1));
                yield return new GetMultipleTestBundle(
                    new JsonToken[] { new JsonPrimitive() },
                    new GetMultipleTest(Primitives: 1));
                yield return new GetMultipleTestBundle(
                    new JsonToken[]
                    {
                        new JsonObject(),
                        new JsonArray(),
                        new JsonArray(),
                        new JsonPrimitive(),
                        new JsonPrimitive(),
                        new JsonPrimitive(),
                    },
                    new GetMultipleTest(Objects: 1, Arrays: 2, Primitives: 3));
            }
        }

        public record GetMultipleTestBundle(
            IEnumerable<JsonToken> Tokens,
            GetMultipleTest Expected);

        public record GetMultipleTest(
            int Objects = 0,
            int Arrays = 0,
            int Primitives = 0);

        [Test]
        public void GetSingle_ReturnsValueDependentOnTheSituation()
        {
            var tokens = new JsonToken[] { new JsonObject(), new JsonArray(), new JsonPrimitive() };
            _jsonArray = new JsonArray(tokens);

            Assert.AreEqual(tokens.ElementAt(0), _jsonArray.GetToken(0));
            Assert.AreEqual(tokens.ElementAt(0), _jsonArray.GetObject(0));
            Assert.AreEqual(null, _jsonArray.GetArray(0));
            Assert.AreEqual(null, _jsonArray.GetPrimitive(0));

            Assert.AreEqual(tokens.ElementAt(1), _jsonArray.GetToken(1));
            Assert.AreEqual(null, _jsonArray.GetObject(1));
            Assert.AreEqual(tokens.ElementAt(1), _jsonArray.GetArray(1));
            Assert.AreEqual(null, _jsonArray.GetPrimitive(1));

            Assert.AreEqual(tokens.ElementAt(2), _jsonArray.GetToken(2));
            Assert.AreEqual(null, _jsonArray.GetObject(2));
            Assert.AreEqual(null, _jsonArray.GetArray(2));
            Assert.AreEqual(tokens.ElementAt(2), _jsonArray.GetPrimitive(2));
        }
    }
}
