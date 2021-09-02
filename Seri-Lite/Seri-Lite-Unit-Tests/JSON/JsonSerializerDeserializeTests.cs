using NUnit.Framework;
using Seri_Lite.JSON;
using Seri_Lite_Unit_Tests.JSON.Models;
using System.Collections;
using System.Collections.Generic;

namespace Seri_Lite_Unit_Tests.JSON
{
    [TestFixture]
    public class JsonSerializerDeserializeTests
    {
        // TODO: add tests for parsing FLOAT

        private JsonSerializer _serializer;

        [SetUp]
        public void SetUp()
        {
            _serializer = new JsonSerializer();
        }

        [Test]
        public void Deserialize_JsonObjectWithSingleString_ReturnsObject()
        {
            var person = new SimplePerson { Name = "Howard" };
            var serialized = _serializer.Serialize(person);

            var result = _serializer.Deserialize<SimplePerson>(serialized);

            Assert.AreEqual(person, result);
        }

        [Test]
        public void Deserialize_JsonObjectWithMultipleValues_ReturnsObject()
        {
            var person = new IntermediatePerson
            {
                Name = "Howard",
                Age = 18,
                Height = 185.5,
                IsMarried = false,
                Partner = null,
            };
            var serialized = _serializer.Serialize(person);

            var result = _serializer.Deserialize<IntermediatePerson>(serialized);

            Assert.AreEqual(person, result);
        }

        [Test]
        public void Deserialize_JsonObjectWithNestedObject_ReturnsObject()
        {
            var person = new IntermediatePerson
            {
                Name = "Howard",
                Age = 18,
                Height = 185.5,
                IsMarried = false,
                Partner = new SimplePerson { Name = "Sara" },
            };
            var serialized = _serializer.Serialize(person);

            var result = _serializer.Deserialize<IntermediatePerson>(serialized);

            Assert.AreEqual(person, result);
        }

        [TestCaseSource(typeof(DeserializationCollectionSource))]
        public void Deserialize_Collection_ReturnsCollection<T>(T collection) where T : IEnumerable
        {
            var serialized = _serializer.Serialize(collection);

            var result = _serializer.Deserialize<T>(serialized);

            CollectionAssert.AreEqual(collection, result);
        }

        class DeserializationCollectionSource : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new List<string> { "Chad", "Phill", "Petra" };
                yield return new List<double> { -1, -0.5, 0.0, 1.55, 123.123456789 };
                yield return new List<SimplePerson>
                {
                    new SimplePerson() { Name="Chad" },
                    new SimplePerson() { Name="Phill" },
                    new SimplePerson() { Name="Petra" },
                };
                yield return new string[] { "Chad", "Phill", "Petra" };
                yield return new double[] { -1, -0.5, 0.0, 1.55, 123.123456789 };
                yield return new SimplePerson[]
                {
                    new SimplePerson() { Name="Chad" },
                    new SimplePerson() { Name="Phill" },
                    new SimplePerson() { Name="Petra" },
                };
                yield return new HashSet<string> { "Chad", "Phill", "Petra" };
                yield return new HashSet<double> { -1, -0.5, 0.0, 1.55, 123.123456789 };
                yield return new HashSet<SimplePerson>
                {
                    new SimplePerson() { Name="Chad" },
                    new SimplePerson() { Name="Phill" },
                    new SimplePerson() { Name="Petra" },
                };
            }
        }
    }
}
