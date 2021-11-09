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
        public void TokenType_ReturnsPrimitiveType()
        {
            var type = _jsonPrimitive.TokenType;

            Assert.AreEqual(JsonTokenType.PRIMITIVE, type);
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

        [TestCaseSource(typeof(CanBeDateTimeTestBundleSource))]
        public void CanBeDateTime_ReturnsTrueIfPrimitiveIsDateTimeFormattedString(CanBeDateTimeTestBundle bundle)
        {
            var primitive = new JsonPrimitive(bundle.Value);

            var result = primitive.CanBeDateTime;

            Assert.AreEqual(bundle.Expected, result);
        }

        class CanBeDateTimeTestBundleSource : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new CanBeDateTimeTestBundle(0, false);
                yield return new CanBeDateTimeTestBundle(1.5, false);
                yield return new CanBeDateTimeTestBundle(-1.5, false);
                yield return new CanBeDateTimeTestBundle(true, false);
                yield return new CanBeDateTimeTestBundle(null, false);
                yield return new CanBeDateTimeTestBundle("", false);
                yield return new CanBeDateTimeTestBundle("21-12-1996", true);
                yield return new CanBeDateTimeTestBundle("1996-12-21", true);
                yield return new CanBeDateTimeTestBundle("21.12.1996", true);
                yield return new CanBeDateTimeTestBundle("1996.12.21", true);
                yield return new CanBeDateTimeTestBundle("21/12/1996", true);
                yield return new CanBeDateTimeTestBundle("1996/12/21", true);
            }
        }

        public record CanBeDateTimeTestBundle(object Value, bool Expected);

        [TestCaseSource(typeof(AsDateTimeTestBundleSource))]
        public void AsDateTime_ReturnsDateTimeIfPrimitiveIsDateTimeFormattedString(AsDateTimeTestBundle bundle)
        {
            var primitive = new JsonPrimitive(bundle.Value);

            var result = primitive.AsDateTime();

            Assert.AreEqual(bundle.Expected, result);
        }

        class AsDateTimeTestBundleSource : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new AsDateTimeTestBundle(0, null);
                yield return new AsDateTimeTestBundle(1.5, null);
                yield return new AsDateTimeTestBundle(-1.5, null);
                yield return new AsDateTimeTestBundle(true, null);
                yield return new AsDateTimeTestBundle(null, null);
                yield return new AsDateTimeTestBundle("", null);
                yield return new AsDateTimeTestBundle("21-12-1996", DateTime.Parse("21-12-1996"));
                yield return new AsDateTimeTestBundle("1996-12-21", DateTime.Parse("1996-12-21"));
                yield return new AsDateTimeTestBundle("21.12.1996", DateTime.Parse("21.12.1996"));
                yield return new AsDateTimeTestBundle("1996.12.21", DateTime.Parse("1996.12.21"));
                yield return new AsDateTimeTestBundle("21/12/1996", DateTime.Parse("21/12/1996"));
                yield return new AsDateTimeTestBundle("1996/12/21", DateTime.Parse("1996/12/21"));
            }
        }

        public record AsDateTimeTestBundle(object Value, DateTime? Expected);

        [TestCaseSource(typeof(CanBeGuidTestBundleSource))]
        public void CanBeGuid_ReturnsTrueIfPrimitiveIsGuidFormattedString(CanBeGuidTestBundle bundle)
        {
            var primitive = new JsonPrimitive(bundle.Value);

            var result = primitive.CanBeGuid;

            Assert.AreEqual(bundle.Expected, result);
        }

        class CanBeGuidTestBundleSource : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new CanBeGuidTestBundle(0, false);
                yield return new CanBeGuidTestBundle(1.5, false);
                yield return new CanBeGuidTestBundle(-1.5, false);
                yield return new CanBeGuidTestBundle(true, false);
                yield return new CanBeGuidTestBundle(null, false);
                yield return new CanBeGuidTestBundle("", false);
                yield return new CanBeGuidTestBundle("21-12-1996", false);
                yield return new CanBeGuidTestBundle("53e5e27a.a89e-4ea9-84c5-47b34c200cd1", false);
                yield return new CanBeGuidTestBundle("53e5e27a-a89e-4ea9-84c547b34c200cd1", false);
                yield return new CanBeGuidTestBundle("53e5e27a-a89e-4ea9-84c5--7b34c200cd1", false);
                yield return new CanBeGuidTestBundle("53e5e27a-a89e-4ea9-84c5-47b34c200cd1", true);
                yield return new CanBeGuidTestBundle("b3ac1d78-2b78-4277-b99c-d6e3975b9f5e", true);
                yield return new CanBeGuidTestBundle("3fcce52a-b74f-4322-b7f0-848de6be9be4", true);
            }
        }

        public record CanBeGuidTestBundle(object Value, bool Expected);

        [TestCaseSource(typeof(AsGuidTestBundleSource))]
        public void AsGuid_ReturnsGuidIfPrimitiveIsGuidFormattedString(AsGuidTestBundle bundle)
        {
            var primitive = new JsonPrimitive(bundle.Value);

            var result = primitive.AsGuid();

            Assert.AreEqual(bundle.Expected, result);
        }

        class AsGuidTestBundleSource : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new AsGuidTestBundle(0, null);
                yield return new AsGuidTestBundle(1.5, null);
                yield return new AsGuidTestBundle(-1.5, null);
                yield return new AsGuidTestBundle(true, null);
                yield return new AsGuidTestBundle(null, null);
                yield return new AsGuidTestBundle("", null);
                yield return new AsGuidTestBundle("21-12-1996", null);
                yield return new AsGuidTestBundle("53e5e27a.a89e-4ea9-84c5-47b34c200cd1", null);
                yield return new AsGuidTestBundle("53e5e27a-a89e-4ea9-84c547b34c200cd1", null);
                yield return new AsGuidTestBundle("53e5e27a-a89e-4ea9-84c5--7b34c200cd1", null);
                yield return new AsGuidTestBundle("53e5e27a-a89e-4ea9-84c5-47b34c200cd1", Guid.Parse("53e5e27a-a89e-4ea9-84c5-47b34c200cd1"));
                yield return new AsGuidTestBundle("b3ac1d78-2b78-4277-b99c-d6e3975b9f5e", Guid.Parse("b3ac1d78-2b78-4277-b99c-d6e3975b9f5e"));
                yield return new AsGuidTestBundle("3fcce52a-b74f-4322-b7f0-848de6be9be4", Guid.Parse("3fcce52a-b74f-4322-b7f0-848de6be9be4"));
            }
        }

        public record AsGuidTestBundle(object Value, Guid? Expected);

        [TestCaseSource(typeof(JsonPrimitiveTestBundleSource))]
        public void PointsToRightParent_IsOfCorrectType_ReturnsCorrectValue(JsonPrimitiveTestBundle tester)
        {
            var jsonPrimitive = tester.JsonPrimitive;
            var expectedParent = tester.Parent;
            var expectedValueType = tester.ValueType;

            Assert.AreEqual(expectedParent, jsonPrimitive.Parent);

            Assert.AreEqual(expectedValueType, jsonPrimitive.ValueType);

            Assert.AreEqual(expectedValueType == JsonPrimitiveType.NULL, jsonPrimitive.IsNull);
            Assert.AreEqual(expectedValueType == JsonPrimitiveType.STRING, jsonPrimitive.IsString);
            Assert.AreEqual(expectedValueType == JsonPrimitiveType.DOUBLE, jsonPrimitive.IsDouble);
            Assert.AreEqual(expectedValueType == JsonPrimitiveType.INTEGER, jsonPrimitive.IsInteger);
            Assert.AreEqual(expectedValueType == JsonPrimitiveType.BOOLEAN, jsonPrimitive.IsBoolean);

            Assert.IsNull(jsonPrimitive.AsNull());
            if (expectedValueType == JsonPrimitiveType.STRING) Assert.AreEqual(jsonPrimitive.Value, jsonPrimitive.AsString()); else Assert.IsNull(jsonPrimitive.AsString());
            if (expectedValueType == JsonPrimitiveType.DOUBLE) Assert.AreEqual(jsonPrimitive.Value, jsonPrimitive.AsDouble()); else Assert.IsNull(jsonPrimitive.AsDouble());
            if (expectedValueType == JsonPrimitiveType.INTEGER) Assert.AreEqual(jsonPrimitive.Value, jsonPrimitive.AsInteger()); else Assert.IsNull(jsonPrimitive.AsInteger());
            if (expectedValueType == JsonPrimitiveType.BOOLEAN) Assert.AreEqual(jsonPrimitive.Value, jsonPrimitive.AsBoolean()); else Assert.IsNull(jsonPrimitive.AsBoolean());
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
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(parent), JsonPrimitiveType.NULL, parent);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(parent, ""), JsonPrimitiveType.STRING, parent);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(parent, 0.0), JsonPrimitiveType.DOUBLE, parent);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(parent, 0), JsonPrimitiveType.INTEGER, parent);
                yield return new JsonPrimitiveTestBundle(new JsonPrimitive(parent, true), JsonPrimitiveType.BOOLEAN, parent);
            }
        }

        public record JsonPrimitiveTestBundle(
            JsonPrimitive JsonPrimitive,
            JsonPrimitiveType ValueType = JsonPrimitiveType.NULL,
            JsonToken Parent = null);
    }
}
