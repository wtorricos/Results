namespace Results;

public interface IErrorResult : IResult
{
    string Message { get; }

    IReadOnlyCollection<ErrorResultDetail> Errors { get; }

    string GetDisplayMessage();

    IResult<TOut> Cast<TOut>();
}

public interface IErrorResult<T> : IErrorResult, IResult<T>
{
}

public sealed record ErrorResultDetail(string Code, string Details)
{
    public string Code { get; } = Code;
    public string Details { get; } = Details;
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ErrorResultAttribute : Attribute
{
}
