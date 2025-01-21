using System.Diagnostics.CodeAnalysis;
namespace MaybeResults;

public static class MaybeExtensions
{
    public static IMaybe<TOut> Map<TOut>(this IMaybe result, Func<TOut> successFunc) =>
        result.Match(successFunc);

    public static Task<IMaybe<TOut>> Map<TOut>(this IMaybe result, Func<Task<TOut>> successFunc) =>
        result.Match(successFunc);

    public static IMaybe<TOut> Map<TIn, TOut>(this IMaybe<TIn> result, Func<TIn, TOut> mapper) =>
        result.Match(
            mapper,
            err => err.Cast<TOut>());

    public static async Task<IMaybe<TOut>> Map<TIn, TOut>(this IMaybe<TIn> result, Func<TIn, Task<TOut>> mapper) =>
        await result.Match(
            async value =>
            {
                TOut mappedResult = await mapper(value).ConfigureAwait(continueOnCapturedContext: false);
                return mappedResult;
            },
            err => err.Cast<TOut>());

    public static async Task<IMaybe<TOut>> Map<TIn, TOut>(this Task<IMaybe<TIn>> result, Func<TIn, TOut> mapper)
    {
        IMaybe<TIn> awaitedResult = await result.ConfigureAwait(continueOnCapturedContext: false);
        return awaitedResult.Map(mapper);
    }

    public static async Task<IMaybe<TOut>> Map<TIn, TOut>(this Task<IMaybe<TIn>> result, Func<TIn, Task<TOut>> mapper)
    {
        IMaybe<TIn> awaitedResult = await result.ConfigureAwait(continueOnCapturedContext: false);
        return await awaitedResult.Map(mapper).ConfigureAwait(continueOnCapturedContext: false);
    }

    public static IMaybe<T> Flatten<T>(this IMaybe<IMaybe<T>> result) =>
        result switch
        {
            Some<IMaybe<T>> some => some.Value,
            INone<IMaybe<T>> err => err.Cast<T>(),
            _ => FlattenThrowArgumentOutOfRangeException<T>(
                expected: $"{nameof(Some<IMaybe<T>>)}, {nameof(INone<IMaybe<T>>)}", result.GetType().Name)
        };

    [ExcludeFromCodeCoverage]
    static IMaybe<T> FlattenThrowArgumentOutOfRangeException<T>(string expected, string actual) =>
        throw new ArgumentOutOfRangeException(
            paramName: "result",
            message:
            $"Invalid result type, expected one of [{expected}] but got {actual})");

    public static IMaybe Flatten(this IMaybe<IMaybe> result) =>
        result switch
        {
            Some<IMaybe> some => some.Value,
            INone<IMaybe> err => err,
            _ => FlattenThrowArgumentOutOfRangeException(expected: $"{nameof(Some<IMaybe>)}, {nameof(INone<IMaybe>)}",
                result.GetType().Name)
        };

    [ExcludeFromCodeCoverage]
    static IMaybe FlattenThrowArgumentOutOfRangeException(string expected, string actual) =>
        throw new ArgumentOutOfRangeException(
            paramName: "result",
            message:
            $"Invalid result type, expected one of [{expected}] but got {actual})");

    public static async Task<IMaybe<T>> Flatten<T>(this Task<IMaybe<IMaybe<T>>> result) =>
        (await result.ConfigureAwait(continueOnCapturedContext: false)).Flatten();

    public static IMaybe FlatMap(this IMaybe result, Func<IMaybe> successFunc) =>
        result.Match(successFunc);

    public static async Task<IMaybe> FlatMap(this IMaybe result, Func<Task<IMaybe>> successFunc) =>
        await result.Match(successFunc).ConfigureAwait(false);

    public static IMaybe<TOut> FlatMap<TOut>(this IMaybe result, Func<IMaybe<TOut>> successFunc) =>
        result.Match(successFunc).Flatten();

    public static Task<IMaybe<TOut>> FlatMap<TOut>(this IMaybe result, Func<Task<IMaybe<TOut>>> mapper) =>
        result.Map(mapper).Flatten();

    public static async Task<IMaybe> FlatMap(this Task<IMaybe> taskResult, Func<IMaybe> successFunc)
    {
        IMaybe result = await taskResult.ConfigureAwait(continueOnCapturedContext: false);
        return result.Match(successFunc);
    }

    public static async Task<IMaybe<TOut>> FlatMap<TOut>(this Task<IMaybe> taskResult, Func<IMaybe<TOut>> successFunc)
    {
        IMaybe result = await taskResult.ConfigureAwait(continueOnCapturedContext: false);
        return result.Match(successFunc).Flatten();
    }

    public static async Task<IMaybe<TOut>> FlatMap<TOut>(this Task<IMaybe> taskResult, Func<Task<IMaybe<TOut>>> mapper)
    {
        IMaybe result = await taskResult.ConfigureAwait(continueOnCapturedContext: false);
        return (await result.Match(mapper).ConfigureAwait(continueOnCapturedContext: false)).Flatten();
    }

    public static IMaybe<TOut> FlatMap<TIn, TOut>(this IMaybe<TIn> result, Func<TIn, IMaybe<TOut>> mapper) =>
        result.Map(mapper).Flatten();

    public static Task<IMaybe<TOut>> FlatMap<TIn, TOut>(this IMaybe<TIn> result, Func<TIn, Task<IMaybe<TOut>>> mapper) =>
        result.Map(mapper).Flatten();

    public static async Task<IMaybe<TOut>> FlatMap<TIn, TOut>(
        this Task<IMaybe<TIn>> result,
        Func<TIn, IMaybe<TOut>> mapper)
    {
        IMaybe<TIn> awaitedResult = await result.ConfigureAwait(continueOnCapturedContext: false);
        return awaitedResult.FlatMap(mapper);
    }

    public static async Task<IMaybe<TOut>> FlatMap<TIn, TOut>(
        this Task<IMaybe<TIn>> result,
        Func<TIn, Task<IMaybe<TOut>>> mapper)
    {
        IMaybe<TIn> awaitedResult = await result.ConfigureAwait(continueOnCapturedContext: false);
        return await awaitedResult.FlatMap(mapper).ConfigureAwait(continueOnCapturedContext: false);
    }

    public static async Task<IMaybe> FlatMap<TIn>(
        this Task<IMaybe<TIn>> result,
        Func<TIn, Task<IMaybe>> mapper)
    {
        IMaybe<IMaybe> maybe = await (await result.ConfigureAwait(continueOnCapturedContext: false))
            .Map(mapper).ConfigureAwait(continueOnCapturedContext: false);
        return maybe.Flatten();
    }

    [ExcludeFromCodeCoverage]
    static IMaybe FlatMapThrowArgumentOutOfRangeException(string paramName, string message) =>
        throw new ArgumentOutOfRangeException(paramName, message);

    public static async Task<IMaybe> FlatMap<TIn>(
        this Task<IMaybe<TIn>> result,
        Func<TIn, IMaybe> mapper)
    {
        IMaybe<TIn> maybe = await result.ConfigureAwait(continueOnCapturedContext: false);
        return maybe.Match(mapper);
    }

    public static void Action<TIn>(this IMaybe<TIn> result, Action<TIn> onSuccess, Action<INone>? onError = null) =>
        _ = result.Match(
            value =>
            {
                onSuccess(value);
                return value;
            },
            err =>
            {
                onError?.Invoke(err);
            });

    public static async Task Action<TIn>(
        this Task<IMaybe<TIn>> result,
        Action<TIn> onSuccess,
        Action<INone>? onError = null)
    {
        IMaybe<TIn> awaitedResult = await result.ConfigureAwait(continueOnCapturedContext: false);
        awaitedResult.Action(onSuccess, onError);
    }

    public static T GetValueOrThrow<T>(this IMaybe<T> maybe)
    {
        if (maybe is Some<T> some)
        {
            return some.Value;
        }

        throw new InvalidOperationException($"Expected {nameof(IMaybe<T>)} but got {maybe.GetType().Name}");
    }
}
