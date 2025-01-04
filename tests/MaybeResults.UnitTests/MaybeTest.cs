namespace MaybeResults.Test;

public sealed class MaybeTest
{
    [Fact(DisplayName = "Should work fine independently of how it is created")]
    public void MaybeFactoryMethods()
    {
        _ = TestError.Create(message: "Message"); // should not throw
        _ = new TestError(message: "Message"); // should not throw
        _ = TestError.Create(message: "Message"); // should not throw
        _ = new TestError(message: "Message"); // should not throw
    }

    [Fact(DisplayName = "Should throw when arguments are invalid")]
    public void MaybeFactoryMethodsThrowWithInvalidArguments()
    {
        Action getMaybe = () => TestError.Create(message: null);
        _ = getMaybe.Should().Throw<ArgumentNullException>();

        getMaybe = () => TestError.Create(message: "Message", details: null);
        _ = getMaybe.Should().Throw<ArgumentNullException>();


        getMaybe = () => TestError.Create(message: null);
        _ = getMaybe.Should().Throw<ArgumentNullException>();

        getMaybe = () => TestError.Create(message: "Message", details: null);
        _ = getMaybe.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = "Should implement ToMaybe")]
    public void MaybeMaybeToMaybe()
    {
        IMaybe<int> resultT = TestError<int>.Create(message: "Message");

        IMaybe result = resultT.ToMaybe();

        switch (result)
        {
            case TestError:
                break;
            default:
                Assert.Fail(message: "Should be TestError");
                break;
        }
    }

    [Fact(DisplayName = "Cast MaybeResults<T> to MaybeResults")]
    public void MaybeTToMaybe()
    {
        IMaybe<int> resultT = Maybe.Create(value: 1);

        IMaybe result = resultT.ToMaybe();

        switch (result)
        {
            case Some:
                break;
            default:
                Assert.Fail(message: "Should be TestError");
                break;
        }
    }
}
