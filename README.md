# dotParse

A simple helper for flexibly parsing strings to value types or custom types.

### Usage
The library is as simple as possible. It just does one thing - parses strings to the specified type.

#### Value Types
Value types are easy as they all implement a TypeConverter. The `Parse` function will check
if the specified type has a type converter that converts from string and use that as the function.
```csharp
string input = "123";

var value = input.Parse<int>();
```
#### Custom Types
Custom types are harder of course but assuming there is a `TypeConverter` implementation or a public static `Parse` 
method, this library will detect that and allow you to parse that type without specifying anything.
```csharp
string input = "This is some data";

var value = input.Parse<SomeType>();
```

### Why
Not many good reasons besides I found it helpful, but here are a few.
1. I like `Parse` as an extension method instead of always calling {Type}.Parse() where {Type} is the type being parsed.
    ```csharp
   //This code
   var value = SomeType.Parse(input);
   //Can instead be
   var value = input.Parse<SomeType>();
   //or if we don't have the type at compile time.
   var value = input.Parse(type);
    ```
2. Having the extension method helps shorten the code when the string may be nullable reference type. 
For example, if you have a function that returns a nullable string, something like the following could occur.
    ```csharp
   //This code
   var input = GetInput();  
   var value = input is not null ? int.Parse(input) : 0;
     
   //Can instead be
   var value = GetInput()?.Parse<int>() ?? 0;
    ```
3. Maybe most importantly, I wanted something that can figure out how to parse a type on it's own. The `Parse` method 
can figure out if a custom type has a `TypeConverter` or if it has a `Parse` method using reflection. This way we can just parse
the type without telling it anything. 
   ```csharp
   var input = "This is a custom type vaule";
   var value = input.Parse<CustomType>();
   ```
   Here it is expected that `CustomType` either has a `TypeConverter` implementation that can convert from a string value,
or a public static `Parse` method that takes a string an returns the type `CustomType`. Regarding the use of reflection, 
this library will generate an expression function instead of invoking method via reflection. The functions are also cached
as they are used so performance issues should of much concern.


4. Override with custom parsing functions. The global static Parser class has a registry (dictionary) of parser
functions. These functions are always checked first when parsing to see if we already know how to parse the type or if
the user provided a custom parse function. you can register any type like this.
   ```csharp
   Parser.Register(CustomType.FromString);
   ```
   Here we are registering a custom function `FromString` to use whenever we encounter a type `CustomType`.

