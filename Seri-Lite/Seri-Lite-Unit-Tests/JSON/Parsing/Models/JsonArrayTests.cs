using NUnit.Framework;
using Seri_Lite.JSON.Parsing.Models;
using System;
using System.Collections;
using System.Collections.Generic;

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
    }
}
