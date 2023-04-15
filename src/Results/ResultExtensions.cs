namespace Results;

public static class ResultExtensions
{
    public static IResult<TOut> Map<TOut>(this IResult result, Func<TOut> successFunc) =>
        result switch
        {
            SuccessResult => Result.Success(successFunc()),
            IErrorResult errorResult => errorResult.Cast<TOut>(),
            _ => throw new ArgumentOutOfRangeException(nameof(result), message: "Invalid result type")
        };

    public static IResult<TOut> Map<TIn, TOut>(this IResult<TIn> result, Func<TIn, TOut> mapper) =>
        result switch
        {
            SuccessResult<TIn> successResult => Result.Success(mapper(successResult.Data)),
            IErrorResult<TIn> errorResult => errorResult.Cast<TOut>(),
            _ => throw new ArgumentOutOfRangeException(nameof(result), message: "Invalid result type")
        };

    public static async Task<IResult<TOut>> Map<TIn, TOut>(this IResult<TIn> result, Func<TIn, Task<TOut>> mapper)
    {
        switch (result)
        {
            case SuccessResult<TIn> successResult:
                TOut mappedResult = await mapper(successResult.Data).ConfigureAwait(continueOnCapturedContext: false);
                return Result.Success(mappedResult);
            case IErrorResult<TIn> errorResult:
                return errorResult.Cast<TOut>();
            default:
                throw new ArgumentOutOfRangeException(nameof(result), message: "Invalid result type");
        }
    }

    public static async Task<IResult<TOut>> Map<TIn, TOut>(this Task<IResult<TIn>> result, Func<TIn, TOut> mapper)
    {
        IResult<TIn> awaitedResult = await result.ConfigureAwait(continueOnCapturedContext: false);
        switch (awaitedResult)
        {
            case SuccessResult<TIn> successResult:
                TOut mappedResult = mapper(successResult.Data);
                return Result.Success(mappedResult);
            case IErrorResult<TIn> errorResult:
                return errorResult.Cast<TOut>();
            default:
                throw new ArgumentOutOfRangeException(nameof(result), message: "Invalid result type");
        }
    }

    public static IResult FlatMap(this IResult result, Func<IResult> successFunc) =>
        result switch
        {
            SuccessResult => Result.Success(successFunc()),
            IErrorResult errorResult => errorResult,
            _ => throw new ArgumentOutOfRangeException(nameof(result), message: "Invalid result type")
        };

    public static IResult<TOut> FlatMap<TOut>(this IResult result, Func<IResult<TOut>> successFunc) =>
        result switch
        {
            SuccessResult => successFunc(),
            IErrorResult errorResult => errorResult.Cast<TOut>(),
            _ => throw new ArgumentOutOfRangeException(nameof(result), message: "Invalid result type")
        };

    public static IResult<TOut> FlatMap<TIn, TOut>(this IResult<TIn> result, Func<TIn, IResult<TOut>> mapper) => result switch
    {
        SuccessResult<TIn> successResult => mapper(successResult.Data),
        IErrorResult<TIn> errorResult => errorResult.Cast<TOut>(),
        _ => throw new ArgumentOutOfRangeException(nameof(result), message: "Invalid result type")
    };

    public static Task<IResult<TOut>> FlatMap<TIn, TOut>(this IResult<TIn> result, Func<TIn, Task<IResult<TOut>>> mapper)
    {
        switch (result)
        {
            case SuccessResult<TIn> successResult:
                return mapper(successResult.Data);
            case IErrorResult<TIn> errorResult:
                IResult<TOut> err = errorResult.Cast<TOut>();
                return Task.FromResult(err);
            default:
                throw new ArgumentOutOfRangeException(nameof(result), message: "Invalid result type");
        }
    }

    public static async Task<IResult<TOut>> FlatMap<TIn, TOut>(
        this Task<IResult<TIn>> result,
        Func<TIn, Task<IResult<TOut>>> mapper)
    {
        IResult<TIn> awaitedResult = await result.ConfigureAwait(continueOnCapturedContext: false);
        return await awaitedResult.FlatMap(mapper).ConfigureAwait(continueOnCapturedContext: false);
    }

    public static void Action<TIn>(this IResult<TIn> result, Action<TIn> onSuccess, Action<IErrorResult>? onError = null)
    {
        switch (result)
        {
            case SuccessResult<TIn> successResult:
                onSuccess(successResult.Data);
                break;
            case IErrorResult<TIn> errorResult:
                onError?.Invoke(errorResult);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), message: "Invalid result type");
        }
    }

    public static async Task Action<TIn>(
        this Task<IResult<TIn>> result,
        Action<TIn> onSuccess,
        Action<IErrorResult>? onError = null)
    {
        IResult<TIn> awaitedResult = await result.ConfigureAwait(continueOnCapturedContext: false);
        switch (awaitedResult)
        {
            case SuccessResult<TIn> successResult:
                onSuccess(successResult.Data);
                break;
            case IErrorResult<TIn> errorResult:
                onError?.Invoke(errorResult);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), message: "Invalid result type");
        }
    }

    public static T AssertSuccess<T>(this IResult<T> result) => result switch
    {
        SuccessResult<T> successResult => successResult.Data,
        IErrorResult errorResult => throw new ResultAssertSuccessFailedException(errorResult),
        _ => throw new ArgumentOutOfRangeException(nameof(result), message: "Invalid result type")
    };

    public static void AssertSuccess(this IResult result)
    {
        switch (result)
        {
            case SuccessResult:
                break; // do nothing
            case IErrorResult errorResult:
                throw new ResultAssertSuccessFailedException(errorResult);
            default:
                throw new ArgumentOutOfRangeException(nameof(result), message: "Invalid result type");
        }
    }
}

public sealed class ResultAssertSuccessFailedException : Exception
{
    public ResultAssertSuccessFailedException(IErrorResult errorResult)
        : base(errorResult.Message) =>
        ErrorResult = errorResult;

    public IErrorResult ErrorResult { get; }
}
