using NUnit.Framework;
using Seri_Lite.JSON;
using System;
using System.Collections;
using NewtonsoftJsonConvert = Newtonsoft.Json.JsonConvert;

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
        public void Serialize_ReturnsSameValueAsNewtonsoftJsonConvert(object value)
        {
            var expected = NewtonsoftJsonConvert.SerializeObject(value);

            var result = _serializer.Serialize(value);

            Assert.AreEqual(expected, result);
        }

        [TestCaseSource(typeof(SerializationObjectSource))]
        public void Serialize_ExecutesFasterThanNewtonsoftJsonConvert(object value)
        {
            var maxExecutionTime = MeasureExecutionTime(() => NewtonsoftJsonConvert.SerializeObject(value));

            var actualExecutionTime = MeasureExecutionTime(() => _serializer.Serialize(value));

            Assert.That(actualExecutionTime, Is.LessThan(maxExecutionTime));
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
                yield return (object)new { };
                yield return (object)1;
                yield return (object)true;
                yield return (object)"Test";
                yield return (object)new { Name = "Test" };
                yield return (object)new { Person = new { Name = "Test1", Surname = "Test2" } };
                yield return (object)new { Names = new string[] { "Test1", "Test2" } };
                yield return (object)new string[] { "Test1", "Test2" };
                yield return (object)new
                {
                    Person = new
                    {
                        Name = "Test1",
                        Age = 18,
                        Height = 180.5,
                        Married = false,
                        Address = new
                        {
                            City = "Test2",
                            Street = "Test3"
                        },
                        Pets = new object[]
                        {
                            new
                            {
                                Name="Test4",
                                Species="Test5"
                            },
                            new
                            {
                                Name="Test6",
                                Species="Test7"
                            },
                        },
                    },
                };
            }
        }
    }
}
