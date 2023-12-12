using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace dotParse;

public static class Parser
{
    private static readonly Dictionary<Type, Func<string, object>> Parsers = new();

    /// <summary>
    /// Parses a string representation of an object into its corresponding type.
    /// </summary>
    /// <typeparam name="T">The type to parse the input string into.</typeparam>
    /// <param name="input">The string to parse.</param>
    /// <param name="provider">An optional format provider for parsing.</param>
    /// <returns>The parsed value of type T.</returns>
    public static T Parse<T>(this string input, IFormatProvider? provider = null) => (T)GetParser(typeof(T))(input);

    /// <summary>
    /// Parses the input string into an object of the specified type.
    /// </summary>
    /// <param name="input">The input string to be parsed.</param>
    /// <param name="type">The type of the object to parse into.</param>
    /// <returns>The parsed object.</returns>
    public static object Parse(this string input, Type type) => GetParser(type)(input);


    /// <summary>
    /// Registers a parser function for a specified type to the internal registry.
    /// </summary>
    /// <typeparam name="T">The type for which the parser is registered.</typeparam>
    /// <param name="parser">The function that parses the input and returns an instance of type T.</param>
    /// <exception cref="InvalidOperationException">Thrown if a parser for the specified type is already registered.</exception>
    public static void Register<T>(Func<string, T> parser) where T : class
    {
        var type = typeof(T);

        if (!Parsers.TryAdd(type, parser))
            throw new InvalidOperationException($"A parser for type '{type}' is already registered.");
    }

    /// <summary>
    /// Removes the parser for the specified type from the internal registry.
    /// </summary>
    /// <typeparam name="T">The type for which to remove the parser.</typeparam>
    /// <returns><c>true</c> if the parser was successfully removed; otherwise, <c>false</c>.</returns>
    public static bool Remove<T>() => Parsers.Remove(typeof(T));

    /// <summary>
    /// Clears all parsers from the internal registry.
    /// </summary>
    public static void Clear() => Parsers.Clear();

    /// <summary>
    /// Determines if a parser is registered for the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to check for registration.</typeparam>
    /// <returns>
    /// <c>true</c> if a parser is registered for the specified type; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsRegistered<T>() => Parsers.ContainsKey(typeof(T));

    /// <summary>
    /// Gets the parser for the specified type.
    /// </summary>
    /// <param name="type">The type of the object to parse.</param>
    /// <returns>The parser delegate for the specified type.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a parser for the specified type cannot be found.</exception>
    private static Func<string, object> GetParser(Type type)
    {
        //1. Registered parse functions take priority over everything else.
        if (Parsers.TryGetValue(type, out var parser))
        {
            return parser;
        }

        //2. Explicitly defined type converters that know how to convert from a string take priority next.
        var converter = TypeDescriptor.GetConverter(type);
        if (converter.CanConvertFrom(typeof(string)))
        {
            Func<string, object> parserConverter = s => converter.ConvertFrom(s);
            Parsers.Add(type, parserConverter);
            return parserConverter;
        }

        //3. Use reflection to find a method with a known/common parse API. If it exists generate a function to call.
        var parseMethod = type.GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(type.IsParseMethod);
        if (parseMethod is not null)
        {
            var parseFunction = GenerateFunction(parseMethod);
            Parsers.Add(type, parseFunction);
            return parseFunction;
        }

        throw new ArgumentOutOfRangeException(nameof(type), $"Could not get parser for type '{type}'.");
    }

    /// <summary>
    /// Generates a function that takes a string parameter and invokes the given method using the parameter value.
    /// </summary>
    /// <param name="method">The method to be invoked.</param>
    /// <returns>A function that takes a string parameter and returns the result of the method call as an object.</returns>
    private static Func<string, object> GenerateFunction(MethodInfo method)
    {
        var parameter = Expression.Parameter(typeof(string), "s");
        var callExpression = Expression.Call(method, parameter);
        var convertExpression = Expression.Convert(callExpression, typeof(object));
        var lambda = Expression.Lambda<Func<string, object>>(convertExpression, parameter);
        return lambda.Compile();
    }

    /// <summary>
    /// Checks if the given <paramref name="methodInfo"/> is a 'Parse' method of the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check the 'Parse' method on.</param>
    /// <param name="methodInfo">The <see cref="MethodInfo"/> representing the method to check.</param>
    /// <returns>
    /// Returns <c>true</c> if the <paramref name="methodInfo"/> is a 'Parse' method that returns the specified <paramref name="type"/>
    /// and accepts a single parameter of type <see cref="string"/>, otherwise returns <c>false</c>.
    /// </returns>
    private static bool IsParseMethod(this Type type, MethodInfo methodInfo)
    {
        return methodInfo.Name == "Parse" &&
               methodInfo.ReturnType == type &&
               methodInfo.GetParameters().Length == 1 &&
               methodInfo.GetParameters()[0].ParameterType == typeof(string);
    }
}