namespace Results.UnitTests;

public sealed class ResultExtensionsAssertTest
{
    [Test(Description = "Assert should return data on success Result<T>")]
    public void ResultTReturnsData()
    {
        IResult<int> result = Result.Success(data: 1);

        int actual = result.AssertSuccess();

        _ = actual.Should().Be(expected: 1);
    }

    [Test(Description = "Assert should throw exception on ErrorResult<T>")]
    public void ResultTThrowExceptionOnError()
    {
        IResult<int> result = Result.Error<TestResultError<int>, int>(message: "Message");

        Action actual = () => result.AssertSuccess();

        _ = actual.Should().Throw<ResultAssertSuccessFailedException>();
    }

    [Test(Description = "Assert should not do anything on success")]
    public void ResultReturnsData()
    {
        IResult result = Result.Success();

        result.AssertSuccess();
    }

    [Test(Description = "Assert should throw exception on ErrorResult")]
    public void ResultThrowExceptionOnError()
    {
        IResult result = Result.Error<TestResultError>(message: "Message");

        Action actual = result.AssertSuccess;

        _ = actual.Should().Throw<ResultAssertSuccessFailedException>();
    }
}
