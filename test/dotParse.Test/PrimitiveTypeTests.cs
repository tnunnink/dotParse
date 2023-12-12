using FluentAssertions;

namespace dotParse.Test;

public class PrimitiveTypeTests
{
    [Test]
    [TestCase("true", true)]
    [TestCase("TRUE", true)]
    [TestCase("false", false)]
    [TestCase("FALSE", false)]
    public void Parse_BooleanValues_ShouldBeExpected(string input, bool expected)
    {
        var result = input.Parse<bool>();

        result.Should().Be(expected);
    }

    [Test]
    [TestCase("0", 0)]
    [TestCase("1", 1)]
    [TestCase("100", 100)]
    [TestCase("123", 123)]
    [TestCase("-123", -123)]
    public void Parse_IntValue_ShouldBeExpected(string input, int expected)
    {
        var result = input.Parse<int>();

        result.Should().Be(expected);
    }

    [Test]
    [TestCase("0.0", 0.0)]
    [TestCase("1.23", 1.23)]
    [TestCase("0.12345", 0.12345)]
    [TestCase("-1100.14356", -1100.14356)]
    public void Parse_DoubleValues_ShouldBeExpected(string input, double expected)
    {
        var result = input.Parse<double>();

        result.Should().Be(expected);
    }

    /*[Test]
    [TestCase("1/1/2023", new DateTime(2023))]
    public void Parse_DateTimes_ShouldBeExpected(string input, DateTime expected)
    {
        var result = input.Parse<DateTime>();

        result.Should().Be(expected);
    }*/
}