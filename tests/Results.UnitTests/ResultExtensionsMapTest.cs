namespace Results.UnitTests;

public sealed class ResultExtensionsMapTest
{
    [Test(Description = "Should map on successful Result<T>")]
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

    [Test(Description = "Should map the error on ErrorResult<T>")]
    public void MapErrorT()
    {
        IResult<int> sut = Result.Error<TestResultError<int>, int>(message: "Message");

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

    [Test(Description = "Should map on successful Result")]
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

    [Test(Description = "Should map the error on ErrorResult")]
    public void MapError()
    {
        IResult sut = Result.Error<TestResultError>(message: "Message");

        IResult<int> actual = sut.Map(() => 1);

        switch (actual)
        {
            case SuccessResult<int>:
                Assert.Fail(message: "Should be an error");
                break;
            case TestResultError<int> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestResultError<int>");
                break;
        }
    }

    [Test(Description = "Should map on successful Result<T> with async lambda")]
    public async Task MapSuccessTAsync()
    {
        IResult<int> sut = Result.Success(data: 1);

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

    [Test(Description = "Should map the error on ErrorResult<T> with async lambda")]
    public async Task MapErrorTAsync()
    {
        IResult<int> sut = Result.Error<TestResultError<int>, int>(message: "Message");

        IResult<string> actual = await sut
            .Map(i => Task.FromResult(i.ToString(CultureInfo.InvariantCulture)))
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

    [Test(Description = "Should map on successful Task<Result<T>>")]
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

    [Test(Description = "Should map the error on Task<ErrorResult<T>>")]
    public async Task MapErrorTaskT()
    {
        Task<IResult<int>> sut = Task.FromResult(Result.Error<TestResultError<int>, int>(message: "Message"));

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
}
