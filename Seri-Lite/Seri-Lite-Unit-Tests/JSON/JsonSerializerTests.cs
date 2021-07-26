using NUnit.Framework;
using Seri_Lite.JSON;
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

        // TODO: rename + move
        class X : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return ((object)new { }, "{}");
                yield return ((object)new { Name = "Test" }, "{\"Name\":\"Test\"}");
                yield return ((object)new { Person = new { Name = "Test1", Surname = "Test2" } }, "{\"Person\":{\"Name\":\"Test1\",\"Surname\":\"Test2\"}}");
                yield return ((object)new { Names = new string[] { "Test1", "Test2" } }, "{\"Names\":[\"Test1\",\"Test2\"]}");
                yield return ((object)new string[] { "Test1", "Test2" }, "[\"Test1\",\"Test2\"]");
                yield return ((object)new
                {
                    Person = new
                    {
                        Name = "Test1",
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
                }, "{\"Person\":{\"Name\":\"Test1\",\"Address\":{\"City\":\"Test2\",\"Street\":\"Test3\"},\"Pets\":[{\"Name\":\"Test4\",\"Species\":\"Test5\"},{\"Name\":\"Test6\",\"Species\":\"Test7\"}]}}");
            }
        }

        [TestCaseSource(typeof(X))]
        public void Y((object, string) z)
        {
            var result = _serializer.Serialize(z.Item1);

            Assert.AreEqual(z.Item2, result);
        }
    }
}
