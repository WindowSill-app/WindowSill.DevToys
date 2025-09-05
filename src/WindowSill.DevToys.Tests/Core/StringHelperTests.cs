using FluentAssertions;
using WindowSill.DevToys.Core;

namespace WindowSill.DevToys.Tests.Core;

public class StringHelperTests
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("hello world", "hello world")]
    [InlineData("hello\rworld", "hello\rworld")]
    [InlineData("hello\\rworld", "hello\rworld")]
    internal void UnescapeString(string input, string expectedResult)
    {
        StringHelper.UnescapeString(input).Should().Be(expectedResult);
    }
}
