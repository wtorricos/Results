namespace MaybeResults.UnitTests;

[None]
public sealed partial record DivideByZeroError;

public sealed class MaybeSample
{
    public static IMaybe<int> MyDivision(int numerator, int denominator)
    {
        if (denominator == 0)
        {
            return DivideByZeroError<int>.Create("Division by zero is not allowed");
        }

        return Maybe.Create(numerator / denominator);
    }

    [Fact(DisplayName = "Success Example")]
    public void SuccessExample()
    {
        IMaybe<int> some = MyDivision(5, 5);
        switch (some)
        {
            case Some<int> result:
                result.Value.Should().Be(1);
                break;
            default:
                Assert.Fail("Expected 1");
                break;
        }
    }

    [Fact(DisplayName = "Success NonGeneric Version Example")]
    public void SuccessNonGenericVersionExample()
    {
        IMaybe canBeDivided = MyDivision(5, 5).ToMaybe();
        switch (canBeDivided)
        {
            case Some result:
                result.Should().NotBeNull();
                break;
            default:
                Assert.Fail("Expected 1");
                break;
        }
    }

    [Fact(DisplayName = "DivideByZeroError Example")]
    public void DivideByZeroErrorExample()
    {
        IMaybe<int> some = MyDivision(5, 0);
        switch (some)
        {
            case Some<int>:
                Assert.Fail("Expected DividedByZeroError");
                break;
            case DivideByZeroError<int> err:
                err.Message.Should().Be("Division by zero is not allowed");
                break;
            default:
                Assert.Fail("Expected DividedByZeroError");
                break;
        }
    }

    [Fact(DisplayName = "Map Example")]
    public void MapExample()
    {
        IMaybe<int> five = MyDivision(5, 1);
        IMaybe<bool> isPair = five.Map(i => i % 2 == 0);
        switch (isPair)
        {
            case Some<bool> result:
                result.Value.Should().Be(false);
                break;
            default:
                Assert.Fail("Expected false");
                break;
        }
    }

    [Fact(DisplayName = "Map Error Example")]
    public void MapErrorExample()
    {
        IMaybe<int> divideError = MyDivision(5, 0);
        IMaybe<bool> isPair = divideError.Map(i => i % 2 == 0);
        switch (isPair)
        {
            case Some<bool>:
                Assert.Fail("Expected DividedByZeroError<bool>");
                break;
            case DivideByZeroError<bool> err:
                err.Message.Should().Be("Division by zero is not allowed");
                break;
            // INone will catch all errors, use specific errors when possible
            case INone:
                Assert.Fail("Expected DividedByZeroError<bool>");
                break;
            default:
                Assert.Fail("Expected DividedByZeroError<bool>");
                break;
        }
    }

    [Fact(DisplayName = "Linq Syntax Example")]
    public void LinqSyntaxExample()
    {
        IMaybe<int> some = from i in MyDivision(6, 1) // Some(6)
                           from j in MyDivision(2, 2) // Some(1)
                           select i + j; // 6 + 1

        switch (some)
        {
            case Some<int> result:
                result.Value.Should().Be(7);
                break;
            default:
                Assert.Fail("Expected 7");
                break;
        }
    }

    [Fact(DisplayName = "Cast Example")]
    public void CastExample()
    {
        IMaybe<string> ValidateDivision(int numerator, int denominator)
        {
            IMaybe<int> result = MyDivision(numerator, denominator);

            if (result is INone error)
            {
                // cannot return error directly because it is of IMaybe<int> type
                return error.Cast<string>();
            }

            return Maybe.Create("Is Valid!");
        }

        ValidateDivision(1, 0).Should().BeAssignableTo<INone<string>>();
        ValidateDivision(1, 0).Should().BeOfType<DivideByZeroError<string>>();
        ValidateDivision(1, 1).Should().BeAssignableTo<Some<string>>();
    }

    [Fact(DisplayName = "Match Example")]
    public void MatchExample()
    {
        IMaybe<int> some = MyDivision(5, 5);

        IMaybe<string> actual = some.Match(
            value => value.ToString(CultureInfo.InvariantCulture),
            error => error.Cast<string>());

        switch (actual)
        {
            case Some<string> result:
                result.Value.Should().Be("1");
                break;
            default:
                Assert.Fail("Expected 1");
                break;
        }
    }
}
