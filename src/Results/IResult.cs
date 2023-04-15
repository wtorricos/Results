namespace Results;

public interface IResult
{
}

public interface IResult<T> : IResult
{
    IResult ToResult();
}

public abstract record Result
{
    public static IResult Success() => SuccessResult.Instance;

    public static IResult<T> Success<T>(T data) => new SuccessResult<T>(data);

    public static IResult Error<TError>(string message)
        where TError : IErrorResult =>
        (IErrorResult)Activator.CreateInstance(typeof(TError), message, Array.Empty<ErrorResultDetail>())!;

    public static IResult Error<TError>(string message, IEnumerable<ErrorResultDetail> errors)
        where TError : IErrorResult =>
        (IErrorResult)Activator.CreateInstance(typeof(TError), message, errors)!;

    public static IResult<T> Error<TError, T>(string message)
        where TError : IErrorResult<T> =>
        (IErrorResult<T>)Activator.CreateInstance(typeof(TError), message, Array.Empty<ErrorResultDetail>())!;

    public static IResult<T> Error<TError, T>(string message, IEnumerable<ErrorResultDetail> errors)
        where TError : IErrorResult<T> =>
        (IErrorResult<T>)Activator.CreateInstance(typeof(TError), message, errors)!;
}
