using NUnit.Framework;
using Seri_Lite.JSON.Parsing.Enums;
using Seri_Lite.JSON.Parsing.Models;
using System;
using System.Collections;

namespace Seri_Lite_Unit_Tests.JSON.Parsing.Models
{
    [TestFixture]
    public class JsonPrimitiveTests
    {
        private JsonPrimitive _jsonPrimitive;

        [SetUp]
        public void SetUp()
        {
            _jsonPrimitive = new JsonPrimitive();
        }

        [Test]
        public void IsObject_ReturnsFalse()
        {
            Assert.IsFalse(_jsonPrimitive.IsObject);
        }

        [Test]
        public void IsArray_ReturnsFalse()
        {
            Assert.IsFalse(_jsonPrimitive.IsArray);
        }

        [Test]
        public void IsPrimitive_ReturnsTrue()
        {
            Assert.IsTrue(_jsonPrimitive.IsPrimitive);
        }

        [Test]
        public void AsObject_ReturnsNull()
        {
            var jsonObject = _jsonPrimitive.AsObject();

            Assert.IsNull(jsonObject);
        }

        [Test]
        public void AsArray_ReturnsNull()
        {
            var jsonArray = _jsonPrimitive.AsArray();

            Assert.IsNull(jsonArray);
        }

        [Test]
        public void AsPrimitive_ReturnsJsonPrimitive()
        {
            var jsonPrimitive = _jsonPrimitive.AsPrimitive();

            Assert.IsNotNull(jsonPrimitive);
        }

        [Test]
        public void Parent_HasNoParent_ReturnsNull()
        {
            Assert.IsNull(_jsonPrimitive.Parent);
        }

        [Test]
        public void Parent_HasParent_ReturnsParent()
        {
            var parent = new JsonPrimitive();
            var child = new JsonPrimitive(parent);

            Assert.AreEqual(parent, child.Parent);
        }

        [Test]
        public void Root_HasNoParent_ReturnsItself()
        {
            Assert.AreEqual(_jsonPrimitive, _jsonPrimitive.Root);
        }

        [Test]
        public void Root_HasParent_ReturnsRoot()
        {
            var root = new JsonPrimitive();
            var parent = new JsonPrimitive(root);
            var child = new JsonPrimitive(parent);

            Assert.AreEqual(root, child.Root);
        }

        [TestCaseSource(typeof(JsonPrimitiveTestBundleSource))]
        public void PointsToRightParent_IsOfCorrectType_ReturnsCorrectValue(JsonPrimitiveTestBundle tester)
        {
            var jsonPrimitive = tester.JsonPrimitive;
            var expectedParent = tester.Parent;
            var expectedType = tester.Type;

            Assert.AreEqual(expectedParent, jsonPrimitive.Parent);

            Assert.AreEqual(expectedType, jsonPrimitive.Type);

            Assert.AreEqual(expectedType == JsonPrimitiveType.NULL, jsonPrimitive.IsNull);
            Assert.AreEqual(expectedType == JsonPrimitiveType.STRING, jsonPrimitive.IsString);
            Assert.AreEqual(expectedType == JsonPrimitiveType.DOUBLE, jsonPrimitive.IsDouble);
            Assert.AreEqual(expectedType == JsonPrimitiveType.INTEGER, jsonPrimitive.IsInteger);
            Assert.AreEqual(expectedType == JsonPrimitiveType.BOOLEAN, jsonPrimitive.IsBoolean);
            Assert.AreEqual(expectedType == JsonPrimitiveType.DATE_TIME, jsonPrimitive.IsDateTime);

            Assert.IsNull(jsonPrimitive.AsNull());
            if (expectedType == JsonPrimitiveType.STRING) Assert.AreEqual(jsonPrimitive.Value, jsonPrimitive.AsString()); else Assert.IsNull(jsonPrimitive.AsString());
            if (expectedType == JsonPrimitiveType.DOUBLE) Assert.AreEqual(jsonPrimitive.Value, jsonPrimitive.AsDouble()); else Assert.IsNull(jsonPrimitive.AsDouble());
            if (expectedType == JsonPrimitiveType.INTEGER) Assert.AreEqual(jsonPrimitive.Value, jsonPrimitive.AsInteger()); else Assert.IsNull(jsonPrimitive.AsInteger());
            if (expectedType == JsonPrimitiveType.BOOLEAN) Assert.AreEqual(jsonPrimitive.Value, jsonPrimitive.AsBoolean()); else Assert.IsNull(jsonPrimitive.AsBoolean());
            if (expectedType == JsonPrimitiveType.DATE_TIME) Assert.AreEqual(jsonPrimitive.Value, jsonPrimitive.AsDateTime()); else Assert.IsNull(jsonPrimitive.AsDateTime());
        }

        class JsonPrimitiveTestBundleSource : IEnumerable
        {
            readonly JsonToken parent = new JsonObject();
            public IEnumerator GetEnumerator()
            {
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(), JsonPrimitiveType.NULL, null);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(""), JsonPrimitiveType.STRING, null);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(0.0), JsonPrimitiveType.DOUBLE, null);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(0), JsonPrimitiveType.INTEGER, null);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(true), JsonPrimitiveType.BOOLEAN, null);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(DateTime.Now), JsonPrimitiveType.DATE_TIME, null);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(parent), JsonPrimitiveType.NULL, parent);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(parent, ""), JsonPrimitiveType.STRING, parent);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(parent, 0.0), JsonPrimitiveType.DOUBLE, parent);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(parent, 0), JsonPrimitiveType.INTEGER, parent);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(parent, true), JsonPrimitiveType.BOOLEAN, parent);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(parent, DateTime.Now), JsonPrimitiveType.DATE_TIME, parent);
            }
        }

        public record JsonPrimitiveTestBundle(
            JsonPrimitive JsonPrimitive,
            JsonPrimitiveType Type = JsonPrimitiveType.NULL,
            JsonToken Parent = null);
    }
}
