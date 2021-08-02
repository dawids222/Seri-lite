using NUnit.Framework;
using Seri_Lite.JSON.Parsing.Models;

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
        public void AsObject_ReturnsObject()
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
    }
}
