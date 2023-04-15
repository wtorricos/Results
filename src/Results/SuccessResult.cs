namespace Results;

public sealed record SuccessResult : IResult
{
    public static readonly SuccessResult Instance = new();

    SuccessResult() { }
}

public sealed record SuccessResult<T>(T Data) : IResult<T>
{
    public T Data { get; } = Data;

    public IResult ToResult() => SuccessResult.Instance;
}
