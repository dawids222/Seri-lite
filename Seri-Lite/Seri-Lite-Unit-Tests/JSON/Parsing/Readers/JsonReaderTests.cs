using NUnit.Framework;
using Seri_Lite.JSON;
using Seri_Lite.JSON.Parsing.Exceptions;
using Seri_Lite.JSON.Parsing.Readers;
using System.Collections;

namespace Seri_Lite_Unit_Tests.JSON.Parsing.Readers
{
    [TestFixture]
    public class JsonReaderTests
    {
        private JsonReader _reader;
        private JsonSerializer _serializer;

        [SetUp]
        public void SetUp()
        {
            _reader = new JsonReader();
            _serializer = new JsonSerializerBuilder().Build();
        }

        [Test]
        public void Read1_SimpleStringRepresentingObject_ReturnsJsonObject()
        {
            var obj = new { Name = "Test" };
            var value = _serializer.Serialize(obj);

            var token = _reader.Read(value);

            Assert.IsTrue(token.IsObject);
            var asObject = token.AsObject();
            Assert.IsTrue(asObject.PropertyExists("Name"));
            Assert.AreEqual(obj.Name, asObject.GetPrimitive("Name")?.AsString());
        }

        [Test]
        public void Read2_SimpleStringRepresentingObject_ReturnsJsonObject()
        {
            var obj = new { Name = "Test", Surname = "Test2" };
            var value = _serializer.Serialize(obj);

            var token = _reader.Read(value);

            Assert.IsTrue(token.IsObject);
            var asObject = token.AsObject();
            Assert.IsTrue(asObject.PropertyExists("Name"));
            Assert.AreEqual(obj.Name, asObject.GetPrimitive("Name")?.AsString());
            Assert.IsTrue(asObject.PropertyExists("Surname"));
            Assert.AreEqual(obj.Surname, asObject.GetPrimitive("Surname")?.AsString());
        }

        [Test]
        public void Read3_SimpleStringRepresentingObject_ReturnsJsonObject()
        {
            var obj = new { Name = "Tom", Height = 185.5, Age = 18, IsMarried = false, Partner = (object)null };
            var value = _serializer.Serialize(obj);

            var token = _reader.Read(value);

            Assert.IsTrue(token.IsObject);
            var asObject = token.AsObject();
            Assert.IsTrue(asObject.PropertyExists("Name"));
            Assert.IsTrue(asObject.GetPrimitive("Name").IsString);
            Assert.AreEqual(obj.Name, asObject.GetPrimitive("Name").AsString());
            Assert.IsTrue(asObject.PropertyExists("Height"));
            Assert.IsTrue(asObject.GetPrimitive("Height").IsDouble);
            Assert.AreEqual(obj.Height, asObject.GetPrimitive("Height").AsDouble());
            Assert.IsTrue(asObject.PropertyExists("Age"));
            Assert.IsTrue(asObject.GetPrimitive("Age").IsInteger);
            Assert.AreEqual(obj.Age, asObject.GetPrimitive("Age").AsInteger());
            Assert.IsTrue(asObject.PropertyExists("IsMarried"));
            Assert.IsTrue(asObject.GetPrimitive("IsMarried").IsBoolean);
            Assert.AreEqual(obj.IsMarried, asObject.GetPrimitive("IsMarried").AsBoolean());
            Assert.IsTrue(asObject.PropertyExists("Partner"));
            Assert.IsTrue(asObject.GetPrimitive("Partner").IsNull);
            Assert.AreEqual(obj.Partner, asObject.GetPrimitive("Partner").AsNull());
        }

        [Test]
        public void Read4_SimpleStringRepresentingObject_ReturnsJsonObject()
        {
            var obj = 5;
            var value = _serializer.Serialize(obj);

            var token = _reader.Read(value);

            Assert.IsTrue(token.IsPrimitive);
            Assert.AreEqual(obj, token.AsPrimitive().AsInteger());
        }

        [Test]
        public void Read5_SimpleStringRepresentingObject_ReturnsJsonObject()
        {
            var obj = new { Person = new { Name = "Ted" } };
            var value = _serializer.Serialize(obj);

            var token = _reader.Read(value);

            Assert.IsTrue(token.IsObject);
            Assert.AreEqual(obj.Person.Name, token.AsObject().GetObject("Person").GetPrimitive("Name").AsString());
        }

        [Test]
        public void Read6_SimpleStringRepresentingObject_ReturnsJsonObject()
        {
            var obj = new { Person = new { Address = new { PostalCode = 123 } } };
            var value = _serializer.Serialize(obj);

            var token = _reader.Read(value);

            Assert.IsTrue(token.IsObject);
            Assert.AreEqual(obj.Person.Address.PostalCode, token.AsObject().GetObject("Person").GetObject("Address").GetPrimitive("PostalCode").AsInteger());
        }

        [Test]
        public void Read7_SimpleStringRepresentingObject_ReturnsJsonObject()
        {
            var obj = new dynamic[] { "1", 1.1, 1, true, null, new { Value = 1 } };
            var value = _serializer.Serialize(obj);

            var token = _reader.Read(value);

            Assert.IsTrue(token.IsArray);
            Assert.AreEqual(obj[0], token.AsArray().GetPrimitive(0).AsString());
            Assert.AreEqual(obj[1], token.AsArray().GetPrimitive(1).AsDouble());
            Assert.AreEqual(obj[2], token.AsArray().GetPrimitive(2).AsInteger());
            Assert.AreEqual(obj[3], token.AsArray().GetPrimitive(3).AsBoolean());
            Assert.AreEqual(obj[4], token.AsArray().GetPrimitive(4).AsNull());
            Assert.AreEqual(obj[5].Value, token.AsArray().GetObject(5).GetPrimitive("Value").AsInteger());
        }

        [Test]
        public void Read8_SimpleStringRepresentingObject_ReturnsJsonObject()
        {
            var obj = new dynamic[] { new dynamic[] { new { Value = 1 } } };
            var value = _serializer.Serialize(obj);

            var token = _reader.Read(value);

            Assert.IsTrue(token.IsArray);
            Assert.AreEqual(obj[0][0].Value, token.AsArray().GetArray(0).GetObject(0).GetPrimitive("Value").AsInteger());
        }

        [Test]
        public void Read9_SimpleStringRepresentingObject_ReturnsJsonObject()
        {
            var obj = new { Value = "Test1 Test2" };
            var value = _serializer.Serialize(obj);

            var token = _reader.Read(value);

            Assert.AreEqual(obj.Value, token.AsObject().GetPrimitive("Value").AsString());
        }

        [Test]
        public void Read10_SimpleStringRepresentingObject_ReturnsJsonObject()
        {
            var obj = new { Child = new { Value = 10 } };
            var value = _serializer.Serialize(obj);

            var token = _reader.Read(value);

            Assert.AreEqual(token, token.AsObject().GetObject("Child").Parent);
        }

        [Test]
        public void Read11_SimpleStringRepresentingObject_ReturnsJsonObject()
        {
            var obj = new object[] { new { Value = 10 } };
            var value = _serializer.Serialize(obj);

            var token = _reader.Read(value);

            Assert.AreEqual(token, token.AsArray().GetObject(0).Parent);
        }

        [Test]
        public void Read12_SimpleStringRepresentingObject_ReturnsJsonObject()
        {
            var obj = @"va\""lue";
            var value = _serializer.Serialize(obj);

            var token = _reader.Read(value);

            Assert.AreEqual(obj, token.AsPrimitive().AsString());
        }

        [TestCaseSource(typeof(InvalidJsonSource))]
        public void Read_InvalidValue_Throws(string value)
        {
            void act() => _reader.Read(value);

            Assert.Throws<JsonReadingException>(act);
        }

        class InvalidJsonSource : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return "'John'"; // Wrong quotes
                yield return "\"John says \"Hello!\"\" - \""; // Inside the string must use escape sequence \"
                yield return "[\"Hello\", 3.14, true, ]"; // Extra comma (,) in array
                yield return "[\"Hello\", 3.14, , true]"; // Extra comma (,) in array
                yield return "[\"Hello\", 3.14, true}"; // Closing bracket is wrong
                yield return "[\"Hello\", 3.14, true, \"name\": \"Joe\"]"; // Name value pair not allowed in array              
                yield return "{\"name\": \"Joe\", \"age\": null, }"; // Extra comma (,) in object
                yield return "{\"name\": \"Joe\", , \"age\": null}"; // Extra comma (,) in object
                yield return "{\"name\": \"Joe\", \"age\": null]"; // Closing bracket is wrong
                yield return "{\"name\": \"Joe\", \"age\": }"; // Missing value in name value pair in object
                yield return "{\"name\": \"Joe\", \"age\" }"; // Missing : after name in object
                yield return "{{}}"; // Missing name in object
                yield return "$1.00"; // Currency sign is now allowed in numbers
                yield return "99.00 * 0.15"; // Expression is not allowed in numbers
            }
        }

        [Test]
        public void TryRead_Valid_ReturnsTrueAndJsonToken()
        {
            var obj = new { Value = "value" };
            var value = _serializer.Serialize(obj);

            var result = _reader.TryRead(value, out var token);

            Assert.IsTrue(result);
            Assert.IsTrue(token.IsObject);
        }

        [Test]
        public void TryRead_Invalid_ReturnsFalseAndNull()
        {
            var obj = new { Value = "va\"lue" };
            var value = _serializer.Serialize(obj);

            var result = _reader.TryRead(value, out var token);

            Assert.IsFalse(result);
            Assert.IsNull(token);
        }
    }
}
