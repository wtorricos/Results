namespace Results.UnitTests;

public sealed class ResultExtensionsMapTest
{
    [Test(Description = "Should map on successful IResult with Func<TOut>")]
    public void MapSuccess()
    {
        IResult sut = Result.Success();

        IResult<int> actual = sut.Map(() => 1);

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

    [Test(Description = "Should map error IResult with Func<TOut>")]
    public void MapError()
    {
        IResult sut = TestResultError.Create(message: "Message");

        IResult<int> actual = sut.Map(() => 1);

        switch (actual)
        {
            case TestResultError<int> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestResultError<int>");
                break;
        }
    }

    [Test(Description = "Should map successful IResult<T> with Func<TOut>")]
    public void MapSuccessT()
    {
        IResult<int> sut = Result.Success(data: 1);

        IResult<string> actual = sut.Map(i => i.ToString(CultureInfo.InvariantCulture));

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

    [Test(Description = "Should map error IResult<T> with Func<TOut>")]
    public void MapErrorT()
    {
        IResult<int> sut = TestResultError<int>.Create(message: "Message");

        IResult<string> actual = sut.Map(i => i.ToString(CultureInfo.InvariantCulture));

        switch (actual)
        {
            case SuccessResult<string>:
                Assert.Fail(message: "Should be an error");
                break;
            case TestResultError<string> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestResultError<string>");
                break;
        }
    }

    [Test(Description = "Should map successful IResult<T> with Task<Func<TOut>>")]
    public async Task MapSuccessTWithAsyncLambda()
    {
        IResult<int> sut = Result.Success(data: 1);

        Task<IResult<string>> taskActual = sut.Map(i => Task.FromResult(i.ToString(CultureInfo.InvariantCulture)));

        IResult<string> actual = await taskActual.ConfigureAwait(continueOnCapturedContext: false);

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

    [Test(Description = "Should map error IResult<T> with Task<Func<TOut>>")]
    public async Task MapErrorTWithAsyncLambda()
    {
        IResult<int> sut = TestResultError<int>.Create(message: "Message");

        IResult<string> actual = await sut
            .Map(i => Task.FromResult(i.ToString(CultureInfo.InvariantCulture)))
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

    [Test(Description = "Should map successful Task<IResult<T>> with Func<T, TOut>")]
    public async Task MapSuccessTaskT()
    {
        Task<IResult<int>> sut = Task.FromResult(Result.Success(data: 1));

        IResult<string> actual = await sut
            .Map(i => i.ToString(CultureInfo.InvariantCulture))
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

    [Test(Description = "Should map error Task<IResult<T>> with Func<T, TOut>")]
    public async Task MapErrorTaskT()
    {
        Task<IResult<int>> sut = Task.FromResult(TestResultError<int>.Create(message: "Message"));

        IResult<string> actual = await sut
            .Map(i => i.ToString(CultureInfo.InvariantCulture))
            .ConfigureAwait(continueOnCapturedContext: false);

        switch (actual)
        {
            case SuccessResult<string>:
                Assert.Fail(message: "Should be an error");
                break;
            case TestResultError<string> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestResultError<string>");
                break;
        }
    }

    [Test(Description = "Should map successful Task<IResult<T>> with Func<T, Task<TOut>>")]
    public async Task MapSuccessTaskTWithAsyncLambda()
    {
        Task<IResult<int>> sut = Task.FromResult(Result.Success(data: 1));

        IResult<string> actual = await sut
            .Map(i => Task.FromResult(i.ToString(CultureInfo.InvariantCulture)))
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

    [Test(Description = "Should map error Task<IResult<T>> with Func<T, Task<TOut>>")]
    public async Task MapErrorTaskTToTaskResultT()
    {
        Task<IResult<int>> sut = Task.FromResult(TestResultError<int>.Create(message: "Message"));

        IResult<string> actual = await sut
            .Map(i => Task.FromResult(i.ToString(CultureInfo.InvariantCulture)))
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
