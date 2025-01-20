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
            secondMaybe(first).Map(second => resultSelector(arg1: first, arg2: second)));

    public static Task<IMaybe<TResult>> SelectMany<TFirst, TSecond, TResult>(
        this Task<IMaybe<TFirst>> firstMaybe,
        Func<TFirst, Task<IMaybe<TSecond>>> secondMaybe,
        Func<TFirst, TSecond, TResult> resultSelector) => firstMaybe.FlatMap(
        first =>
            secondMaybe(first).Map(second => resultSelector(arg1: first, arg2: second)));

    public static Task<IMaybe<TResult>> SelectMany<TFirst, TSecond, TResult>(
        this IMaybe<TFirst> firstMaybe,
        Func<TFirst, Task<IMaybe<TSecond>>> secondMaybe,
        Func<TFirst, TSecond, TResult> resultSelector) => firstMaybe.FlatMap(
        first =>
            secondMaybe(first).Map(second => resultSelector(arg1: first, arg2: second)));

    public static Task<IMaybe<TResult>> SelectMany<TFirst, TSecond, TResult>(
        this Task<IMaybe<TFirst>> firstMaybe,
        Func<TFirst, IMaybe<TSecond>> secondMaybe,
        Func<TFirst, TSecond, TResult> resultSelector) => firstMaybe.FlatMap(
        first =>
            secondMaybe(first).Map(second => resultSelector(arg1: first, arg2: second)));

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

internal sealed record PredicateFailedError : INone
{
    public PredicateFailedError(string message, IEnumerable<NoneDetail> details)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Details = details?.ToList() ?? throw new ArgumentNullException(nameof(details));
    }
    public PredicateFailedError(string message) : this(message, Array.Empty<NoneDetail>())
    {
    }

    public static IMaybe Create(string message, IEnumerable<NoneDetail> details)
    {
        return new PredicateFailedError(message, details);
    }

    public static IMaybe Create(string message)
    {
        return new PredicateFailedError(message);
    }

    public string Message { get; }

    public IReadOnlyCollection<NoneDetail> Details { get; }

    public string GetDisplayMessage()
    {
        return Message;
    }

    public IMaybe<TOut> Cast<TOut>()
    {
        return PredicateFailedError<TOut>.Create(Message, Details);
    }
}

public sealed record PredicateFailedError<T> : INone<T>
{
    public PredicateFailedError(string message, IEnumerable<NoneDetail> details)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Details = details?.ToList() ?? throw new ArgumentNullException(nameof(details));
    }
    public PredicateFailedError(string message) : this(message, Array.Empty<NoneDetail>())
    {
    }

    public static IMaybe<T> Create(string message, IEnumerable<NoneDetail> details)
    {
        return new PredicateFailedError<T>(message, details);
    }

    public static IMaybe<T> Create(string message)
    {
        return new PredicateFailedError<T>(message);
    }

    public string Message { get; }

    public IReadOnlyCollection<NoneDetail> Details { get; }

    public string GetDisplayMessage()
    {
        return Message;
    }

    public IMaybe<TOut> Cast<TOut>()
    {
        return PredicateFailedError<TOut>.Create(Message, Details);
    }

    public IMaybe ToMaybe()
    {
        return PredicateFailedError.Create(Message, Details);
    }

    public IMaybe<TResult> Match<TResult>(Func<T, TResult> onSome, Func<INone, IMaybe<TResult>> onNone)
    {
        return onNone(this);
    }

    public Task<IMaybe<TResult>> Match<TResult>(Func<T, Task<TResult>> onSome, Func<INone, IMaybe<TResult>> onNone)
    {
        return Task.FromResult(onNone(this));
    }
}
