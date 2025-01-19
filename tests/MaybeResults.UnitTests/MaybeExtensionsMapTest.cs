namespace MaybeResults.UnitTests;

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
public sealed class MaybeExtensionsMapTest
{
    [Fact(DisplayName = "Should map on successful IMaybe with Func<TOut>")]
    public void MapSuccess()
    {
        IMaybe sut = Maybe.Create();

        IMaybe<int> actual = sut.Map(() => 1);

        switch (actual)
        {
            case Some<int> successResult:
                _ = successResult.Value.Should().Be(expected: 1);
                break;
            case INone err:
                Assert.Fail(err.GetDisplayMessage());
                break;
            default:
                Assert.Fail("Invalid case");
                break;
        }
    }

    [Fact(DisplayName = "Should map error IMaybe with Func<TOut>")]
    public void MapError()
    {
        IMaybe sut = TestError.Create(message: "Message");

        IMaybe<int> actual = sut.Map(() => 1);

        switch (actual)
        {
            case TestError<int> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestError<int>");
                break;
        }
    }

    [Fact(DisplayName = "Should map on successful IMaybe with Func<Task<TOut>>")]
    public async Task MapSuccessToTask()
    {
        IMaybe sut = Maybe.Create();

        IMaybe<int> actual = await sut.Map(() => Task.FromResult(1));

        switch (actual)
        {
            case Some<int> successResult:
                _ = successResult.Value.Should().Be(expected: 1);
                break;
            case INone err:
                Assert.Fail(err.GetDisplayMessage());
                break;
            default:
                Assert.Fail("Invalid case");
                break;
        }
    }

    [Fact(DisplayName = "Should map error IMaybe with Func<Task<TOut>>")]
    public async Task MapErrorToTask()
    {
        IMaybe sut = TestError.Create(message: "Message");

        IMaybe<int> actual = await sut.Map(() => Task.FromResult(1));

        switch (actual)
        {
            case TestError<int> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestError<int>");
                break;
        }
    }

    [Fact(DisplayName = "Should map successful IMaybe<T> with Func<TOut>")]
    public void MapSuccessT()
    {
        IMaybe<int> sut = Maybe.Create(value: 1);

        IMaybe<string> actual = sut.Map(i => i.ToString(CultureInfo.InvariantCulture));

        switch (actual)
        {
            case Some<string> successResult:
                _ = successResult.Value.Should().Be(expected: "1");
                break;
            case INone err:
                Assert.Fail(err.GetDisplayMessage());
                break;
            default:
                Assert.Fail("Invalid case");
                break;
        }
    }

    [Fact(DisplayName = "Should map error IMaybe<T> with Func<TOut>")]
    public void MapErrorT()
    {
        IMaybe<int> sut = TestError<int>.Create(message: "Message");

        IMaybe<string> actual = sut.Map(i => i.ToString(CultureInfo.InvariantCulture));

        switch (actual)
        {
            case Some<string>:
                Assert.Fail(message: "Should be an error");
                break;
            case TestError<string> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestError<string>");
                break;
        }
    }

    [Fact(DisplayName = "Should map successful IMaybe<T> with Task<Func<TOut>>")]
    public async Task MapSuccessTWithAsyncLambda()
    {
        IMaybe<int> sut = Maybe.Create(value: 1);

        Task<IMaybe<string>> taskActual = sut.Map(i => Task.FromResult(i.ToString(CultureInfo.InvariantCulture)));

        IMaybe<string> actual = await taskActual;

        switch (actual)
        {
            case Some<string> successResult:
                _ = successResult.Value.Should().Be(expected: "1");
                break;
            case INone err:
                Assert.Fail(err.GetDisplayMessage());
                break;
            default:
                Assert.Fail("Invalid case");
                break;
        }
    }

    [Fact(DisplayName = "Should map error IMaybe<T> with Task<Func<TOut>>")]
    public async Task MapErrorTWithAsyncLambda()
    {
        IMaybe<int> sut = TestError<int>.Create(message: "Message");

        IMaybe<string> actual = await sut
            .Map(i => Task.FromResult(i.ToString(CultureInfo.InvariantCulture)));

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

    [Fact(DisplayName = "Should map successful Task<IMaybe<T>> with Func<T, TOut>")]
    public async Task MapSuccessTaskT()
    {
        Task<IMaybe<int>> sut = Task.FromResult(Maybe.Create(value: 1));

        IMaybe<string> actual = await sut
            .Map(i => i.ToString(CultureInfo.InvariantCulture));

        switch (actual)
        {
            case Some<string> successResult:
                _ = successResult.Value.Should().Be(expected: "1");
                break;
            case INone err:
                Assert.Fail(err.GetDisplayMessage());
                break;
            default:
                Assert.Fail("Invalid case");
                break;
        }
    }

    [Fact(DisplayName = "Should map error Task<IMaybe<T>> with Func<T, TOut>")]
    public async Task MapErrorTaskT()
    {
        Task<IMaybe<int>> sut = Task.FromResult(TestError<int>.Create(message: "Message"));

        IMaybe<string> actual = await sut
            .Map(i => i.ToString(CultureInfo.InvariantCulture));

        switch (actual)
        {
            case Some<string>:
                Assert.Fail(message: "Should be an error");
                break;
            case TestError<string> err:
                _ = err.GetDisplayMessage().Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestError<string>");
                break;
        }
    }

    [Fact(DisplayName = "Should map successful Task<IMaybe<T>> with Func<T, Task<TOut>>")]
    public async Task MapSuccessTaskTWithAsyncLambda()
    {
        Task<IMaybe<int>> sut = Task.FromResult(Maybe.Create(value: 1));

        IMaybe<string> actual = await sut
            .Map(i => Task.FromResult(i.ToString(CultureInfo.InvariantCulture)));

        switch (actual)
        {
            case Some<string> successResult:
                _ = successResult.Value.Should().Be(expected: "1");
                break;
            case INone err:
                Assert.Fail(err.GetDisplayMessage());
                break;
            default:
                Assert.Fail("Invalid case");
                break;
        }
    }

    [Fact(DisplayName = "Should map error Task<IMaybe<T>> with Func<T, Task<TOut>>")]
    public async Task MapErrorTaskTToTaskResultT()
    {
        Task<IMaybe<int>> sut = Task.FromResult(TestError<int>.Create(message: "Message"));

        IMaybe<string> actual = await sut
            .Map(i => Task.FromResult(i.ToString(CultureInfo.InvariantCulture)));

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
}
