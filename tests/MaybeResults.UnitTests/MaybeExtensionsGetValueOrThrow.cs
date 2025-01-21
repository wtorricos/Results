namespace MaybeResults.UnitTests;

public sealed class MaybeExtensionsGetValueOrThrow
{
    [Fact(DisplayName = "Should return the value successfully")]
    public void GetValueOrThrow()
    {
        // Arrange
        IMaybe<int> some = Maybe.Create(1);

        // Act
        int actual = some.GetValueOrThrow();

        // Assert
        actual.Should().Be(1);
    }

    [Fact(DisplayName = "Should throw when is None")]
    public void GetValueOrThrowThrows()
    {
        // Arrange
        IMaybe<int> some = TestError<int>.Create("Error");

        // Act
        Func<int> actual = () => some.GetValueOrThrow();

        // Assert
        actual.Should().Throw<InvalidOperationException>("Error");
    }
}
