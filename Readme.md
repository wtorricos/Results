# Results

This is a different take on Result that allows you to write error tolerant code that can be composed without losing performance.

Install it from [Nuget](https://www.nuget.org/packages/WTorricos.Results).

## Documentation
Version 2.0.0 contains breaking changes, review the changelog for a migration guide.
- [Changelog](./CHANGELOG.md)

## Why?

We don't have a consolidated approach for error handling in dotnet.
For those of us that like the Result approach we have different options:
- [Josef Ottosson implementation](https://josef.codes/my-take-on-the-result-class-in-c-sharp/)
- [Result](https://github.com/ardalis/Result)
- [ErrorOr](https://github.com/amantinband/error-or)
- [FluentResults](https://github.com/altmann/FluentResults)
- Many more

They are all a great way to write error tolerant code. However, [Results](https://github.com/wtorricos/Results) brings a slightly
different approach inspired in functional programming that intents to be light and easy to get started with.

If you like the repository please feel free to open an issue or a PR for new features or improvements.

## How?

[Results](https://github.com/wtorricos/Results) uses [source generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)
to generate all the boilerplate code for you avoiding reflection. This way you get simplicity and good performance combined.

## Getting Started

Results work with records. In order to create custom errors you just need to decorate your **partial records** with the
`[None]` attribute:

```csharp
// You can name your records whatever you want but the Error suffix is recommended as well as making them sealed
[None]
public sealed partial record DivideByZeroError;
```

By doing this the generator will extend `DivideByZeroError` and will create a partial `DivideByZeroError` class as well as a `DivideByZeroError<T>` generic version.

You can use it like this:

```csharp
public IMaybe<int> MyDivision(int numerator, int denominator)
{
    if (denominator == 0)
    {
        return DivideByZeroError<int>.Create("Division by zero is not allowed");
    }

    return Maybe.Create(numerator / denominator);
}
```

### Main Operations To Work With IMaybe

As of now there are 3 operations that are supported that allow you to compose your results: Map, FlatMap and Action.

Map example:

```csharp
// Works with generic results
IMaybe<int> intResult = Maybe.Create(1);
IMaybe<string> stringResult = intResult.Map(x => x.ToString());

// As well as with non generic results
IMaybe result = Maybe.Create();
IMaybe<string> stringResult = result.Map(() => "Hello World");

// You can also map tasks
Task<IMaybe<int>> intResult = Task.FromResult(Maybe.Create(1));
Task<IMaybe<string>> stringResult = intResult.Map(x => x.ToString());

// Or return tasks
IMaybe<int> intResult = Maybe.Create(1);
Task<IMaybe<string>> taskResult = intResult.Map(i => Task.FromResult(i.ToString(CultureInfo.InvariantCulture)));
```

Flat map supports all the map operations but it "flattens" a result of a result into a single result, for example:

```csharp
IMaybe<int> intResult = Maybe.Create(1);
// If we use map we would get IMaybe<IMaybe<string>> but with flat map we get IMaybe<string>
IMaybe<string> stringResult = intResult.FlatMap(x => Maybe.Create(x.ToString()));
```

Action example:
```csharp
IMaybe<int> intResult = Maybe.Create(1);
intResult.Action(x => Console.WriteLine(x));
```

Review the unit tests for more examples:
- [Map](https://github.com/wtorricos/Results/blob/main/tests/MaybeResults.Test/MaybeExtensionsMapTest.cs)
- [FlatMap](https://github.com/wtorricos/Results/blob/main/tests/MaybeResults.Test/MaybeExtensionsFlatMapTest.cs)
- [Action](https://github.com/wtorricos/Results/blob/main/tests/MaybeResults.Test/MaybeExtensionsActionTest.cs)

You can also review this [complementary post](https://medium.com/@walticotc/result-pattern-in-c-537bedda17a6) for more examples.

### Linq Syntax

Maybe also supports Linq syntax, for example:
```csharp
IMaybe<int> seven = from i in MyDivision(6, 1)
                    from j in MyDivision(2, 2)
                    select i + j;

// which would be equivalent to:
IMaybe<int> some = MyDivision(6, 1)
    .FlatMap(i => MyDivision(2, 2)
        .Map(j => i + j));

// Operations supported with Linq syntax are: from, where and select
IMaybe<int> seven = from i in MyDivision(6, 1)
                    from j in MyDivision(2, 2)
                    where i > j
                    select i + j;
switch (result)
{
    case Some<int> some:
        Console.WriteLine(some.Value);
        break;
    // PredicateFailedError can be used to handle the case when the where clause failed
    case PredicateFailedError<int> err:
        Console.WriteLine(err.Message);
        break;
    // handle custom errors
    case MyError<int> err:
        Console.WriteLine(err.Message);
        break;
    // Handle any other errors
    case INone err:
        Console.WriteLine(err.Message);
        break;
}
```

### Exit Maybe World

What if I want to get out of the IMaybe world? `IsSucess` or similar properties are intentionally not exposed and more well
suited approaches are enforced. For example:
```csharp
// Using switch or switch expressions is the recommended approach as will indirectly motivate you to handle all cases
IMaybe<int> some = MyDivision(5, 0);
switch (some)
{
    case Some<int> result:
        Console.WriteLine($"result: {result.Value}");
        break;
    case DivideByZeroError<int> err:
        Console.WriteLine($"error: {err.Message}");
        break;
    case INone err:
        Console.WriteLine($"unexpected error ${err.Message}");
        break;
}

int value = MyDivision(5, 0) switch
{
    Some<int> success => success.Value,
    DivideByZeroError<int> error => 0, // Fallback value
    INone<int> error => throw new InvalidOperationException(error.GetDisplayMessage()),
    _ => throw new InvalidOperationException("Unknown error")
};

// You can also use pattern matching
IMaybe<int> intResult = MyDivision(5, 0);
if (intResult is Some<int> success)
{
    Console.WriteLine(success.Value);
}
else if (intResult is DivideByZeroError<int> error)
{
    Console.WriteLine(error.GetDisplayMessage());
}

// Don't forget you have is not at your dispossal as well
if (intResult is not Some<int>)
{
    Console.WriteLine("Not a success");
}
```

### Cast

The recommend approach is to use the fluent extensions (Map, FlatMap, Action) and the Linq syntax
to work with Maybe results. However, there may be cases where you can find Cast useful, specially
if you want to create your own extension methods.

As the name suggest Cast is used to cast IMaybe<TypeA> to IMaybe<TypeB> for example:
```csharp
IMaybe<string> ValidateDivision(int numerator, int denominator)
{
    IMaybe<int> result = MyDivision(numerator, denominator);

    if (result is INone error)
    {
        // cannot return error directly because it is of IMaybe<int> type
        return error.Cast<string>();
    }

    return Maybe.Create("Is Valid!");
}

ValidateDivision(1, 0).Should().BeAssignableTo<INone<string>>();
ValidateDivision(1, 0).Should().BeOfType<DivideByZeroError<string>>();
ValidateDivision(1, 1).Should().BeAssignableTo<Some<string>>();
```

## FluentValidation

Here is a gist of an Error that plays well with the [FluentValidation](https://docs.fluentvalidation.net/en/latest/) library.

```csharp
// 1. Create a custom Maybe Error
[None]
public sealed partial record ValidationErrorResult<T>
{
    // Add a custom constructor to create a ValidationErrorResult<T> from a FluentValidation ValidationResult
    public ValidationErrorResult(ValidationResult validationResult)
    {
        // A valid validationResult means it's a dev error that needs to be fixed immediately
        // for that reason it's ok to throw an exception here.
        if (validationResult.IsValid)
        {
            throw new ValidationErrorResultException();
        }

        Message = typeof(T).Name;

        List<NoneDetail> errorList = new(validationResult.Errors.Count);
        errorList.AddRange(validationResult.Errors.Select(e => new NoneDetail(e.PropertyName, e.ErrorMessage)));
        Details = errorList;
    }
}

public sealed class ValidationErrorResultException : Exception
{
    public ValidationErrorResultException() : base("ValidationErrorResult cannot be created from a successful validation")
    {
    }

    public ValidationErrorResultException(string message) : base(message)
    {
    }

    public ValidationErrorResultException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

// 2. Add an AbstractValidator extension method to validate entities
public static class AbstractValidatorExtensions
{
    public static IMaybe<T> ValidateToMaybe<T>(this AbstractValidator<T> validator, T target)
    {
        ValidationResult? result = validator.Validate(target);
        return result is null || result.IsValid
            ? Maybe.Create(target)
            : new ValidationErrorResult<T>(result);
    }
}

// 3. Use it to validate your objects and get an IMaybe result
IMaybe<MyObject> validatedObject = validator.ValidateToMaybe(MyObject);
```
## MediatR

Here is a gist of a Behavior that plays well with the [MediatR](https://github.com/jbogard/MediatR) library.

It will catch any unhandled exception inside a request handler of type IRequestHandler<TIn, IMaybe> or
IRequestHandler<TIn, IMaybe<TOut>> and will guaranty that it doesn't throw exceptions.
```csharp
// 1. We create a generic error that will be returned for unhandled exceptions
[None]
public sealed partial record InternalError;

// 2. We create a generic behavior that will capture unhandled exceptions (truly exceptional exceptions)
/// <summary>
///     By default, we capture every TResponse of type IMaybe inside a try-catch block,
///     in order to guaranty that they always return an IMaybe, even on unexpected exceptions.
/// </summary>
public sealed class RequestErrorHandlerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IMaybe
{
    public async ValueTask<TResponse> Handle(
        TRequest message,
        CancellationToken cancellationToken,
        MessageHandlerDelegate<TRequest, TResponse> next)
    {
        try
        {
            return await next(message: message, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
#pragma warning disable CA1031 // catch a more specific exception
        catch (Exception e)
#pragma warning restore CA1031
        {
            Type responseType = typeof(TResponse);
            Type[] genericArguments = responseType.GetGenericArguments();

            Log.Error("Failed to create response {responseType} for {requestType}", responseType.Name, message.GetType().Name);

            // if there are no generic arguments it means TResponse is of type Result
            if (genericArguments.Length == 0)
            {
                return (TResponse)InternalError.Create(e.Message);
            }

            // if there are generic arguments it means TResponse is of type Result<T>
            Type genericErrorType = typeof(InternalError<>);
            Type responseErrorType = genericErrorType.MakeGenericType(genericArguments);

            // We use the constructor instead of the Create factory method for convenience
            object error = Activator.CreateInstance(type: responseErrorType, e.Message, Array.Empty<NoneDetail>())!;
            return (TResponse)error;
        }
    }
}

// 3. We register the behavior in the DI container (remember that order matters when you register this behavior)
services
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestErrorHandlerBehavior<,>));
```
