using NUnit.Framework;
using Seri_Lite.JSON;
using Seri_Lite.JSON.Parsing.Models;
using Seri_Lite.JSON.Serialization.Property;
using Seri_Lite_Unit_Tests.JSON.Models;
using System;
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
            _serializer = new JsonSerializerBuilder().Build();
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
            var person = new AdvancePerson
            {
                Id = Guid.Parse("17B1019F-AF9E-43AF-8C71-484274B9CC49"),
                BirthDate = new DateTime(1996, 12, 21),
                Name = "Howard",
                Age = 18,
                Height = 185.5,
                IsMarried = false,
                Partner = new SimplePerson { Name = "Sara" },
            };
            var serialized = _serializer.Serialize(person);

            var result = _serializer.Deserialize<AdvancePerson>(serialized);

            Assert.AreEqual(person, result);
        }

        [Test]
        public void Deserialize_ObjectToObjectType_ReturnsToken()
        {
            var person = new AdvancePerson
            {
                Id = Guid.Parse("17B1019F-AF9E-43AF-8C71-484274B9CC49"),
                BirthDate = new DateTime(1996, 12, 21),
                Name = "Howard",
                Age = 18,
                Height = 185.5,
                IsMarried = false,
                Partner = new SimplePerson { Name = "Sara" },
            };
            var serialized = _serializer.Serialize(person);

            var result = _serializer.Deserialize<object>(serialized);

            Assert.That(result, Is.TypeOf<JsonObject>());
        }

        [Test]
        public void Deserialize_CollectionToObjectType_ReturnsToken()
        {
            var people = new List<string> { "Chad", "Phill", "Petra" };
            var serialized = _serializer.Serialize(people);

            var result = _serializer.Deserialize<object>(serialized);

            Assert.That(result, Is.TypeOf<JsonArray>());
        }

        [Test]
        public void Deserialize_LowerCaseProperties_ReturnsObject()
        {
            var person = new AdvancePerson
            {
                Id = Guid.Parse("17B1019F-AF9E-43AF-8C71-484274B9CC49"),
                BirthDate = new DateTime(1996, 12, 21),
                Name = "Howard",
                Age = 18,
                Height = 185.5,
                IsMarried = false,
                Partner = new SimplePerson { Name = "Sara" },
            };
            _serializer = new JsonSerializerBuilder()
                .SetPropertyNameResolver(new CamelCasePropertyNameResolver())
                .Build();
            var serialized = _serializer.Serialize(person);

            var result = _serializer.Deserialize<AdvancePerson>(serialized);

            Assert.AreEqual(person, result);
        }

        [TestCaseSource(typeof(DeserializationPrimitiveAsObjectSource))]
        public void Deserialize_PrimitiveToObjectType_ReturnsPrimitive(object value)
        {
            var serialized = _serializer.Serialize(value);

            var result = _serializer.Deserialize<object>(serialized);

            Assert.AreEqual(value, result);
        }

        class DeserializationPrimitiveAsObjectSource : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return 1;
                yield return 1.1;
                yield return true;
                yield return "true";
                yield return null;
            }
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
