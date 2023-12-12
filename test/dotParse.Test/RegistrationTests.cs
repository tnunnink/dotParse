using FluentAssertions;

namespace dotParse.Test;

[TestFixture]
public class RegistrationTests
{
    [TearDown]
    public void Cleanup()
    {
        Parser.Clear();
    }
    
    [Test]
    public void IsRegistered_SomeClassNotRegistered_ShouldBeFalse()
    {
        var result = Parser.IsRegistered<SomeClass>();

        result.Should().BeFalse();
    }
    
    [Test]
    public void Register_SomeClass_ShouldBeRegistered()
    {
        Parser.Register(s => new SomeClass(s));

        var result = Parser.IsRegistered<SomeClass>();

        result.Should().BeTrue();
    }
    
    [Test]
    public void Register_AlreadyRegistered_ShouldThrowInvalidOperationException()
    {
        Parser.Register(s => new SomeClass(s));

        FluentActions.Invoking(() => Parser.Register(s => new SomeClass(s))).Should()
            .Throw<InvalidOperationException>();
    }
    
    [Test]
    public void Parse_SomeClassAfterRegistration_ShouldBeExpected()
    {
        const string input = "SomeValue";
        Parser.Register(s => new SomeClass(s));

        var result = input.Parse<SomeClass>(); 

        result.Should().NotBeNull();
        result.Value.Should().Be(input);
    }

    [Test]
    public void Remove_TypeIsNotRegistered_ShouldBeFalse()
    {
        var result = Parser.Remove<ParseClass>();

        result.Should().BeFalse();
    }

    [Test]
    public void Remove_TypeIsRegistered_ShouldBeTrueAndNotIsRegistered()
    {
        Parser.Register(ParseClass.Parse);
        Parser.IsRegistered<ParseClass>().Should().BeTrue();

        var result = Parser.Remove<ParseClass>();
        
        result.Should().BeTrue();
        Parser.IsRegistered<ParseClass>().Should().BeFalse();
    }
}