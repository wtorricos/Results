namespace Results.UnitTests;

public sealed class ResultExtensionsFlatMapTest
{
    [Test(Description = "Should flatten success IResult to IResult")]
    public void FlatMapSuccess()
    {
        IResult sut = Result.Success();

        IResult actual = sut.FlatMap(Result.Success);

        switch (actual)
        {
            case SuccessResult<string> successResult:
                _ = successResult.Data.Should().Be(expected: "1");
                break;
            case IErrorResult err:
                Assert.Fail(err.GetDisplayMessage());
                break;
        }
    }

    [Test(Description = "Should flatten error IResult to IResult")]
    public void FlatMapError()
    {
        IResult sut = TestResultError.Create(message: "Message");

        IResult actual = sut.FlatMap(Result.Success);

        switch (actual)
        {
            case TestResultError err:
                err.Message.Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be error");
                break;
        }
    }

    [Test(Description = "Should flatten success IResult to IResult<T>")]
    public void FlatMapSuccessToResultT()
    {
        IResult sut = Result.Success();

        IResult actual = sut.FlatMap(() => Result.Success(data: 1));

        switch (actual)
        {
            case SuccessResult<int> successResult:
                _ = successResult.Data.Should().Be(expected: 1);
                break;
            case IErrorResult err:
                Assert.Fail(err.GetDisplayMessage());
                break;
        }
    }

    [Test(Description = "Should flatten error IResult to IResult<T>")]
    public void FlatMapErrorToResultT()
    {
        IResult sut = TestResultError.Create("Message");

        IResult actual = sut.FlatMap(() => Result.Success(data: 1));

        switch (actual)
        {
            case TestResultError<int> err:
                err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail("Should be TestResultError<int>");
                break;
        }
    }

    [Test(Description = "Should flatten success IResult to Task<IResult<T>>")]
    public async Task FlatMapSuccessToTaskResultT()
    {
        IResult sut = Result.Success();

        IResult actual = await sut.FlatMap(() => Task.FromResult(Result.Success(data: 1)));

        switch (actual)
        {
            case SuccessResult<int> successResult:
                _ = successResult.Data.Should().Be(expected: 1);
                break;
            case IErrorResult err:
                Assert.Fail(err.GetDisplayMessage());
                break;
        }
    }

    [Test(Description = "Should flatten error IResult to Task<IResult<T>>")]
    public async Task FlatMapErrorToTaskResultT()
    {
        IResult sut = TestResultError.Create("Message");

        IResult actual = await sut.FlatMap(() => Task.FromResult(Result.Success(data: 1)));

        switch (actual)
        {
            case TestResultError<int> err:
                err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail("Should be TestResultError<int>");
                break;
        }
    }

    [Test(Description = "Should flatten success Task<IResult> to Task<IResult>")]
    public async Task FlatMapSuccessTaskResultToTaskResult()
    {
        Task<IResult> sut = Task.FromResult(Result.Success());

        IResult actual = await sut.FlatMap(Result.Success);

        switch (actual)
        {
            case SuccessResult:
                break; // continue
            default:
                Assert.Fail("Should be success");
                break;
        }
    }

    [Test(Description = "Should flatten error Task<IResult> to Task<IResult>")]
    public async Task FlatMapErrorTaskResultToTaskResult()
    {
        Task<IResult> sut = Task.FromResult(TestResultError.Create("Message"));

        IResult actual = await sut.FlatMap(Result.Success);

        switch (actual)
        {
            case TestResultError err:
                err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail("Should be TestResultError");
                break;
        }
    }

    [Test(Description = "Should flatten success Task<IResult> to IResult<T>")]
    public async Task FlatMapSuccessTaskResultToResultT()
    {
        Task<IResult> sut = Task.FromResult(Result.Success());

        IResult<int> actual = await sut.FlatMap(() => Result.Success(data: 1));

        switch (actual)
        {
            case SuccessResult<int> successResult:
                successResult.Data.Should().Be(1);
                break;
            default:
                Assert.Fail("Should be SuccessResult<int>");
                break;
        }
    }

    [Test(Description = "Should flatten error Task<IResult> to IResult<T>")]
    public async Task FlatMapErrorTaskResultToResultT()
    {
        Task<IResult> sut = Task.FromResult(TestResultError.Create("Message"));

        IResult<int> actual = await sut.FlatMap(() => Result.Success(data: 1));

        switch (actual)
        {
            case TestResultError<int> err:
                err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail("Should be TestResultError<int>");
                break;
        }
    }

    [Test(Description = "Should flatten success Task<IResult> to Task<Result<T>>")]
    public async Task FlatMapSuccessTaskResultToSuccessTaskResultT()
    {
        Task<IResult> sut = Task.FromResult(Result.Success());

        IResult<int> actual = await sut.FlatMap(() => Task.FromResult(Result.Success(data: 1)));

        switch (actual)
        {
            case SuccessResult<int> successResult:
                successResult.Data.Should().Be(1);
                break;
            default:
                Assert.Fail("Should be SuccessResult<int>");
                break;
        }
    }

    [Test(Description = "Should flatten error Task<IResult> to Task<IResult<T>>")]
    public async Task FlatMapErrorTaskResultToTaskResultT()
    {
        Task<IResult> sut = Task.FromResult(TestResultError.Create("Message"));

        IResult<int> actual = await sut.FlatMap(() => Task.FromResult(Result.Success(data: 1)));

        switch (actual)
        {
            case TestResultError<int> err:
                err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail("Should be TestResultError<int>");
                break;
        }
    }

    [Test(Description = "Should flatten success IResult<T> to IResult<T>")]
    public void FlatMapSuccessResultTToResultT()
    {
        IResult<int> sut = Result.Success(data: 1);

        IResult<string> actual = sut.FlatMap(i => Result.Success(i.ToString(CultureInfo.InvariantCulture)));

        switch (actual)
        {
            case SuccessResult<string> successResult:
                _ = successResult.Data.Should().Be(expected: "1");
                break;
            case IErrorResult err:
                Assert.Fail(err.GetDisplayMessage());
                break;
        }
    }

    [Test(Description = "Should flatten error IResult<T> to Result<T>")]
    public void FlatMapErrorResultTToResultT()
    {
        IResult<int> sut = TestResultError<int>.Create(message: "Message");

        IResult<string> actual = sut.FlatMap(i => Result.Success(i.ToString(CultureInfo.InvariantCulture)));

        switch (actual)
        {
            case TestResultError<string> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestResultError<string>");
                break;
        }
    }

    [Test(Description = "Should flatten successful IResult<T> with Func<T, Task<IResult<TOut>>")]
    public async Task FlatMapSuccessResultTWithAsyncLambda()
    {
        IResult<int> sut = Result.Success(data: 1);

        IResult<string> actual = await sut
            .FlatMap(i => Task.FromResult(Result.Success(i.ToString(CultureInfo.InvariantCulture))))
            .ConfigureAwait(continueOnCapturedContext: false);

        switch (actual)
        {
            case SuccessResult<string> successResult:
                _ = successResult.Data.Should().Be(expected: "1");
                break;
            case IErrorResult err:
                Assert.Fail(err.GetDisplayMessage());
                break;
        }
    }

    [Test(Description = "Should flatten error IResult<T> with Func<T, Task<IResult<TOut>>")]
    public async Task FlatMapErrorResultTWithAsyncLambda()
    {
        IResult<int> sut = TestResultError<int>.Create(message: "Message");

        IResult<string> actual = await sut
            .FlatMap(i => Task.FromResult(Result.Success(i.ToString(CultureInfo.InvariantCulture))))
            .ConfigureAwait(continueOnCapturedContext: false);

        switch (actual)
        {
            case TestResultError<string> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestResultError<string>");
                break;
        }
    }

    [Test(Description = "Should flatten success Task<IResult<T>> with Func<T, IResult<TOut>>")]
    public async Task FlatMapSuccessTaskResultTWithSyncLambda()
    {
        Task<IResult<int>> sut = Task.FromResult(Result.Success(data: 1));

        IResult<string> actual = await sut
            .FlatMap(i => Result.Success(i.ToString(CultureInfo.InvariantCulture)))
            .ConfigureAwait(continueOnCapturedContext: false);

        switch (actual)
        {
            case SuccessResult<string> success:
                success.Data.Should().Be(expected: "1");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestResultError<string>");
                break;
        }
    }

    [Test(Description = "Should flatten error Task<IResult<T>> with Func<T, IResult<TOut>>")]
    public async Task FlatMapErrorTaskResultTWithSyncLambda()
    {
        Task<IResult<int>> sut = Task.FromResult(TestResultError<int>.Create(message: "Message"));

        IResult<string> actual = await sut
            .FlatMap(i => Result.Success(i.ToString(CultureInfo.InvariantCulture)))
            .ConfigureAwait(continueOnCapturedContext: false);

        switch (actual)
        {
            case TestResultError<string> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestResultError<string>");
                break;
        }
    }

    [Test(Description = "Should flatten success Task<IResult<T>> with Func<T, Task<IResult<TOut>>>")]
    public async Task FlatMapTSuccessTaskResultTWithAsyncLambda()
    {
        Task<IResult<int>> sut = Task.FromResult(Result.Success(data: 1));

        IResult<string> actual = await sut
            .FlatMap(i => Task.FromResult(Result.Success(i.ToString(CultureInfo.InvariantCulture))))
            .ConfigureAwait(continueOnCapturedContext: false);

        switch (actual)
        {
            case SuccessResult<string> success:
                success.Data.Should().Be(expected: "1");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestResultError<string>");
                break;
        }
    }

    [Test(Description = "Should flatten error Task<IResult<T>> with Func<T, Task<IResult<TOut>>>")]
    public async Task FlatMapErrorTaskResultTWithAsyncLambda()
    {
        Task<IResult<int>> sut = Task.FromResult(TestResultError<int>.Create(message: "Message"));

        IResult<string> actual = await sut
            .FlatMap(i => Task.FromResult(Result.Success(i.ToString(CultureInfo.InvariantCulture))))
            .ConfigureAwait(continueOnCapturedContext: false);

        switch (actual)
        {
            case TestResultError<string> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestResultError<string>");
                break;
        }
    }
}
