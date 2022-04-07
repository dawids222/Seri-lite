using NUnit.Framework;
using Seri_Lite.JSON;
using Seri_Lite.JSON.Parsing.Models;
using Seri_Lite.JSON.Serialization.Property;
using Seri_Lite_Unit_Tests.JSON.Enums;
using Seri_Lite_Unit_Tests.JSON.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Seri_Lite_Unit_Tests.JSON
{
    [TestFixture]
    public class JsonSerializerDeserializeGenericTests : JsonSerializerDeserializeTestsBase
    {
        protected override T Deserialize<T>(string value) => _converter.Deserialize<T>(value);
    }

    [TestFixture]
    public class JsonSerializerDeserializeTypeTests : JsonSerializerDeserializeTestsBase
    {
        protected override T Deserialize<T>(string value) => (T)_converter.Deserialize(typeof(T), value);
    }

    public abstract class JsonSerializerDeserializeTestsBase
    {
        protected JsonConverter _converter;

        protected abstract T Deserialize<T>(string value);

        [SetUp]
        public void SetUp()
        {
            _converter = new JsonConverterBuilder().Build();
        }

        [Test]
        public void Deserialize_JsonObjectWithSingleString_ReturnsObject()
        {
            var person = new SimplePerson { Name = "Howard" };
            var serialized = _converter.Serialize(person);

            var result = Deserialize<SimplePerson>(serialized);

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
            var serialized = _converter.Serialize(person);

            var result = Deserialize<IntermediatePerson>(serialized);

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
                Salary = 2500.99f,
                IsMarried = false,
                Partner = new SimplePerson { Name = "Sara" },
            };
            var serialized = _converter.Serialize(person);

            var result = Deserialize<AdvancePerson>(serialized);

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
            var serialized = _converter.Serialize(person);

            var result = Deserialize<object>(serialized);

            Assert.That(result, Is.TypeOf<JsonObject>());
        }

        [Test]
        public void Deserialize_CollectionToObjectType_ReturnsToken()
        {
            var people = new List<string> { "Chad", "Phill", "Petra" };
            var serialized = _converter.Serialize(people);

            var result = Deserialize<object>(serialized);

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
            _converter = new JsonConverterBuilder()
                .SetPropertyNameResolver(new CamelCasePropertyNameResolver())
                .Build();
            var serialized = _converter.Serialize(person);

            var result = Deserialize<AdvancePerson>(serialized);

            Assert.AreEqual(person, result);
        }

        [Test]
        public void Deserialize_DictionaryStringString_ReturnsDictionary()
        {
            var dictionary = new Dictionary<string, string> { { "id", "1234" }, { "age", "25" } };
            var serialized = _converter.Serialize(dictionary);

            var result = Deserialize<Dictionary<string, string>>(serialized);

            CollectionAssert.AreEqual(dictionary, result);
        }

        [Test]
        public void Deserialize_DictionaryIntSimplePerson_ReturnsDictionary()
        {
            var dictionary = new Dictionary<int, SimplePerson> {
                { 1, new SimplePerson{ Name = "Guss" } },
                { 2, new() }
            };
            var serialized = _converter.Serialize(dictionary);

            var result = Deserialize<Dictionary<int, SimplePerson>>(serialized);

            CollectionAssert.AreEqual(dictionary, result);
        }

        [TestCaseSource(typeof(DeserializationPrimitiveAsObjectSource))]
        public void Deserialize_PrimitiveToObjectType_ReturnsPrimitive(object value)
        {
            var serialized = _converter.Serialize(value);

            var result = Deserialize<object>(serialized);

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
            var serialized = _converter.Serialize(collection);

            var result = Deserialize<T>(serialized);

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

        [Test]
        public void Deserialize_EnumValue_ReturnsEnum()
        {
            var value = SimpleEnum.VALUE_1;
            var serialized = _converter.Serialize(value);

            var result = _converter.Deserialize<SimpleEnum>(serialized);

            Assert.AreEqual(value, result);
        }

        [Test]
        public void Deserialize_ListOfEnums_ReturnsListOfEnums()
        {
            var value = new List<SimpleEnum> { SimpleEnum.VALUE_1, SimpleEnum.VALUE_2 };
            var serialized = _converter.Serialize(value);

            var result = _converter.Deserialize<List<SimpleEnum>>(serialized);

            CollectionAssert.AreEqual(value, result);
        }

        [Test]
        public void Deserialize_ArrayOfEnums_ReturnsArrayOfEnums()
        {
            var value = new SimpleEnum[] { SimpleEnum.VALUE_1, SimpleEnum.VALUE_2 };
            var serialized = _converter.Serialize(value);

            var result = _converter.Deserialize<SimpleEnum[]>(serialized);

            CollectionAssert.AreEqual(value, result);
        }

        [Test]
        public void Deserialize_ObjectWithEnumValue_ReturnsObject()
        {
            var value = new EnumObject();
            var serialized = _converter.Serialize(value);

            var result = _converter.Deserialize<EnumObject>(serialized);

            Assert.AreEqual(value, result);
        }

        private record EnumObject
        {
            public SimpleEnum Value { get; set; }
        }
    }
}
