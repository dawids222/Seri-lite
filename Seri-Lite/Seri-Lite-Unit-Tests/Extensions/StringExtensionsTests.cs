using NUnit.Framework;
using Seri_Lite.Extensions;
using System.Collections.Generic;

namespace Seri_Lite_Unit_Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [TestCaseSource(nameof(_invertFirstLetterTestCaseSource))]
        public void InvertFirstLetter_ReturnsStringWithInvertedFirstLetter(InvertFirstLetterTest test)
        {
            var result = test.Input.InvertFirstLetter();

            Assert.AreEqual(test.Expected, result);
        }

        private static IEnumerable<InvertFirstLetterTest> _invertFirstLetterTestCaseSource = new InvertFirstLetterTest[]
        {
            new InvertFirstLetterTest("Test", "test"),
            new InvertFirstLetterTest("test", "Test"),
            new InvertFirstLetterTest(null, null),
            new InvertFirstLetterTest("", ""),
            new InvertFirstLetterTest("1", "1"),
            new InvertFirstLetterTest("@", "@"),
        };
    }

    public class InvertFirstLetterTest
    {
        public string Input { get; }
        public string Expected { get; }

        public InvertFirstLetterTest(string input, string expected)
        {
            Input = input;
            Expected = expected;
        }
    }
}
