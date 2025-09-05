using FluentAssertions;
using WindowSill.DevToys.Core;

namespace WindowSill.DevToys.Tests.Core;

public class JsonHelperTests
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData(" ", "")]
    [InlineData("   {  }  ", "{}")]
    [InlineData("   [  ]  ", "[]")]
    [InlineData("   { \"foo\": 123 }  ", "{\r\n  \"foo\": 123\r\n}")]
    public void FormatTwoSpaces(string? input, string expectedResult)
    {
        expectedResult = expectedResult.Replace("\r\n", Environment.NewLine);
        string result = JsonHelper.Format(input, Indentation.TwoSpaces);
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData(" ", "")]
    [InlineData("   {  }  ", "{}")]
    [InlineData("   [  ]  ", "[]")]
    [InlineData("   { \"foo\": 123 }  ", "{\r\n    \"foo\": 123\r\n}")]
    public void FormatFourSpaces(string? input, string expectedResult)
    {
        expectedResult = expectedResult.Replace("\r\n", Environment.NewLine);
        string result = JsonHelper.Format(input, Indentation.FourSpaces);
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData(" ", "")]
    [InlineData("   {  }  ", "{}")]
    [InlineData("   [  ]  ", "[]")]
    [InlineData("   { \"foo\": 123 }  ", "{\r\n\t\"foo\": 123\r\n}")]
    public void FormatOneTab(string? input, string expectedResult)
    {
        expectedResult = expectedResult.Replace("\r\n", Environment.NewLine);
        string result = JsonHelper.Format(input, Indentation.OneTab);
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData(" ", "")]
    [InlineData("   {  }  ", "{}")]
    [InlineData("   [  ]  ", "[]")]
    [InlineData("   { \"foo\": 123 }  ", "{\"foo\":123}")]
    public void FormatMinifiedAsync(string? input, string expectedResult)
    {
        string result = JsonHelper.Format(input, Indentation.Minified);
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("{ \"Date\": \"2012-04-21T18:25:43-05:00\" }", "{\"Date\":\"2012-04-21T18:25:43-05:00\"}")]
    public void FormatDoesNotAlterateDateTimesAsync(string? input, string expectedResult)
    {
        string result = JsonHelper.Format(input, Indentation.Minified);
        result.Should().Be(expectedResult);
    }
}
