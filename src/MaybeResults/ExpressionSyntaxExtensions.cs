using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
namespace MaybeResults;

// By defining Where, Select, and SelectMany operators, linq will be able to work with our custom type.
// The compiler translates expressions using these keywords to the equivalent method calls.
// https://learn.microsoft.com/en-us/dotnet/csharp/linq/standard-query-operators/
// Additional tutorial: https://tyrrrz.me/blog/monadic-comprehension-via-linq
public static class ExpressionSyntaxExtensions
{
    public static IMaybe<TResult> Select<TSource, TResult>(
        this IMaybe<TSource> maybe,
        Func<TSource, TResult> selector) => maybe.Map(selector);

    public static Task<IMaybe<TResult>> Select<TSource, TResult>(
        this Task<IMaybe<TSource>> maybe,
        Func<TSource, TResult> selector) => maybe.Map(selector);

    public static IMaybe<TResult> SelectMany<TFirst, TSecond, TResult>(
        this IMaybe<TFirst> firstMaybe,
        Func<TFirst, IMaybe<TSecond>> secondMaybe,
        Func<TFirst, TSecond, TResult> resultSelector) => firstMaybe.FlatMap(
        first =>
            secondMaybe(first).Map(second => resultSelector(first, second)));

    public static Task<IMaybe<TResult>> SelectMany<TFirst, TSecond, TResult>(
        this Task<IMaybe<TFirst>> firstMaybe,
        Func<TFirst, Task<IMaybe<TSecond>>> secondMaybe,
        Func<TFirst, TSecond, TResult> resultSelector) => firstMaybe.FlatMap(
        first =>
            secondMaybe(first).Map(second => resultSelector(first, second)));

    public static Task<IMaybe<TResult>> SelectMany<TFirst, TSecond, TResult>(
        this IMaybe<TFirst> firstMaybe,
        Func<TFirst, Task<IMaybe<TSecond>>> secondMaybe,
        Func<TFirst, TSecond, TResult> resultSelector) => firstMaybe.FlatMap(
        first =>
            secondMaybe(first).Map(second => resultSelector(first, second)));

    public static Task<IMaybe<TResult>> SelectMany<TFirst, TSecond, TResult>(
        this Task<IMaybe<TFirst>> firstMaybe,
        Func<TFirst, IMaybe<TSecond>> secondMaybe,
        Func<TFirst, TSecond, TResult> resultSelector) => firstMaybe.FlatMap(
        first =>
            secondMaybe(first).Map(second => resultSelector(first, second)));

    public static IMaybe<T> Where<T>(this IMaybe<T> maybe, Expression<Func<T, bool>> predicate) =>
        maybe.FlatMap(result => predicate.Compile()(result)
            ? maybe
            : PredicateFailedError<T>.Create($"Predicate failed: '{predicate}'"));

    public static IMaybe<T> Where<T>(this IMaybe<T> maybe, Expression<Func<T, IMaybe<bool>>> predicate) =>
        maybe.FlatMap(result => predicate.Compile()(result)
            .FlatMap(p => p
                ? maybe
                : PredicateFailedError<T>.Create($"Predicate failed: '{predicate}'")));

    public static Task<IMaybe<T>> Where<T>(this Task<IMaybe<T>> maybe, Expression<Func<T, bool>> predicate) =>
        maybe.FlatMap(result => predicate.Compile()(result)
            ? Maybe.Create(result)
            : PredicateFailedError<T>.Create($"Predicate failed: '{predicate}'"));

    public static async Task<IMaybe<T>> Where<T>(this IMaybe<T> maybe, Expression<Func<T, Task<bool>>> predicate) =>
        await maybe.FlatMap(async result => await predicate.Compile()(result)
            ? Maybe.Create(result)
            : PredicateFailedError<T>.Create($"Predicate failed: '{predicate}'"));

    public static async Task<IMaybe<T>> Where<T>(this Task<IMaybe<T>> maybe, Expression<Func<T, Task<bool>>> predicate) =>
        await maybe.FlatMap<T, T>(async result => await predicate.Compile()(result)
            ? Maybe.Create(result)
            : PredicateFailedError<T>.Create($"Predicate failed: '{predicate}'"));
}

[ExcludeFromCodeCoverage]
public sealed record PredicateFailedError : INone
{
    public PredicateFailedError(string message, IEnumerable<NoneDetail> details)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Details = details?.ToList() ?? throw new ArgumentNullException(nameof(details));
    }

    public PredicateFailedError(string message) : this(message, details: Array.Empty<NoneDetail>())
    {
    }

    public string Message { get; }

    public IReadOnlyCollection<NoneDetail> Details { get; }

    public string GetDisplayMessage() =>
        Message;

    public IMaybe<TOut> Cast<TOut>() =>
        PredicateFailedError<TOut>.Create(Message, Details);

    public IMaybe Match(Func<IMaybe> onSome, Action<INone>? onNone = null)
    {
        onNone?.Invoke(this);
        return this;
    }

    public IMaybe<TResult> Match<TResult>(Func<TResult> onSome, Action<INone>? onNone = null)
    {
        onNone?.Invoke(this);
        return Cast<TResult>();
    }

    public Task<IMaybe> Match(Func<Task<IMaybe>> onSome, Action<INone>? onNone = null)
    {
        onNone?.Invoke(this);
        return Task.FromResult<IMaybe>(this);
    }

    public Task<IMaybe<TResult>> Match<TResult>(Func<Task<TResult>> onSome, Action<INone>? onNone = null)
    {
        onNone?.Invoke(this);
        return Task.FromResult(Cast<TResult>());
    }

    public static IMaybe Create(string message, IEnumerable<NoneDetail> details) =>
        new PredicateFailedError(message, details);

    public static IMaybe Create(string message) =>
        new PredicateFailedError(message);
}

[ExcludeFromCodeCoverage]
public sealed record PredicateFailedError<T> : INone<T>
{
    public PredicateFailedError(string message, IEnumerable<NoneDetail> details)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Details = details?.ToList() ?? throw new ArgumentNullException(nameof(details));
    }
    public PredicateFailedError(string message) : this(message, details: Array.Empty<NoneDetail>())
    {
    }

    public string Message { get; }

    public IReadOnlyCollection<NoneDetail> Details { get; }

    public string GetDisplayMessage() =>
        Message;

    public IMaybe<TOut> Cast<TOut>() =>
        PredicateFailedError<TOut>.Create(Message, Details);

    public IMaybe ToMaybe() =>
        PredicateFailedError.Create(Message, Details);

    public IMaybe Match(Func<T, IMaybe> onSome, Action<INone<T>>? onNone = null)
    {
        onNone?.Invoke(this);
        return this;
    }

    public IMaybe Match(Func<IMaybe> onSome, Action<INone>? onNone = null)
    {
        onNone?.Invoke(this);
        return this;
    }

    public IMaybe<TResult> Match<TResult>(Func<TResult> onSome, Action<INone>? onNone = null)
    {
        onNone?.Invoke(this);
        return Cast<TResult>();
    }

    public Task<IMaybe> Match(Func<Task<IMaybe>> onSome, Action<INone>? onNone = null)
    {
        onNone?.Invoke(this);
        return Task.FromResult<IMaybe>(this);
    }

    public Task<IMaybe<TResult>> Match<TResult>(Func<Task<TResult>> onSome, Action<INone>? onNone = null)
    {
        onNone?.Invoke(this);
        return Task.FromResult(Cast<TResult>());
    }

    public IMaybe<TResult> Match<TResult>(Func<T, TResult> onSome, Action<INone<T>>? onNone = null)
    {
        onNone?.Invoke(this);
        return Cast<TResult>();
    }

    public Task<IMaybe<TResult>> Match<TResult>(Func<T, Task<TResult>> onSome, Action<INone<T>>? onNone = null)
    {
        onNone?.Invoke(this);
        return Task.FromResult(Cast<TResult>());
    }

    public static IMaybe<T> Create(string message, IEnumerable<NoneDetail> details) =>
        new PredicateFailedError<T>(message, details);

    public static IMaybe<T> Create(string message) =>
        new PredicateFailedError<T>(message);
}
