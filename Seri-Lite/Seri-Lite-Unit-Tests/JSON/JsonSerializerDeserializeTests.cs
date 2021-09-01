using NUnit.Framework;
using Seri_Lite.JSON;
using Seri_Lite_Unit_Tests.JSON.Models;
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

        [Test]
        public void Deserialize_ListOfStrings_ReturnsArray()
        {
            var people = new List<string> { "Chad", "Phill", "Petra" };
            var serialized = _serializer.Serialize(people);

            var result = _serializer.Deserialize<List<string>>(serialized);

            CollectionAssert.AreEqual(people, result);
        }

        [Test]
        public void Deserialize_ListOfDoubles_ReturnsArray()
        {
            var numbers = new List<double> { -1, -0.5, 0.0, 1.55, 123.123456789 };
            var serialized = _serializer.Serialize(numbers);

            var result = _serializer.Deserialize<List<double>>(serialized);

            CollectionAssert.AreEqual(numbers, result);
        }

        [Test]
        public void Deserialize_ListOfObjects_ReturnsArray()
        {
            var people = new List<SimplePerson>
            {
                new SimplePerson() { Name="Chad" },
                new SimplePerson() { Name="Phill" },
                new SimplePerson() { Name="Petra" },
            };
            var serialized = _serializer.Serialize(people);

            var result = _serializer.Deserialize<List<SimplePerson>>(serialized);

            CollectionAssert.AreEqual(people, result);
        }

        [Test]
        public void Deserialize_ArrayOfStrings_ReturnsArray()
        {
            var people = new string[] { "Chad", "Phill", "Petra" };
            var serialized = _serializer.Serialize(people);

            var result = _serializer.Deserialize<string[]>(serialized);

            CollectionAssert.AreEqual(people, result);
        }

        [Test]
        public void Deserialize_ArrayOfDoubles_ReturnsArray()
        {
            var numbers = new double[] { -1, -0.5, 0.0, 1.55, 123.123456789 };
            var serialized = _serializer.Serialize(numbers);

            var result = _serializer.Deserialize<double[]>(serialized);

            CollectionAssert.AreEqual(numbers, result);
        }

        [Test]
        public void Deserialize_ArrayOfObjects_ReturnsArray()
        {
            var people = new SimplePerson[]
            {
                new SimplePerson() { Name="Chad" },
                new SimplePerson() { Name="Phill" },
                new SimplePerson() { Name="Petra" },
            };
            var serialized = _serializer.Serialize(people);

            var result = _serializer.Deserialize<SimplePerson[]>(serialized);

            CollectionAssert.AreEqual(people, result);
        }
    }
}
