namespace MaybeResults.UnitTests;

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

    [Fact(DisplayName = "Match IMaybe")]
    public void MatchIMaybe()
    {
 #pragma warning disable CA1859
        IMaybe resultT = Some.Instance;
 #pragma warning restore CA1859

        IMaybe result = resultT.Match(() => Some.Instance as IMaybe);

        switch (result)
        {
            case Some some:
                some.Should().NotBeNull();
                break;
            default:
                Assert.Fail(message: "Should be Some");
                break;
        }
    }

    [Fact(DisplayName = "Match IMaybe invokes onNone when error")]
    public void MatchIMaybeOnNone()
    {
        IMaybe resultT = TestError.Create("error");
        bool onErrorInvoked = false;

        IMaybe result = resultT.Match(() => Some.Instance, err =>
        {
            err.Message.Should().Be("error");
            onErrorInvoked = true;
        });

        switch (result)
        {
            case Some some:
                some.Should().BeNull();
                break;
            case INone err:
                onErrorInvoked.Should().BeTrue();
                err.Message.Should().Be("error");
                break;
            default:
                Assert.Fail(message: "Should be INone");
                break;
        }
    }

    [Fact(DisplayName = "Match invokes onSomeT")]
    public void MaybeMatchSomeT()
    {
        IMaybe<int> resultT = Maybe.Create(value: 1);

        IMaybe<string> result = resultT.Match(value => value.ToString(CultureInfo.InvariantCulture));

        switch (result)
        {
            case Some<string> some:
                some.Value.Should().Be("1");
                break;
            default:
                Assert.Fail(message: "Should be 1");
                break;
        }
    }

    [Fact(DisplayName = "Match invokes onSomeT with onNone callback")]
    public void MaybeMatchSomeTWithOnNone()
    {
        IMaybe<int> maybeInt = Maybe.Create(value: 1);

        IMaybe<string> result = maybeInt.Match(
            value => value.ToString(CultureInfo.InvariantCulture),
            err => Assert.Fail("Should not invoke onNone"));

        switch (result)
        {
            case Some<string> some:
                some.Value.Should().Be("1");
                break;
            default:
                Assert.Fail(message: "Should be 1");
                break;
        }
    }

    [Fact(DisplayName = "Match invokes onSome")]
    public void MaybeMatchNone()
    {
        IMaybe<int> resultT = TestError<int>.Create("Message");
        bool noneInvoked = false;

        IMaybe<string> result = resultT.Match(
            value => value.ToString(CultureInfo.InvariantCulture),
            err => noneInvoked = true);

        switch (result)
        {
            case Some<string>:
                Assert.Fail(message: "Should be none");
                break;
            case TestError<string> err:
                noneInvoked.Should().Be(true);
                err.Message.Should().Be("Message");
                break;
            default:
                Assert.Fail(message: "Should be INone<string>");
                break;
        }
    }

    [Fact(DisplayName = "Match overloads from IMaybe")]
    public async Task SomeMatchOverloads()
    {
        IMaybe<int> resultT = Maybe.Create(1);

        _ = resultT.Match(() => Some.Instance as IMaybe).Should().BeAssignableTo<Some>();
        _ = resultT.Match(() => "1").Should().BeAssignableTo<Some<string>>();
        _ = (await resultT.Match(() => Task.FromResult("1"))).Should().BeAssignableTo<Some<string>>();
        _ = (await resultT.Match(() => Task.FromResult(Some.Instance as IMaybe))).Should().BeAssignableTo<Some>();
    }
}
