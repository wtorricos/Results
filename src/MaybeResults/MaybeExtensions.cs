namespace MaybeResults;

public static class MaybeExtensions
{
    public static IMaybe<TOut> Map<TOut>(this IMaybe result, Func<TOut> successFunc)
    {
        return result switch
        {
            Some => Maybe.Create(successFunc()),
            INone none => none.Cast<TOut>(),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };
    }

    public static async Task<IMaybe<TOut>> Map<TOut>(this IMaybe result, Func<Task<TOut>> successFunc)
    {
        return result switch
        {
            Some => Maybe.Create(await successFunc().ConfigureAwait(continueOnCapturedContext: false)),
            INone none => none.Cast<TOut>(),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };
    }

    public static IMaybe<TOut> Map<TIn, TOut>(this IMaybe<TIn> result, Func<TIn, TOut> mapper)
    {
        return result.Match(
            mapper,
            err => err.Cast<TOut>());
    }

    public static async Task<IMaybe<TOut>> Map<TIn, TOut>(this IMaybe<TIn> result, Func<TIn, Task<TOut>> mapper)
    {
        return await result.Match(
            async value =>
            {
                TOut mappedResult = await mapper(value).ConfigureAwait(continueOnCapturedContext: false);
                return mappedResult;
            },
            err => err.Cast<TOut>());
    }

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

    public static IMaybe<T> Flatten<T>(this IMaybe<IMaybe<T>> result)
    {
        return result switch
        {
            Some<IMaybe<T>> some => some.Value,
            INone<IMaybe<T>> err => err.Cast<T>(),
            _ => throw new ArgumentOutOfRangeException(nameof(result), "Invalid result type")
        };
    }

    public static async Task<IMaybe<T>> Flatten<T>(this Task<IMaybe<IMaybe<T>>> result)
    {
        return (await result.ConfigureAwait(continueOnCapturedContext: false)).Flatten();
    }

    public static IMaybe FlatMap(this IMaybe result, Func<IMaybe> successFunc)
    {
        return result switch
        {
            Some => successFunc(),
            INone none => none,
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };
    }

    public static async Task<IMaybe> FlatMap(this IMaybe result, Func<Task<IMaybe>> successFunc)
    {
        return result switch
        {
            Some => await successFunc().ConfigureAwait(continueOnCapturedContext: false),
            INone none => none,
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };
    }

    public static IMaybe<TOut> FlatMap<TOut>(this IMaybe result, Func<IMaybe<TOut>> successFunc)
    {
        return result switch
        {
            Some => successFunc(),
            INone none => none.Cast<TOut>(),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };
    }

    public static Task<IMaybe<TOut>> FlatMap<TOut>(this IMaybe result, Func<Task<IMaybe<TOut>>> mapper)
    {
        return result switch
        {
            Some => mapper(),
            INone err => Task.FromResult(err.Cast<TOut>()),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };
    }

    public static async Task<IMaybe> FlatMap(this Task<IMaybe> taskResult, Func<IMaybe> successFunc)
    {
        IMaybe result = await taskResult.ConfigureAwait(continueOnCapturedContext: false);
        return result switch
        {
            Some => successFunc(),
            INone none => none,
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };
    }

    public static async Task<IMaybe<TOut>> FlatMap<TOut>(this Task<IMaybe> taskResult, Func<IMaybe<TOut>> successFunc)
    {
        IMaybe result = await taskResult.ConfigureAwait(continueOnCapturedContext: false);
        return result switch
        {
            Some => successFunc(),
            INone none => none.Cast<TOut>(),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };
    }

    public static async Task<IMaybe<TOut>> FlatMap<TOut>(this Task<IMaybe> taskResult, Func<Task<IMaybe<TOut>>> mapper)
    {
        IMaybe result = await taskResult.ConfigureAwait(continueOnCapturedContext: false);
        return result switch
        {
            Some => await mapper(),
            INone err => err.Cast<TOut>(),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };
    }

    public static IMaybe<TOut> FlatMap<TIn, TOut>(this IMaybe<TIn> result, Func<TIn, IMaybe<TOut>> mapper)
    {
        return result
            .Map(mapper)
            .Flatten();
    }

    public static Task<IMaybe<TOut>> FlatMap<TIn, TOut>(this IMaybe<TIn> result, Func<TIn, Task<IMaybe<TOut>>> mapper)
    {
        return result
            .Map(mapper)
            .Flatten();
    }

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
        return maybe switch
        {
            Some<IMaybe> => Some.Instance,
            INone none => none,
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };
    }

    public static async Task<IMaybe> FlatMap<TIn>(
        this Task<IMaybe<TIn>> result,
        Func<TIn, IMaybe> mapper)
    {
        return (await result.ConfigureAwait(continueOnCapturedContext: false)).Map(mapper) switch
        {
            Some<IMaybe> => Some.Instance,
            INone none => none,
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };
    }

    public static void Action<TIn>(this IMaybe<TIn> result, Action<TIn> onSuccess, Action<INone>? onError = null)
    {
        _ = result.Match(
            value =>
            {
                onSuccess(value);
                return value;
            },
            err =>
            {
                onError?.Invoke(err);
                return err.Cast<TIn>();
            });
    }

    public static async Task Action<TIn>(
        this Task<IMaybe<TIn>> result,
        Action<TIn> onSuccess,
        Action<INone>? onError = null)
    {
        IMaybe<TIn> awaitedResult = await result.ConfigureAwait(continueOnCapturedContext: false);
        awaitedResult.Action(onSuccess, onError);
    }
}
