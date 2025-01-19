namespace MaybeResults;

public static class MaybeExtensions
{
    public static IMaybe<TOut> Map<TOut>(this IMaybe result, Func<TOut> successFunc) =>
        result switch
        {
            Some => Maybe.Create(successFunc()),
            INone none => none.Cast<TOut>(),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };

    public static async Task<IMaybe<TOut>> Map<TOut>(this IMaybe result, Func<Task<TOut>> successFunc) =>
        result switch
        {
            Some => Maybe.Create(await successFunc()),
            INone none => none.Cast<TOut>(),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };

    public static IMaybe<TOut> Map<TIn, TOut>(this IMaybe<TIn> result, Func<TIn, TOut> mapper) =>
        result switch
        {
            Some<TIn> some => Maybe.Create(mapper(some.Value)),
            INone<TIn> none => none.Cast<TOut>(),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };

    public static async Task<IMaybe<TOut>> Map<TIn, TOut>(this IMaybe<TIn> result, Func<TIn, Task<TOut>> mapper)
    {
        switch (result)
        {
            case Some<TIn> some:
                TOut mappedResult = await mapper(some.Value).ConfigureAwait(continueOnCapturedContext: false);
                return Maybe.Create(mappedResult);
            case INone<TIn> none:
                return none.Cast<TOut>();
            default:
                throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type");
        }
    }

    public static async Task<IMaybe<TOut>> Map<TIn, TOut>(this Task<IMaybe<TIn>> result, Func<TIn, TOut> mapper)
    {
        IMaybe<TIn> awaitedResult = await result.ConfigureAwait(continueOnCapturedContext: false);
        switch (awaitedResult)
        {
            case Some<TIn> some:
                TOut mappedResult = mapper(some.Value);
                return Maybe.Create(mappedResult);
            case INone<TIn> none:
                return none.Cast<TOut>();
            default:
                throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type");
        }
    }

    public static async Task<IMaybe<TOut>> Map<TIn, TOut>(this Task<IMaybe<TIn>> result, Func<TIn, Task<TOut>> mapper)
    {
        IMaybe<TIn> awaitedResult = await result.ConfigureAwait(continueOnCapturedContext: false);
        switch (awaitedResult)
        {
            case Some<TIn> some:
                TOut mappedResult = await mapper(some.Value);
                return Maybe.Create(mappedResult);
            case INone<TIn> none:
                return none.Cast<TOut>();
            default:
                throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type");
        }
    }

    public static IMaybe FlatMap(this IMaybe result, Func<IMaybe> successFunc) =>
        result switch
        {
            Some => successFunc(),
            INone none => none,
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };

    public static async Task<IMaybe> FlatMap(this IMaybe result, Func<Task<IMaybe>> successFunc) =>
        result switch
        {
            Some => await successFunc(),
            INone none => none,
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };

    public static IMaybe<TOut> FlatMap<TOut>(this IMaybe result, Func<IMaybe<TOut>> successFunc) =>
        result switch
        {
            Some => successFunc(),
            INone none => none.Cast<TOut>(),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };

    public static Task<IMaybe<TOut>> FlatMap<TOut>(this IMaybe result, Func<Task<IMaybe<TOut>>> mapper) =>
        result switch
        {
            Some => mapper(),
            INone err => Task.FromResult(err.Cast<TOut>()),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };

    public static async Task<IMaybe> FlatMap(this Task<IMaybe> taskResult, Func<IMaybe> successFunc)
    {
        IMaybe result = await taskResult;
        return result switch
        {
            Some => successFunc(),
            INone none => none,
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };
    }

    public static async Task<IMaybe<TOut>> FlatMap<TOut>(this Task<IMaybe> taskResult, Func<IMaybe<TOut>> successFunc)
    {
        IMaybe result = await taskResult;
        return result switch
        {
            Some => successFunc(),
            INone none => none.Cast<TOut>(),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };
    }

    public static async Task<IMaybe<TOut>> FlatMap<TOut>(this Task<IMaybe> taskResult, Func<Task<IMaybe<TOut>>> mapper)
    {
        IMaybe result = await taskResult;
        return result switch
        {
            Some => await mapper(),
            INone err => err.Cast<TOut>(),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };
    }

    public static IMaybe<TOut> FlatMap<TIn, TOut>(this IMaybe<TIn> result, Func<TIn, IMaybe<TOut>> mapper) => result switch
    {
        Some<TIn> some => mapper(some.Value),
        INone<TIn> none => none.Cast<TOut>(),
        _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
    };

    public static Task<IMaybe<TOut>> FlatMap<TIn, TOut>(this IMaybe<TIn> result, Func<TIn, Task<IMaybe<TOut>>> mapper)
    {
        switch (result)
        {
            case Some<TIn> some:
                return mapper(some.Value);
            case INone<TIn> none:
                IMaybe<TOut> err = none.Cast<TOut>();
                return Task.FromResult(err);
            default:
                throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type");
        }
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
        Func<TIn, Task<IMaybe>> mapper) =>
        await result.ConfigureAwait(false) switch
        {
            Some<TIn> some => await mapper(some.Value).ConfigureAwait(false),
            INone none => none,
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };

    public static async Task<IMaybe> FlatMap<TIn>(
        this Task<IMaybe<TIn>> result,
        Func<TIn, IMaybe> mapper) =>
        await result.ConfigureAwait(false) switch
        {
            Some<TIn> some => mapper(some.Value),
            INone none => none,
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type")
        };

    public static void Action<TIn>(this IMaybe<TIn> result, Action<TIn> onSuccess, Action<INone>? onError = null)
    {
        switch (result)
        {
            case Some<TIn> some:
                onSuccess(some.Value);
                break;
            case INone<TIn> none:
                onError?.Invoke(none);
                break;
            default:
                throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type");
        }
    }

    public static async Task Action<TIn>(
        this Task<IMaybe<TIn>> result,
        Action<TIn> onSuccess,
        Action<INone>? onError = null)
    {
        IMaybe<TIn> awaitedResult = await result.ConfigureAwait(continueOnCapturedContext: false);
        switch (awaitedResult)
        {
            case Some<TIn> some:
                onSuccess(some.Value);
                break;
            case INone<TIn> none:
                onError?.Invoke(none);
                break;
            default:
                throw new ArgumentOutOfRangeException(paramName: nameof(result), message: "Invalid result type");
        }
    }
}
