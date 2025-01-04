namespace MaybeResults.Test;

public sealed class MaybeExtensionsActionTest
{
    [Fact(DisplayName = "Action should execute success lambda on successful Result<T>")]
    public void ActionSuccessT()
    {
        IMaybe<int> sut = Maybe.Create(value: 1);

        string stringResult = string.Empty;
        sut.Action(
            i => stringResult = i.ToString(CultureInfo.InvariantCulture),
            err => stringResult = err.GetDisplayMessage());

        _ = stringResult.Should().Be(expected: "1");
    }

    [Fact(DisplayName = "Action should execute error lambda on ErrorResult<T>")]
    public void ActionErrorT()
    {
        IMaybe<int> sut = TestError<int>.Create(message: "Error");

        string stringResult = string.Empty;
        sut.Action(
            i => stringResult = i.ToString(CultureInfo.InvariantCulture),
            err => stringResult = err.GetDisplayMessage());

        _ = stringResult.Should().Be(expected: "Error");
    }

    [Fact(DisplayName = "Action should not do anything with no error handler on ErrorResult<T>")]
    public void ActionErrorTDefault()
    {
        IMaybe<int> sut = TestError<int>.Create(message: "Error");

        string stringResult = string.Empty;
        sut.Action(
            i => stringResult = i.ToString(CultureInfo.InvariantCulture));

        _ = stringResult.Should().Be(string.Empty);
    }

    [Fact(DisplayName = "Action should execute success lambda on successful Task<Result<T>>")]
    public async Task ActionSuccessTaskT()
    {
        Task<IMaybe<int>> sut = Task.FromResult(Maybe.Create(value: 1));

        string stringResult = string.Empty;
        await sut
            .Action(
                i => stringResult = i.ToString(CultureInfo.InvariantCulture),
                err => stringResult = err.GetDisplayMessage());

        _ = stringResult.Should().Be(expected: "1");
    }

    [Fact(DisplayName = "Action should execute error lambda on Task<ErrorResult<T>>")]
    public async Task ActionErrorTaskT()
    {
        Task<IMaybe<int>> sut = Task.FromResult(TestError<int>.Create(message: "Error"));

        string stringResult = string.Empty;
        await sut
            .Action(
                i => stringResult = i.ToString(CultureInfo.InvariantCulture),
                err => stringResult = err.GetDisplayMessage());

        _ = stringResult.Should().Be(expected: "Error");
    }

    [Fact(DisplayName = "Action should not do anything with no error handler on Task<ErrorResult<T>>")]
    public async Task ActionErrorTaskTDefault()
    {
        Task<IMaybe<int>> sut = Task.FromResult(TestError<int>.Create(message: "Error"));

        string stringResult = string.Empty;
        await sut
            .Action(i => stringResult = i.ToString(CultureInfo.InvariantCulture));

        _ = stringResult.Should().Be(string.Empty);
    }
}
