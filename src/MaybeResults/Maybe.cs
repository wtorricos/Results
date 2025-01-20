namespace MaybeResults;

public interface IMaybe;

public interface IMaybe<out T> : IMaybe
{
    IMaybe ToMaybe();

    IMaybe<TResult> Match<TResult>(Func<T, TResult> onSome, Func<INone, IMaybe<TResult>> onNone);

    Task<IMaybe<TResult>> Match<TResult>(Func<T, Task<TResult>> onSome, Func<INone, IMaybe<TResult>> onNone);
}

public static class Maybe
{
    public static IMaybe Create() => Some.Instance;

    public static IMaybe<T> Create<T>(T value) => new Some<T>(value);
}

public sealed record Some : IMaybe
{
    public static readonly Some Instance = new();

    Some() { }
}

public sealed record Some<T>(T Value) : IMaybe<T>
{
    public T Value { get; } = Value;

    public IMaybe ToMaybe() => Some.Instance;

    public IMaybe<TResult> Match<TResult>(Func<T, TResult> onSome, Func<INone, IMaybe<TResult>> onNone) =>
        Maybe.Create(onSome(Value));

    public async Task<IMaybe<TResult>> Match<TResult>(Func<T, Task<TResult>> onSome, Func<INone, IMaybe<TResult>> onNone) =>
        Maybe.Create(await onSome(Value).ConfigureAwait(false));
}

public interface INone : IMaybe
{
    string Message { get; }

    IReadOnlyCollection<NoneDetail> Details { get; }

    string GetDisplayMessage();

    IMaybe<TOut> Cast<TOut>();
}

public interface INone<out T> : INone, IMaybe<T>;

public sealed record NoneDetail(string Code, string Description)
{
    public string Code { get; } = Code;

    public string Description { get; } = Description;
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class NoneAttribute : Attribute;
