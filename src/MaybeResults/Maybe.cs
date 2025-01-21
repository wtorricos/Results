namespace MaybeResults;

public interface IMaybe
{
    IMaybe Match(Func<IMaybe> onSome, Action<INone>? onNone = null);

    IMaybe<TResult> Match<TResult>(Func<TResult> onSome, Action<INone>? onNone = null);

    Task<IMaybe> Match(Func<Task<IMaybe>> onSome, Action<INone>? onNone = null);

    Task<IMaybe<TResult>> Match<TResult>(Func<Task<TResult>> onSome, Action<INone>? onNone = null);
}

public interface IMaybe<out T> : IMaybe
{
    IMaybe ToMaybe();

    IMaybe Match(Func<T, IMaybe> onSome, Action<INone<T>>? onNone = null);

    IMaybe<TResult> Match<TResult>(Func<T, TResult> onSome, Action<INone<T>>? onNone = null);

    Task<IMaybe<TResult>> Match<TResult>(Func<T, Task<TResult>> onSome, Action<INone<T>>? onNone = null);
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

    public IMaybe Match(Func<IMaybe> onSome, Action<INone>? onNone = null) =>
        onSome();

    public IMaybe<TResult> Match<TResult>(Func<TResult> onSome, Action<INone>? onNone = null) =>
        Maybe.Create(onSome());

    public Task<IMaybe> Match(Func<Task<IMaybe>> onSome, Action<INone>? onNone = null) =>
        onSome();

    public async Task<IMaybe<TResult>> Match<TResult>(Func<Task<TResult>> onSome, Action<INone>? onNone = null) =>
        Maybe.Create(await onSome().ConfigureAwait(continueOnCapturedContext: false));
}

public sealed record Some<T>(T Value) : IMaybe<T>
{
    public T Value { get; } = Value;

    public IMaybe ToMaybe() =>
        Some.Instance;

    public IMaybe Match(Func<T, IMaybe> onSome, Action<INone<T>>? onNone = null) =>
        onSome(Value);

    public IMaybe Match(Func<IMaybe> onSome, Action<INone>? onNone = null) =>
        onSome();

    public IMaybe<TResult> Match<TResult>(Func<TResult> onSome, Action<INone>? onNone = null) =>
        Maybe.Create(onSome());

    public Task<IMaybe> Match(Func<Task<IMaybe>> onSome, Action<INone>? onNone = null) =>
        onSome();

    public async Task<IMaybe<TResult>> Match<TResult>(Func<Task<TResult>> onSome, Action<INone>? onNone = null) =>
        Maybe.Create(await onSome().ConfigureAwait(continueOnCapturedContext: false));

    public IMaybe<TResult> Match<TResult>(Func<T, TResult> onSome, Action<INone<T>>? onNone) =>
        Maybe.Create(onSome(Value));

    public async Task<IMaybe<TResult>> Match<TResult>(Func<T, Task<TResult>> onSome, Action<INone<T>>? onNone) =>
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
