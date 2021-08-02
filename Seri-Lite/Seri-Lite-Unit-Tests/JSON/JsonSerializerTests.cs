﻿using NUnit.Framework;
using Seri_Lite.JSON;
using Seri_Lite.JSON.Enums;
using Seri_Lite.JSON.Serialization.Property;
using System;
using System.Collections;

namespace Seri_Lite_Unit_Tests.JSON
{
    [TestFixture]
    public class JsonSerializerTests
    {
        private JsonSerializer _serializer;

        [SetUp]
        public void SetUp()
        {
            _serializer = new JsonSerializer();
        }

        [TestCaseSource(typeof(SerializationObjectSource))]
        public void Serialize_Standard_ReturnsSameValueAsNewtonsoftJsonConvert(object value)
        {
            var expected = Newtonsoft.Json.JsonConvert.SerializeObject(value);

            var result = _serializer.Serialize(value);

            Assert.AreEqual(expected, result);
        }

        [TestCaseSource(typeof(SerializationObjectSource))]
        public void Serialize_PropertyNameCamelCase_ReturnsSameValueAsNewtonsoftJsonConvert(object value)
        {
            var propertyNameResolver = new CamelCasePropertyNameResolver();
            _serializer = new JsonSerializer(NullPropertyBehaviour.SERIALIZE, propertyNameResolver);
            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy(),
                },
            };
            var expected = Newtonsoft.Json.JsonConvert.SerializeObject(value, settings);

            var result = _serializer.Serialize(value);

            Assert.AreEqual(expected, result);
        }

        [TestCaseSource(typeof(SerializationObjectSource))]
        public void Serialize_NullValueIgnore_ReturnsSameValueAsNewtonsoftJsonConvert(object value)
        {
            var propertyNameResolver = new InheritCasePropertyNameResolver();
            _serializer = new JsonSerializer(NullPropertyBehaviour.IGNORE, propertyNameResolver);
            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            };
            var expected = Newtonsoft.Json.JsonConvert.SerializeObject(value, settings);

            var result = _serializer.Serialize(value);

            Assert.AreEqual(expected, result);
        }

        [TestCaseSource(typeof(SerializationObjectSource))]
        public void Serialize_ExecutesFasterThanNewtonsoftJsonConvert(object value)
        {
            var maxExecutionTime = MeasureExecutionTime(() => Newtonsoft.Json.JsonConvert.SerializeObject(value));

            var actualExecutionTime = MeasureExecutionTime(() => _serializer.Serialize(value));

            Assert.That(actualExecutionTime, Is.LessThan(maxExecutionTime));
        }

        [Test]
        public void Constructor_NullPropertyNameResolver_Throws()
        {
            static void act() => new JsonSerializer(NullPropertyBehaviour.SERIALIZE, null);

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
                yield return new { Name = "Test" };
                yield return new { Person = new { Name = "Test1", Surname = "Test2" } };
                yield return new { Names = new string[] { "Test1", "Test2" } };
                yield return new int[] { 1, 2, 2 };
                yield return new { Array = new int[] { 1, 2, 2 } };
                yield return new bool[] { false, true, true };
                yield return new { Array = new bool[] { false, true, true } };
                yield return new string[] { "Test1", "Test2", "Test2" };
                yield return new { Array = new string[] { "Test1", "Test2", "Test2" } };
                yield return new object[] { null, null, null };
                yield return new { Array = new object[] { null, null, null } };
                yield return new string[][] { new string[] { "Test1", "Test2" }, new string[] { "Test3", "Test4" } };
                yield return new object[] { "Test", 1, 1.55, true };
                yield return new { Person = (object)null };
                yield return new object[] { new { Name = "Test1", Surname = "Test2" }, new { Name = "Test3", Surname = "Test4" } };
                yield return new { Person = new { Name = "Test1", Age = 18, Height = 180.5, Married = false, Address = new { City = "Test2", Street = "Test3" }, Pets = new object[] { new { Name = "Test4", Species = "Test5" }, new { Name = "Test6", Species = "Test7" }, }, }, };
            }
        }
    }
}
