namespace MaybeResults.UnitTests;

public sealed class MaybeExtensionsFlatMapTest
{
    [Fact(DisplayName = "Should flatten success IMaybe to IMaybe")]
    public void FlatMapSuccess()
    {
        IMaybe sut = Maybe.Create();

        IMaybe actual = sut.FlatMap(Maybe.Create);

        switch (actual)
        {
            case Some:
                break;
            case INone err:
                Assert.Fail(err.GetDisplayMessage());
                break;
            default:
                Assert.Fail("Invalid case");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten error IMaybe to IMaybe")]
    public void FlatMapError()
    {
        IMaybe sut = TestError.Create(message: "Message");

        IMaybe actual = sut.FlatMap(Maybe.Create);

        switch (actual)
        {
            case TestError err:
                _ = err.Message.Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be error");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten success IMaybe to Task<IMaybe>")]
    public async Task FlatMapSuccessToTask()
    {
        IMaybe sut = Maybe.Create();

        IMaybe actual = await sut.FlatMap(() => Task.FromResult(Maybe.Create()));

        switch (actual)
        {
            case Some:
                break;
            case INone err:
                Assert.Fail(err.GetDisplayMessage());
                break;
            default:
                Assert.Fail("Invalid case");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten error IMaybe to Task<IMaybe>")]
    public async Task FlatMapErrorToTask()
    {
        IMaybe sut = TestError.Create(message: "Message");

        IMaybe actual = await sut.FlatMap(() => Task.FromResult(Maybe.Create()));

        switch (actual)
        {
            case TestError err:
                _ = err.Message.Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be error");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten success IMaybe to IMaybe<T>")]
    public void FlatMapSuccessToResultT()
    {
        IMaybe sut = Maybe.Create();

        IMaybe actual = sut.FlatMap(() => Maybe.Create(value: 1));

        switch (actual)
        {
            case Some<int> some:
                _ = some.Value.Should().Be(expected: 1);
                break;
            case INone err:
                Assert.Fail(err.GetDisplayMessage());
                break;
            default:
                Assert.Fail("Invalid case");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten error IMaybe to IMaybe<T>")]
    public void FlatMapErrorToResultT()
    {
        IMaybe sut = TestError.Create("Message");

        IMaybe<int> actual = sut.FlatMap(() => Maybe.Create(value: 1));

        switch (actual)
        {
            case TestError<int> err:
                _ = err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail("Should be TestError<int>");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten success IMaybe to Task<IMaybe<T>>")]
    public async Task FlatMapSuccessToTaskResultT()
    {
        IMaybe sut = Maybe.Create();

        IMaybe actual = await sut.FlatMap(() => Task.FromResult(Maybe.Create(value: 1)));

        switch (actual)
        {
            case Some<int> some:
                _ = some.Value.Should().Be(expected: 1);
                break;
            case INone err:
                Assert.Fail(err.GetDisplayMessage());
                break;
            default:
                Assert.Fail("Invalid case");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten error IMaybe to Task<IMaybe<T>>")]
    public async Task FlatMapErrorToTaskResultT()
    {
        IMaybe sut = TestError.Create("Message");

        IMaybe<int> actual = await sut.FlatMap(() => Task.FromResult(Maybe.Create(value: 1)));

        switch (actual)
        {
            case TestError<int> err:
                _ = err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail("Should be TestError<int>");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten success Task<IMaybe> to Task<IMaybe>")]
    public async Task FlatMapSuccessTaskResultToTaskResult()
    {
        Task<IMaybe> sut = Task.FromResult(Maybe.Create());

        IMaybe actual = await sut.FlatMap(Maybe.Create);

        switch (actual)
        {
            case Some:
                break; // continue
            default:
                Assert.Fail("Should be success");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten error Task<IMaybe> to Task<IMaybe>")]
    public async Task FlatMapErrorTaskResultToTaskResult()
    {
        Task<IMaybe> sut = Task.FromResult(TestError.Create("Message"));

        IMaybe actual = await sut.FlatMap(Maybe.Create);

        switch (actual)
        {
            case TestError err:
                _ = err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail("Should be TestError");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten success Task<IMaybe> to IMaybe<T>")]
    public async Task FlatMapSuccessTaskResultToResultT()
    {
        Task<IMaybe> sut = Task.FromResult(Maybe.Create());

        IMaybe<int> actual = await sut.FlatMap(() => Maybe.Create(value: 1));

        switch (actual)
        {
            case Some<int> some:
                _ = some.Value.Should().Be(1);
                break;
            default:
                Assert.Fail("Should be Some<int>");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten error Task<IMaybe> to IMaybe<T>")]
    public async Task FlatMapErrorTaskResultToResultT()
    {
        Task<IMaybe> sut = Task.FromResult(TestError.Create("Message"));

        IMaybe<int> actual = await sut.FlatMap(() => Maybe.Create(value: 1));

        switch (actual)
        {
            case TestError<int> err:
                _ = err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail("Should be TestError<int>");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten success Task<IMaybe> to Task<Result<T>>")]
    public async Task FlatMapSuccessTaskResultToSuccessTaskResultT()
    {
        Task<IMaybe> sut = Task.FromResult(Maybe.Create());

        IMaybe<int> actual = await sut.FlatMap(() => Task.FromResult(Maybe.Create(value: 1)));

        switch (actual)
        {
            case Some<int> some:
                _ = some.Value.Should().Be(1);
                break;
            default:
                Assert.Fail("Should be Some<int>");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten error Task<IMaybe> to Task<IMaybe<T>>")]
    public async Task FlatMapErrorTaskResultToTaskResultT()
    {
        Task<IMaybe> sut = Task.FromResult(TestError.Create("Message"));

        IMaybe<int> actual = await sut.FlatMap(() => Task.FromResult(Maybe.Create(value: 1)));

        switch (actual)
        {
            case TestError<int> err:
                _ = err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail("Should be TestError<int>");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten success IMaybe<T> to IMaybe<T>")]
    public void FlatMapSomeTToResultT()
    {
        IMaybe<int> sut = Maybe.Create(value: 1);

        IMaybe<string> actual = sut.FlatMap(i => Maybe.Create(i.ToString(CultureInfo.InvariantCulture)));

        switch (actual)
        {
            case Some<string> some:
                _ = some.Value.Should().Be(expected: "1");
                break;
            case INone err:
                Assert.Fail(err.GetDisplayMessage());
                break;
            default:
                Assert.Fail("Invalid case");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten error IMaybe<T> to Result<T>")]
    public void FlatMapErrorResultTToResultT()
    {
        IMaybe<int> sut = TestError<int>.Create(message: "Message");

        IMaybe<string> actual = sut.FlatMap(i => Maybe.Create(i.ToString(CultureInfo.InvariantCulture)));

        switch (actual)
        {
            case TestError<string> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestError<string>");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten successful IMaybe<T> with Func<T, Task<IMaybe<TOut>>")]
    public async Task FlatMapSomeTWithAsyncLambda()
    {
        IMaybe<int> sut = Maybe.Create(value: 1);

        IMaybe<string> actual = await sut
            .FlatMap(i => Task.FromResult(Maybe.Create(i.ToString(CultureInfo.InvariantCulture))));

        switch (actual)
        {
            case Some<string> some:
                _ = some.Value.Should().Be(expected: "1");
                break;
            case INone err:
                Assert.Fail(err.GetDisplayMessage());
                break;
            default:
                Assert.Fail("Invalid case");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten error IMaybe<T> with Func<T, Task<IMaybe<TOut>>")]
    public async Task FlatMapErrorResultTWithAsyncLambda()
    {
        IMaybe<int> sut = TestError<int>.Create(message: "Message");

        IMaybe<string> actual = await sut
            .FlatMap(i => Task.FromResult(Maybe.Create(i.ToString(CultureInfo.InvariantCulture))));

        switch (actual)
        {
            case TestError<string> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestError<string>");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten success Task<IMaybe<T>> with Func<T, IMaybe<TOut>>")]
    public async Task FlatMapSuccessTaskResultTWithSyncLambda()
    {
        Task<IMaybe<int>> sut = Task.FromResult(Maybe.Create(value: 1));

        IMaybe<string> actual = await sut
            .FlatMap(i => Maybe.Create(i.ToString(CultureInfo.InvariantCulture)));

        switch (actual)
        {
            case Some<string> success:
                _ = success.Value.Should().Be(expected: "1");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestError<string>");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten error Task<IMaybe<T>> with Func<T, IMaybe<TOut>>")]
    public async Task FlatMapErrorTaskResultTWithSyncLambda()
    {
        Task<IMaybe<int>> sut = Task.FromResult(TestError<int>.Create(message: "Message"));

        IMaybe<string> actual = await sut
            .FlatMap(i => Maybe.Create(i.ToString(CultureInfo.InvariantCulture)));

        switch (actual)
        {
            case TestError<string> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestError<string>");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten success Task<IMaybe<T>> with Func<T, Task<IMaybe<TOut>>>")]
    public async Task FlatMapTSuccessTaskResultTWithAsyncLambda()
    {
        Task<IMaybe<int>> sut = Task.FromResult(Maybe.Create(value: 1));

        IMaybe<string> actual = await sut
            .FlatMap(i => Task.FromResult(Maybe.Create(i.ToString(CultureInfo.InvariantCulture))));

        switch (actual)
        {
            case Some<string> success:
                _ = success.Value.Should().Be(expected: "1");
                break;
            default:
                Assert.Fail(message: "Should be successful");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten error Task<IMaybe<T>> with Func<T, Task<IMaybe<TOut>>>")]
    public async Task FlatMapErrorTaskResultTWithAsyncLambda()
    {
        Task<IMaybe<int>> sut = Task.FromResult(TestError<int>.Create(message: "Message"));

        IMaybe<string> actual = await sut
            .FlatMap(i => Task.FromResult(Maybe.Create(i.ToString(CultureInfo.InvariantCulture))));

        switch (actual)
        {
            case TestError<string> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestError<string>");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten success Task<IMaybe<T>> with Func<T, Task<IMaybe>>")]
    public async Task FlatMapTSuccessTaskResultWithMaybeAsyncLambda()
    {
        Task<IMaybe<int>> sut = Task.FromResult(Maybe.Create(value: 1));

        IMaybe actual = await sut.FlatMap(_ => Task.FromResult(Some.Instance as IMaybe));

        switch (actual)
        {
            case Some success:
                _ = success.Should().NotBeNull();
                break;
            default:
                Assert.Fail(message: "Should be successful");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten error Task<IMaybe<T>> with Func<T, Task<IMaybe>>")]
    public async Task FlatMapTErrorTaskResultWithMaybeAsyncLambda()
    {
        Task<IMaybe<int>> sut = Task.FromResult(TestError<int>.Create(message: "Message"));

        IMaybe actual = await sut.FlatMap(_ => Task.FromResult(Some.Instance as IMaybe));

        switch (actual)
        {
            case Some:
                Assert.Fail("Should be an error of type TestError<string>");
                break;
            case INone err:
                err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestError<string>");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten success Task<IMaybe<T>> with Func<T, IMaybe>")]
    public async Task FlatMapTSuccessResultWithMaybeAsyncLambda()
    {
        Task<IMaybe<int>> sut = Task.FromResult(Maybe.Create(value: 1));

        IMaybe actual = await sut.FlatMap(_ => Some.Instance);

        switch (actual)
        {
            case Some success:
                _ = success.Should().NotBeNull();
                break;
            default:
                Assert.Fail(message: "Should be successful");
                break;
        }
    }

    [Fact(DisplayName = "Should flatten error Task<IMaybe<T>> with Func<T, IMaybe>")]
    public async Task FlatMapTErrorResultWithMaybeAsyncLambda()
    {
        Task<IMaybe<int>> sut = Task.FromResult(TestError<int>.Create(message: "Message"));

        IMaybe actual = await sut.FlatMap(_ => Some.Instance);

        switch (actual)
        {
            case Some:
                Assert.Fail("Should be an error of type TestError<string>");
                break;
            case INone err:
                err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestError<string>");
                break;
        }
    }
}
