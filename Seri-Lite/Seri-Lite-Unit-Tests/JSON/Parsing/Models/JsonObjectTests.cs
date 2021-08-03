using NUnit.Framework;
using Seri_Lite.JSON.Parsing.Exceptions;
using Seri_Lite.JSON.Parsing.Models;
using System.Linq;

namespace Seri_Lite_Unit_Tests.JSON.Parsing.Models
{
    [TestFixture]
    public class JsonObjectTests
    {
        private JsonObject _jsonObject;

        [SetUp]
        public void SetUp()
        {
            _jsonObject = new JsonObject();
        }

        [Test]
        public void IsObject_ReturnsTrue()
        {
            Assert.IsTrue(_jsonObject.IsObject);
        }

        [Test]
        public void IsArray_ReturnsFalse()
        {
            Assert.IsFalse(_jsonObject.IsArray);
        }

        [Test]
        public void IsPrimitive_ReturnsFalse()
        {
            Assert.IsFalse(_jsonObject.IsPrimitive);
        }

        [Test]
        public void AsObject_ReturnsJsonObject()
        {
            var jsonObject = _jsonObject.AsObject();

            Assert.IsNotNull(jsonObject);
        }

        [Test]
        public void AsArray_ReturnsNull()
        {
            var jsonArray = _jsonObject.AsArray();

            Assert.IsNull(jsonArray);
        }

        [Test]
        public void AsPrimitive_ReturnsNull()
        {
            var jsonPrimitive = _jsonObject.AsPrimitive();

            Assert.IsNull(jsonPrimitive);
        }

        [Test]
        public void Parent_HasNoParent_ReturnsNull()
        {
            Assert.IsNull(_jsonObject.Parent);
        }

        [Test]
        public void Parent_HasParent_ReturnsParent()
        {
            var parent = new JsonObject();
            var child = new JsonObject(parent);

            Assert.AreEqual(parent, child.Parent);
        }

        [Test]
        public void Root_HasNoParent_ReturnsItself()
        {
            Assert.AreEqual(_jsonObject, _jsonObject.Root);
        }

        [Test]
        public void Root_HasParent_ReturnsRoot()
        {
            var root = new JsonObject();
            var parent = new JsonObject(root);
            var child = new JsonObject(parent);

            Assert.AreEqual(root, child.Root);
        }

        [Test]
        public void Constructor_WithoutParameters_ObjectIsEmptyWithNoParent()
        {
            Assert.AreEqual(null, _jsonObject.Parent);
            Assert.AreEqual(0, _jsonObject.GetProperties().Count());
        }

        [Test]
        public void Constructor_WithParent_ObjectIsEmptyWithParent()
        {
            var parent = new JsonObject();

            _jsonObject = new JsonObject(parent);

            Assert.AreEqual(parent, _jsonObject.Parent);
            Assert.AreEqual(0, _jsonObject.GetProperties().Count());
        }

        [Test]
        public void Constructor_WithProperties_ObjectContainsPropertiesAndHasNoParent()
        {
            var properties = new JsonProperty[]
            {
                new JsonProperty("Name", new JsonObject()),
                new JsonProperty("Surname", new JsonObject()),
            };

            _jsonObject = new JsonObject(properties);

            Assert.AreEqual(null, _jsonObject.Parent);
            Assert.AreEqual(properties.Length, _jsonObject.GetProperties().Count());
        }

        [Test]
        public void Constructor_WithParentAndProperties_ObjectContainsPropertiesAndHasParent()
        {
            var parent = new JsonObject();
            var properties = new JsonProperty[]
            {
                new JsonProperty("Name", new JsonObject()),
                new JsonProperty("Surname", new JsonObject()),
            };

            _jsonObject = new JsonObject(parent, properties);

            Assert.AreEqual(parent, _jsonObject.Parent);
            Assert.AreEqual(properties.Length, _jsonObject.GetProperties().Count());
        }

        [Test]
        public void AddProperty_IsUnique_AddsProperty()
        {
            var property = new JsonProperty("Name", new JsonObject());

            _jsonObject.AddProperty(property);

            Assert.AreEqual(1, _jsonObject.GetProperties().Count());
            Assert.AreEqual(property, _jsonObject.GetProperties().ElementAt(0));
        }

        [Test]
        public void AddProperty_IsNotUnique_ThrowsPropertyAlreadyExistsException()
        {
            var property = new JsonProperty("Name", new JsonObject());
            _jsonObject.AddProperty(property);

            void act() => _jsonObject.AddProperty(property);

            Assert.Throws<PropertyAlreadyExistsException>(act);
        }

        [Test]
        public void AddProperties_IsUnique_AddsProperties()
        {
            var properties = new JsonProperty[]
            {
                new JsonProperty("Name", new JsonObject()),
                new JsonProperty("Surname", new JsonObject()),
            };

            _jsonObject.AddProperties(properties);

            CollectionAssert.AreEqual(properties, _jsonObject.GetProperties());
        }

        [Test]
        public void AddProperties_IsNotUnique_ThrowsPropertyAlreadyExistsException()
        {
            var properties = new JsonProperty[]
            {
                new JsonProperty("Name", new JsonObject()),
                new JsonProperty("Name", new JsonObject()),
            };

            void act() => _jsonObject.AddProperties(properties);

            Assert.Throws<PropertyAlreadyExistsException>(act);
        }

        [Test]
        public void CheckIfPropertyExists_Exists_ReturnsTrue()
        {
            var property = new JsonProperty("Name", new JsonObject());

            _jsonObject.AddProperty(property);

            Assert.IsTrue(_jsonObject.CheckIfPropertyExists("Name"));
        }

        [Test]
        public void CheckIfPropertyExists_DoesNotExist_ReturnsFalse()
        {
            Assert.IsFalse(_jsonObject.CheckIfPropertyExists("Name"));
        }

        [Test]
        public void GetProperty_Exists_ReturnsProperty()
        {
            var property = new JsonProperty("Name", new JsonObject());

            _jsonObject.AddProperty(property);

            Assert.AreEqual(property, _jsonObject.GetProperty("Name"));
        }

        [Test]
        public void GetProperty_DoesNotExist_ReturnsNull()
        {
            Assert.IsNull(_jsonObject.GetProperty("Name"));
        }

        [Test]
        public void GetToken_Exists_ReturnsJsonToken()
        {
            var token = new JsonObject();
            var property = new JsonProperty("Name", token);

            _jsonObject.AddProperty(property);

            Assert.AreEqual(token, _jsonObject.GetToken("Name"));
        }

        [Test]
        public void GetToken_DoesNotExist_ReturnsNull()
        {
            Assert.IsNull(_jsonObject.GetToken("Name"));
        }

        [Test]
        public void GetObject_ExistsAndIsAnObject_ReturnsJsonObject()
        {
            var token = new JsonObject();
            var property = new JsonProperty("Name", token);

            _jsonObject.AddProperty(property);

            Assert.AreEqual(token, _jsonObject.GetObject("Name"));
        }

        [Test]
        public void GetObject_ExistsAndIsAnArray_ReturnsNull()
        {
            var token = new JsonArray();
            var property = new JsonProperty("Name", token);

            _jsonObject.AddProperty(property);

            Assert.IsNull(_jsonObject.GetObject("Name"));
        }

        [Test]
        public void GetObject_ExistsAndIsAPrimitive_ReturnsNull()
        {
            var token = new JsonPrimitive();
            var property = new JsonProperty("Name", token);

            _jsonObject.AddProperty(property);

            Assert.IsNull(_jsonObject.GetObject("Name"));
        }

        [Test]
        public void GetArray_ExistsAndIsAnObject_ReturnsNull()
        {
            var token = new JsonObject();
            var property = new JsonProperty("Name", token);

            _jsonObject.AddProperty(property);

            Assert.IsNull(_jsonObject.GetArray("Name"));
        }

        [Test]
        public void GetArray_ExistsAndIsAnArray_ReturnsJsonArray()
        {
            var token = new JsonArray();
            var property = new JsonProperty("Name", token);

            _jsonObject.AddProperty(property);

            Assert.AreEqual(token, _jsonObject.GetArray("Name"));
        }

        [Test]
        public void GetArray_ExistsAndIsAPrimitive_ReturnsNull()
        {
            var token = new JsonPrimitive();
            var property = new JsonProperty("Name", token);

            _jsonObject.AddProperty(property);

            Assert.IsNull(_jsonObject.GetArray("Name"));
        }

        [Test]
        public void GetPrimitive_ExistsAndIsAnObject_ReturnsNull()
        {
            var token = new JsonObject();
            var property = new JsonProperty("Name", token);

            _jsonObject.AddProperty(property);

            Assert.IsNull(_jsonObject.GetPrimitive("Name"));
        }

        [Test]
        public void GetPrimitive_ExistsAndIsAnArray_ReturnsNull()
        {
            var token = new JsonArray();
            var property = new JsonProperty("Name", token);

            _jsonObject.AddProperty(property);

            Assert.IsNull(_jsonObject.GetPrimitive("Name"));
        }

        [Test]
        public void GetPrimitive_ExistsAndIsAPrimitive_ReturnsJsonPrimitive()
        {
            var token = new JsonPrimitive();
            var property = new JsonProperty("Name", token);

            _jsonObject.AddProperty(property);

            Assert.AreEqual(token, _jsonObject.GetPrimitive("Name"));
        }
    }
}
