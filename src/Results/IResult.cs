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
}
