using System.ComponentModel;
using System.Globalization;

namespace dotParse.Test;

public class ParsableClass : IParsable<ParsableClass>
{
    public string Data { get; private set; } = string.Empty;
    
    public static ParsableClass Parse(string s, IFormatProvider? provider)
    {
        return new ParsableClass { Data = s };
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out ParsableClass result)
    {
        if (s is null)
        {
            result = new ParsableClass();
            return false;
        }
        
        result = new ParsableClass { Data = s };
        return true;
    }
}

public class ParseClass
{
    public string Value { get; private set; } = string.Empty;

    public static ParseClass Parse(string input)
    {
        return new ParseClass { Value = input };
    }
    
    public static bool TryParse(string input, out ParseClass result)
    {
        result = new ParseClass { Value = input };
        return true;
    }
}

[TypeConverter(typeof(ClassConverter))]
public class ConvertableClass(string value)
{
    public string Value { get; } = value;
}

public class ClassConverter : TypeConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string s)
        {
            return new ConvertableClass(s);
        }
        
        return base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string);
    }
}

public class SomeClass(string value)
{
    public string Value { get; } = value;

    public static object Parse(string value) => new SomeClass(value);
}