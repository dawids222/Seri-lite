﻿using Moq;
using NUnit.Framework;
using Seri_Lite.JSON;
using Seri_Lite.JSON.Enums;
using Seri_Lite.JSON.Parsing.Readers;
using Seri_Lite.JSON.Serialization.Property;
using Seri_Lite_Unit_Tests.JSON.Enums;
using Seri_Lite_Unit_Tests.JSON.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Seri_Lite_Unit_Tests.JSON
{
    [TestFixture]
    public class JsonConverterSerializeTests
    {
        private Mock<IJsonReader> _jsonReaderMock;

        private JsonConverter _converter;

        [SetUp]
        public void SetUp()
        {
            _jsonReaderMock = new Mock<IJsonReader>();

            _converter = new JsonConverterBuilder().Build();
        }

        [TestCaseSource(typeof(SerializationObjectSource))]
        public void Serialize_Standard_ReturnsSameValueAsNewtonsoftJsonConvert(object value)
        {
            var expected = Newtonsoft.Json.JsonConvert.SerializeObject(value);

            var result = _converter.Serialize(value);

            Assert.AreEqual(expected, result);
        }

        [TestCaseSource(typeof(SerializationObjectSource))]
        public void Serialize_PropertyNameCamelCase_ReturnsSameValueAsNewtonsoftJsonConvert(object value)
        {
            var propertyNameResolver = new CamelCasePropertyNameResolver();
            _converter = new JsonConverter(NullPropertyBehaviour.SERIALIZE, _jsonReaderMock.Object, propertyNameResolver);
            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy(),
                },
            };
            var expected = Newtonsoft.Json.JsonConvert.SerializeObject(value, settings);

            var result = _converter.Serialize(value);

            Assert.AreEqual(expected, result);
        }

        [TestCaseSource(typeof(SerializationObjectSource))]
        public void Serialize_NullValueIgnore_ReturnsSameValueAsNewtonsoftJsonConvert(object value)
        {
            var propertyNameResolver = new InheritCasePropertyNameResolver();
            _converter = new JsonConverter(NullPropertyBehaviour.IGNORE, _jsonReaderMock.Object, propertyNameResolver);
            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            };
            var expected = Newtonsoft.Json.JsonConvert.SerializeObject(value, settings);

            var result = _converter.Serialize(value);

            Assert.AreEqual(expected, result);
        }

        [TestCaseSource(typeof(SerializationObjectSource))]
        public void Serialize_ExecutesFasterThanNewtonsoftJsonConvert(object value)
        {
            var maxExecutionTime = MeasureExecutionTime(() => Newtonsoft.Json.JsonConvert.SerializeObject(value));

            var actualExecutionTime = MeasureExecutionTime(() => _converter.Serialize(value));

            Assert.That(actualExecutionTime, Is.LessThan(maxExecutionTime));
        }

        [TestCaseSource(typeof(SerializationObjectSource))]
        public void Serialize_ExecutesFasterThanMicrosoftJsonSerializer(object value)
        {
            var maxExecutionTime = MeasureExecutionTime(() => System.Text.Json.JsonSerializer.Serialize(value));

            var actualExecutionTime = MeasureExecutionTime(() => _converter.Serialize(value));

            Assert.That(actualExecutionTime, Is.LessThan(maxExecutionTime));
        }

        [Test]
        public void Constructor_NullPropertyNameResolver_Throws()
        {
            void act() => new JsonConverter(NullPropertyBehaviour.SERIALIZE, _jsonReaderMock.Object, null);

            Assert.Throws<ArgumentNullException>(act);
        }

        private static double MeasureExecutionTime(Action action)
        {
            var start = DateTime.Now;
            action.Invoke();
            var end = DateTime.Now;
            var timeSpan = end - start;
            return timeSpan.TotalMilliseconds;
        }

        class SerializationObjectSource : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new { };
                yield return Array.Empty<object>();
                yield return null;
                yield return 1;
                yield return 1.5;
                yield return true;
                yield return "Test";
                yield return "va\"lue";
                yield return "va\"\"lue";
                yield return "va\\\"lue";
                yield return SimpleEnum.VALUE_1;
                yield return new DateTime(2021, 9, 2);
                yield return Guid.Parse("a90194f9-48c6-4a74-9d89-fd976cf1d93f");
                yield return new { Name = "Test" };
                yield return new { Person = new { Name = "Test1", Surname = "Test2" } };
                yield return new { Names = new string[] { "Test1", "Test2" } };
                yield return new int[] { 1, 2, 2 };
                yield return new { Array = new int[] { 1, 2, 2 } };
                yield return new bool[] { false, true, true };
                yield return new { Array = new bool[] { false, true, true } };
                yield return new string[] { "Test1", "Test2", "Test2" };
                yield return new { Array = new string[] { "Test1", "Test2", "Test2" } };
                yield return new { List = new List<string> { "Test1", "Test2", "Test2" } };
                yield return new { HashSet = new HashSet<string> { "Test1", "Test2", "Test2" } };
                yield return new { ArrayList = new ArrayList { "Test1", "Test2", "Test2" } };
                yield return new { ArrayList = new ArrayList { "Test", 1, 1.1, false } };
                yield return new { Array = new SimpleEnum[] { SimpleEnum.VALUE_1, SimpleEnum.VALUE_2 } };
                yield return new { Dictionary = new Dictionary<string, string> { { "id", "1234" }, { "name", "Bob" } } };
                yield return new Dictionary<string, string> { { "id", "1234" }, { "name", "Bob" } };
                yield return new Dictionary<string, int> { { "id", 1 }, { "age", 25 } };
                yield return new Dictionary<string, SimplePerson> { { "id", new SimplePerson { Name = "Juan" } }, { "age", new() } };
                yield return new object[] { null, null, null };
                yield return new { Array = new object[] { null, null, null } };
                yield return new string[][] { new string[] { "Test1", "Test2" }, new string[] { "Test3", "Test4" } };
                yield return new object[] { "Test", 1, 1.55, true };
                yield return new { Person = (object)null };
                yield return new { Name = (string)null };
                yield return new { People = (ICollection)null };
                yield return new { Name = "Test", Value = SimpleEnum.VALUE_1 };
                yield return new object[] { new { Name = "Test1", Surname = "Test2" }, new { Name = "Test3", Surname = "Test4" } };
                yield return new { Person = new { Id = Guid.Parse("a90194f9-48c6-4a74-9d89-fd976cf1d93f"), BirthDate = new DateTime(2021, 9, 2), Name = "Test1", Age = 18, Height = 180.5, Married = false, Address = new { City = "Test2", Street = "Test3" }, Pets = new object[] { new { Name = "Test4", Species = "Test5" }, new { Name = "Test6", Species = "Test7" }, }, }, };
            }
        }
    }
}
