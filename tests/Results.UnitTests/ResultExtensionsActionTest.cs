namespace Results.UnitTests;

public sealed class ResultExtensionsActionTest
{
    [Test(Description = "Action should execute success lambda on successful Result<T>")]
    public void ActionSuccessT()
    {
        IResult<int> sut = Result.Success(data: 1);

        string stringResult = string.Empty;
        sut.Action(
            i => stringResult = i.ToString(CultureInfo.InvariantCulture),
            err => stringResult = err.GetDisplayMessage());

        _ = stringResult.Should().Be(expected: "1");
    }

    [Test(Description = "Action should execute error lambda on ErrorResult<T>")]
    public void ActionErrorT()
    {
        IResult<int> sut = TestResultError<int>.Create(message: "Error");

        string stringResult = string.Empty;
        sut.Action(
            i => stringResult = i.ToString(CultureInfo.InvariantCulture),
            err => stringResult = err.GetDisplayMessage());

        _ = stringResult.Should().Be(expected: "Error");
    }

    [Test(Description = "Action should not do anything with no error handler on ErrorResult<T>")]
    public void ActionErrorTDefault()
    {
        IResult<int> sut = TestResultError<int>.Create(message: "Error");

        string stringResult = string.Empty;
        sut.Action(
            i => stringResult = i.ToString(CultureInfo.InvariantCulture));

        _ = stringResult.Should().Be(string.Empty);
    }

    [Test(Description = "Action should execute success lambda on successful Task<Result<T>>")]
    public async Task ActionSuccessTaskT()
    {
        Task<IResult<int>> sut = Task.FromResult(Result.Success(data: 1));

        string stringResult = string.Empty;
        await sut
            .Action(
                i => stringResult = i.ToString(CultureInfo.InvariantCulture),
                err => stringResult = err.GetDisplayMessage())
            .ConfigureAwait(continueOnCapturedContext: false);

        _ = stringResult.Should().Be(expected: "1");
    }

    [Test(Description = "Action should execute error lambda on Task<ErrorResult<T>>")]
    public async Task ActionErrorTaskT()
    {
        Task<IResult<int>> sut = Task.FromResult(TestResultError<int>.Create(message: "Error"));

        string stringResult = string.Empty;
        await sut
            .Action(
                i => stringResult = i.ToString(CultureInfo.InvariantCulture),
                err => stringResult = err.GetDisplayMessage())
            .ConfigureAwait(continueOnCapturedContext: false);

        _ = stringResult.Should().Be(expected: "Error");
    }

    [Test(Description = "Action should not do anything with no error handler on Task<ErrorResult<T>>")]
    public async Task ActionErrorTaskTDefault()
    {
        Task<IResult<int>> sut = Task.FromResult(TestResultError<int>.Create(message: "Error"));

        string stringResult = string.Empty;
        await sut
            .Action(i => stringResult = i.ToString(CultureInfo.InvariantCulture))
            .ConfigureAwait(continueOnCapturedContext: false);

        _ = stringResult.Should().Be(string.Empty);
    }
}
