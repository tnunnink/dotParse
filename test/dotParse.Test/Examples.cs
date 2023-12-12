namespace dotParse.Test;

[TestFixture]
public class Examples
{
    [Test]
    [TestCase("123")]
    public void METHOD(string? input)
    {
        //This code:
        var temp1 = input is not null ? int.Parse(input) : 0;
        
        //Can instead be:
        var temp2 = input?.Parse<int>() ?? 0;
    }
}