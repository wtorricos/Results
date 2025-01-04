namespace MaybeResults.Test;

public sealed class MaybeExtensionsCastTest
{
    [Fact(DisplayName = "Cast ErrorResult<T1> to ErrorResult<T2>")]
    public void CastError()
    {
        INone<int> result = (INone<int>)TestError<int>.Create(message: "Message");

        IMaybe<string> actual = result.Cast<string>();

        switch (actual)
        {
            case TestError<string> err:
                _ = err.Message.Should().Be(expected: "Message");
                break;
            default:
                Assert.Fail(message: "Should be an error of type MaybeTestRecord<string>");
                break;
        }
    }

    [Fact(DisplayName = "Cast ErrorResult<T1> to ErrorResult<T2> includes error details")]
    public void CastErrorDetails()
    {
        INone<int> result = (INone<int>)TestError<int>.Create(
            message: "Message",
            details: [new(Code: "name", Description: "name cannot be empty")]);

        IMaybe<string> actual = result.Cast<string>();

        switch (actual)
        {
            case TestError<string> err:
                _ = err.Message.Should().Be(expected: "Message");
                _ = err.Details.Should()
                    .SatisfyRespectively(
                        d0 =>
                        {
                            _ = d0.Code.Should().Be(expected: "name");
                            _ = d0.Description.Should().Be(expected: "name cannot be empty");
                        });
                break;
            default:
                Assert.Fail(message: "Should be an error of type MaybeTestRecord<string>");
                break;
        }
    }
}
