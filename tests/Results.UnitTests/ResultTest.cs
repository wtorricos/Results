namespace Results.UnitTests;

public sealed class ResultTest
{
    [Test(Description = "Should implement ToResult")]
    public void ErrorResultToResult()
    {
        IResult<int> resultT = new TestResultError<int>(message: "Message");

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
        IResult<int> resultT = Result.Success(1);

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
