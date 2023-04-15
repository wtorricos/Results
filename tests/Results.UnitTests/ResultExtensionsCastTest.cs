namespace Results.UnitTests;

public sealed class ResultExtensionsCastTest
{
    [Test(Description = "Cast ErrorResult<T1> to ErrorResult<T2>")]
    public void CastError()
    {
        IErrorResult<int> result = (IErrorResult<int>)Result.Error<TestResultError<int>, int>(message: "Message");

        IResult<string> actual = result.Cast<string>();

        switch (actual)
        {
            case TestResultError<string> err:
                _ = err.Message.Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestResultError<string>");
                break;
        }
    }

    [Test(Description = "Cast ErrorResult<T1> to ErrorResult<T2> includes error details")]
    public void CastErrorDetails()
    {
        IErrorResult<int> result = (IErrorResult<int>)Result.Error<TestResultError<int>, int>(
            message: "Message",
            new List<ErrorResultDetail>
            {
                new(Code: "name", Details: "name cannot be empty")
            });

        IResult<string> actual = result.Cast<string>();

        switch (actual)
        {
            case TestResultError<string> err:
                _ = err.Message.Should().Be(expected: "Message");
                _ = err.Errors.Should().SatisfyRespectively(
                    d0 =>
                    {
                        _ = d0.Code.Should().Be(expected: "name");
                        _ = d0.Details.Should().Be(expected: "name cannot be empty");
                    });
                break;
            default:
                Assert.Fail(message: "Should be an error of type TestResultError<string>");
                break;
        }
    }
}
