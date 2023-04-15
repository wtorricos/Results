namespace Results.UnitTests;

public sealed class ResultExtensionsFlatMapTest
{
    [Test(Description = "Should flatten success Result to Result")]
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

    [Test(Description = "Should flatten ErrorResult to Result")]
    public void FlatMapError()
    {
        IResult sut = new TestResultError("Message");

        IResult actual = sut.FlatMap(Result.Success);

        switch (actual)
        {
            case TestResultError err:
                err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail("Should be error");
                break;
        }
    }

    [Test(Description = "Should flatten success Result to Result<T>")]
    public void FlatMapSuccessResultT()
    {
        IResult sut = Result.Success();

        IResult actual = sut.FlatMap(() => Result.Success(1));

        switch (actual)
        {
            case SuccessResult<int> successResult:
                _ = successResult.Data.Should().Be(1);
                break;
            case IErrorResult err:
                Assert.Fail(err.GetDisplayMessage());
                break;
        }
    }

    [Test(Description = "Should flatten ErrorResult to Result<T>")]
    public void FlatMapErrorResultToResultT()
    {
        IResult sut = new TestResultError("Message");

        IResult<int> actual = sut.FlatMap(() => Result.Success(1));

        switch (actual)
        {
            case IErrorResult err:
                err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail("Should be an error");
                break;
        }
    }

    [Test(Description = "Should flatten success Result<T> to Result<T>")]
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

    [Test(Description = "Should flatten an ErrorResult<T> to Result<T>")]
    public void FlatMapErrorResultTToResultT()
    {
        IResult<int> sut = Result.Error<TestResultError<int>, int>(message: "Message");

        IResult<string> actual = sut.FlatMap(i => Result.Success(i.ToString(CultureInfo.InvariantCulture)));

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

    [Test(Description = "Should flatten successful Result<T> with async lambda")]
    public async Task FlatMapSuccessResultTAsync()
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

    [Test(Description = "Should flatten ErrorResult<T> with async lambda")]
    public async Task FlatMapErrorResultTAsync()
    {
        IResult<int> sut = Result.Error<TestResultError<int>, int>(message: "Message");

        IResult<string> actual = await sut
            .FlatMap(i => Task.FromResult(Result.Success(i.ToString(CultureInfo.InvariantCulture))))
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

    [Test(Description = "Should flatten Task<Result<T>> with async lambda")]
    public async Task FlatMapTaskResultTAsync()
    {
        Task<IResult<int>> sut = Task.FromResult(Result.Success(1));

        IResult<string> actual = await sut
            .FlatMap(i => Task.FromResult(Result.Success(i.ToString(CultureInfo.InvariantCulture))))
            .ConfigureAwait(continueOnCapturedContext: false);

        switch (actual)
        {
            case SuccessResult<string> success:
                success.Data.Should().Be("1");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestResultError<string>");
                break;
        }
    }

    [Test(Description = "Should flatten Task<ErrorResult<T>> with async lambda")]
    public async Task FlatMapTaskErrorResultTAsync()
    {
        Task<IResult<int>> sut = Task.FromResult(Result.Error<TestResultError<int>, int>("Message"));

        IResult<string> actual = await sut
            .FlatMap(i => Task.FromResult(Result.Success(i.ToString(CultureInfo.InvariantCulture))))
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
}
