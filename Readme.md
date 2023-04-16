# Results

This is a different take on Result that allows you to write error tolerant code that can be composed without losing performance.

Install it from [Nuget](https://www.nuget.org/packages/WTorricos.Results).

## Why?

We don't have a consolidated approach for error handling in dotnet.
For those of us that like the Result approach we have different options:
- [Result](https://github.com/ardalis/Result)
- [ErrorOr](https://github.com/amantinband/error-or)
- [FluentResults](https://github.com/altmann/FluentResults)
- Many more

They are all a great way to write error tolerant code. However, [Results](https://github.com/wtorricos/Results) brings a slightly
different approach inspired in functional programming that intents to be light and easy to get started with.

If you like the repository please feel free to open an issue or a PR for new features or improvements.

## How?

[Results](https://github.com/wtorricos/Results) uses [source generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)
to generate all the boiler plate code for you, and avoid using reflection at the same time. This way you get good performance in
a simple and easy to use way.

## Getting Started

Results works with records, in order to create custom errors you just need to decorate your **partial records** with the
`ErrorResult` attribute: (You can name your records whatever you want but the Error suffix is recommended as well as making them sealed)

```csharp
[ErrorResult]
public sealed partial record OperationNotSupportedError;
```

By doing this the generator will extend `OperationNotSupportedError` and will create a generic version as well
`OperationNotSupportedError<T>` so you can use them like this:

```csharp
// Non generic version
public IResult MyOperation(int value)
{
    if (value == 0)
    {
        // Public constructors are generated as well, but Factory methods are recommended.
        return OperationNotSupportedError.Create("The value cannot be 0");
    }

    // process the value

    return Result.Success();
}

// Generic version
public IResult<int> MyOperationT(int value)
{
    if (value == 0)
    {
        // Public constructors are generated as well, but Factory methods are recommended.
        return OperationNotSupportedError<int>.Create("The value cannot be 0");
    }

    // process the value

    return Result.Success(value);
}
```

As of now there are 3 operations that are supported that allow you to compose your results:
- [Map](https://github.com/wtorricos/Results/blob/main/tests/Results.UnitTests/ResultExtensionsMapTest.cs)
- [FlatMap](https://github.com/wtorricos/Results/blob/main/tests/Results.UnitTests/ResultExtensionsFlatMapTest.cs)
- [Action](https://github.com/wtorricos/Results/blob/main/tests/Results.UnitTests/ResultExtensionsActionTest.cs)

Map example:

```csharp
// Works with generic results
IResult<int> intResult = Result.Success(1);
IResult<string> stringResult = intResult.Map(x => x.ToString());

// As well as with non generic results
IResult result = Result.Success();
IResult<string> stringResult = result.Map(() => "Hello World");

// You can also map tasks
Task<IResult<int>> intResult = Task.FromResult(Result.Success(1));
Task<IResult<string>> stringResult = intResult.Map(x => x.ToString());

// Or return tasks
IResult<int> intResult = Result.Success(data: 1);
Task<IResult<string>> taskResult = sut.Map(i => Task.FromResult(i.ToString(CultureInfo.InvariantCulture)));
```

Flat map supports all the map operations but it "flattens" a result of a result into a single result, for example:

```csharp
IResult<int> intResult = Result.Success(1);
// If we use map we would get Result<Result<string>> but with flat map we get Result<string>
IResult<string> stringResult = intResult.FlatMap(x => Result.Success(x.ToString()));
```

Action example:
```csharp
IResult<int> intResult = Result.Success(1);
intResult.Action(x => Console.WriteLine(x));
```

What if I want to get out of the Result world? `IsSucess` or similar properties are intentionally not exposed and better more well
suited approaches are enforced. For example:
```csharp
// Using switch or switch expressions is the recommended approach as will indirectly motivate you to handle all cases
IResult<int> intResult = Result.Success(1);
switch (intResult)
{
    case SuccessResult<int> success:
        Console.WriteLine(success.Data);
        break;
    case OperationNotSupportedError<int> error:
        Console.WriteLine(error.GetDisplayMessage());
        break;
    case IErrorResult<int> error:
        Console.WriteLine(error.GetDisplayMessage());
        break;
}

int value = intResult switch
{
    SuccessResult<int> success => success.Data,
    OperationNotSupportedError<int> error: 0, // Fallback value
    IErrorResult<int> error => throw new InvalidOperationException(error.GetDisplayMessage()),
    _ => throw new InvalidOperationException("Unknown error")
};

// You can also use pattern matching
if (intResult is SuccessResult<int> success)
{
    Console.WriteLine(success.Data);
}
else if (intResult is IErrorResult<int> error)
{
    Console.WriteLine(error.GetDisplayMessage());
}

// Don't forget you have is not at your dispossal as well
if (intResult is not SuccessResult<int>)
{
    Console.WriteLine("Not a success");
}
```
