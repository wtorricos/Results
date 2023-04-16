namespace Results.UnitTests;

public sealed class ResultTest
{
    [Test(Description = "Should work fine independently of how it is created")]
    public void ErrorFactoryMethods()
    {
        _ = TestResultError<int>.Create(message: "Message"); // should not throw
        _ = new TestResultError<int>(message: "Message"); // should not throw
        _ = TestResultError.Create(message: "Message"); // should not throw
        _ = new TestResultError(message: "Message"); // should not throw
    }

    [Test(Description = "Should throw when arguments are invalid")]
    public void ErrorFactoryMethodsThrowWithInvalidArguments()
    {
        Action getResult = () => TestResultError<int>.Create(message: null);
        getResult.Should().Throw<ArgumentNullException>();

        getResult = () =>  TestResultError<int>.Create(message: "Message", errors: null);
        getResult.Should().Throw<ArgumentNullException>();


        getResult = () => TestResultError.Create(message: null);
        getResult.Should().Throw<ArgumentNullException>();

        getResult = () => TestResultError.Create(message: "Message", errors: null);
        getResult.Should().Throw<ArgumentNullException>();
    }

    [Test(Description = "Should implement ToResult")]
    public void ErrorResultToResult()
    {
        IResult<int> resultT = TestResultError<int>.Create(message: "Message");

        IResult result = resultT.ToResult();

        switch (result)
        {
            case TestResultError:
                break;
            default:
                Assert.Fail(message: "Should be TestResultError");
                break;
        }
    }

    [Test(Description = "Cast Result<T> to Result")]
    public void ResultTToResult()
    {
        IResult<int> resultT = Result.Success(data: 1);

        IResult result = resultT.ToResult();

        switch (result)
        {
            case SuccessResult:
                break;
            default:
                Assert.Fail(message: "Should be TestResultError");
                break;
        }
    }
}
