using FluentAssertions;

namespace dotParse.Test;

public class CustomTypeTests
{
    [Test]
    public void Parse_ParseClass_ShouldHaveExpectedValues()
    {
        const string input = "This is a test input";

        var result = input.Parse<ParseClass>();

        result.Should().NotBeNull();
        result.Value.Should().Be("This is a test input");
    }
    
    [Test]
    public void ParseByType_ParseClass_ShouldHaveExpectedValues()
    {
        const string input = "This is a test input";

        var result = (ParseClass)input.Parse(typeof(ParseClass));

        result.Should().NotBeNull();
        result.Value.Should().Be("This is a test input");
    }

    [Test]
    public void Parse_SomeClass_ShouldThrowArgumentOutOfRangeException()
    {
        const string input = "This is a test input";

        FluentActions.Invoking(() => input.Parse<SomeClass>()) .Should().Throw<ArgumentOutOfRangeException>();
    }
}