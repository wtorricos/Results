namespace MaybeResults.Test;

#pragma warning disable CA2007
public sealed class ExpressionSyntaxExtensionsTest
{
    [Fact(DisplayName = "Should allow to use expression syntax for a single parameter")]
    public void ExpressionSyntax1()
    {
        IMaybe<int> maybe1 = Maybe.Create(value: 1);

        IMaybe<int> result =
            from value1 in maybe1
            select value1 + 2;

        switch (result)
        {
            case Some<int> some:
                Assert.Equal(expected: 3, actual: some.Value);
                break;
            default:
                Assert.Fail("Expected Some<int> but got None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to use expression syntax for 2 parameters")]
    public void ExpressionSyntax2()
    {
        IMaybe<int> maybe1 = Maybe.Create(value: 1);
        IMaybe<int> maybe2 = Maybe.Create(value: 2);

        IMaybe<int> result =
            from value1 in maybe1
            from value2 in maybe2
            select value1 + value2;

        switch (result)
        {
            case Some<int> some:
                Assert.Equal(expected: 3, actual: some.Value);
                break;
            default:
                Assert.Fail("Expected Some<int> but got None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to use expression syntax for 2 parameters when one is none")]
    public void ExpressionSyntax2Fail()
    {
        IMaybe<int> maybe1 = Maybe.Create(value: 1);
        IMaybe<int> maybe2 = TestError<int>.Create("error");

        IMaybe<int> result =
            from value1 in maybe1
            from value2 in maybe2
            select value1 + value2;

        switch (result)
        {
            case Some<int>:
                Assert.Fail("Expected an error");
                break;
            case TestError<int> err:
                Assert.Equal("error", err.Message);
                break;
            default:
                Assert.Fail("Expected Some<int> but got None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to use expression syntax for n parameters")]
    public void ExpressionSyntax3()
    {
        IMaybe<int> maybe1 = Maybe.Create(value: 1);
        IMaybe<int> maybe2 = Maybe.Create(value: 2);
        IMaybe<int> maybe3 = Maybe.Create(value: 3);
        IMaybe<int> maybe4 = Maybe.Create(value: 4);
        IMaybe<int> maybe5 = Maybe.Create(value: 5);

        IMaybe<int> result =
            from value1 in maybe1
            from value2 in maybe2
            from value3 in maybe3
            from value4 in maybe4
            from value5 in maybe5
            select value1 + value2 + value3 + value4 + value5;

        switch (result)
        {
            case Some<int> some:
                Assert.Equal(expected: 15, actual: some.Value);
                break;
            default:
                Assert.Fail("Expected Some<int> but got None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to use async expression syntax")]
    public async Task ExpressionSyntaxAsync1()
    {
        Task<IMaybe<int>> maybe1 = Task.FromResult(Maybe.Create(value: 1));

        IMaybe<int> result = await
            (from value1 in maybe1
             select value1);

        switch (result)
        {
            case Some<int> some:
                Assert.Equal(expected: 1, actual: some.Value);
                break;
            default:
                Assert.Fail("Expected Some<int> but got None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to use async expression syntax for 2 parameters")]
    public async Task ExpressionSyntaxAsync2()
    {
        Task<IMaybe<int>> maybe1 = Task.FromResult(Maybe.Create(value: 1));
        Task<IMaybe<int>> maybe2 = Task.FromResult(Maybe.Create(value: 2));

        IMaybe<int> result = await
            (from value1 in maybe1
             from value2 in maybe2
             select value1 + value2);

        switch (result)
        {
            case Some<int> some:
                Assert.Equal(expected: 3, actual: some.Value);
                break;
            default:
                Assert.Fail("Expected Some<int> but got None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to use async expression syntax for n parameters")]
    public async Task ExpressionSyntaxAsync3()
    {
        Task<IMaybe<int>> maybe1 = Task.FromResult(Maybe.Create(value: 1));
        Task<IMaybe<int>> maybe2 = Task.FromResult(Maybe.Create(value: 2));
        Task<IMaybe<int>> maybe3 = Task.FromResult(Maybe.Create(value: 3));
        Task<IMaybe<int>> maybe4 = Task.FromResult(Maybe.Create(value: 4));
        Task<IMaybe<int>> maybe5 = Task.FromResult(Maybe.Create(value: 5));

        IMaybe<int> result = await
            (from value1 in maybe1
             from value2 in maybe2
             from value3 in maybe3
             from value4 in maybe4
             from value5 in maybe5
             select value1 + value2 + value3 + value4 + value5);

        switch (result)
        {
            case Some<int> some:
                Assert.Equal(expected: 15, actual: some.Value);
                break;
            default:
                Assert.Fail("Expected Some<int> but got None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to mix sync and async expression syntax for 2 parameters")]
    public async Task ExpressionSyntaxAsyncMixN1()
    {
        IMaybe<int> maybe1 = Maybe.Create(value: 2);
        Task<IMaybe<int>> maybe2 = Task.FromResult(Maybe.Create(value: 1));

        IMaybe<int> result = await
            (from value1 in maybe1
             from value2 in maybe2
             select value1 + value2);

        switch (result)
        {
            case Some<int> some:
                Assert.Equal(expected: 3, actual: some.Value);
                break;
            default:
                Assert.Fail("Expected Some<int> but got None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to mix async and sync expression syntax for 2 parameters")]
    public async Task ExpressionSyntaxAsyncMixN2()
    {
        Task<IMaybe<int>> maybe1 = Task.FromResult(Maybe.Create(value: 2));
        IMaybe<int> maybe2 = Maybe.Create(value: 1);

        IMaybe<int> result = await
            (from value1 in maybe1
             from value2 in maybe2
             select value1 + value2);

        switch (result)
        {
            case Some<int> some:
                Assert.Equal(expected: 3, actual: some.Value);
                break;
            default:
                Assert.Fail("Expected Some<int> but got None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to mix sync and async expression syntax for n parameters")]
    public async Task ExpressionSyntaxAsyncMixN3()
    {
        IMaybe<int> maybe1 = Maybe.Create(value: 2);
        Task<IMaybe<int>> maybe2 = Task.FromResult(Maybe.Create(value: 1));
        Task<IMaybe<int>> maybe3 = Task.FromResult(Maybe.Create(value: 3));
        Task<IMaybe<int>> maybe4 = Task.FromResult(Maybe.Create(value: 4));
        IMaybe<int> maybe5 = Maybe.Create(value: 5);

        IMaybe<int> result = await
            (from value1 in maybe1
             from value2 in maybe2
             from value3 in maybe3
             from value4 in maybe4
             from value5 in maybe5
             select value1 + value2 + value3 + value4 + value5);

        switch (result)
        {
            case Some<int> some:
                Assert.Equal(expected: 15, actual: some.Value);
                break;
            default:
                Assert.Fail("Expected Some<int> but got None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to use where for a single parameter with where result true")]
    public void ExpressionSyntaxWhere1()
    {
        IMaybe<int> maybe1 = Maybe.Create(value: 1);

        IMaybe<int> result =
            from value1 in maybe1
            where value1 > 0
            select value1 + 2;
        switch (result)
        {
            case Some<int> some:
                Assert.Equal(expected: 3, actual: some.Value);
                break;
            default:
                Assert.Fail("Expected Some<int> but got None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to use where for a single parameter with where result false")]
    public void ExpressionSyntaxWhere2()
    {
        IMaybe<int> maybe1 = Maybe.Create(value: 1);

        IMaybe<int> result =
            from value1 in maybe1
            where value1 > 2
            select value1 + 2;
        switch (result)
        {
            case Some<int>:
                Assert.Fail("Expected None");
                break;
            case PredicateFailedError<int> err:
                err.Message.Should().Be("Predicate failed: 'value1 => (value1 > 2)'");
                break;
            default:
                Assert.Fail("Expected None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to use where for a single parameter with none")]
    public void ExpressionSyntaxWhere21()
    {
        IMaybe<int> maybe1 = TestError<int>.Create("TestError");
        IMaybe<int> maybe2 = Maybe.Create(2);

        IMaybe<int> result =
            from value1 in maybe1
            from value2 in maybe2
            where value2 > 0
            select value1 + value2;
        switch (result)
        {
            case Some<int>:
                Assert.Fail("Expected TestError<int>");
                break;
            case TestError<int> err:
                err.Message.Should().Be("TestError");
                break;
            default:
                Assert.Fail("Expected TestError<int>");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to use where for a Maybe single parameter with where result true")]
    public void ExpressionSyntaxWhere3()
    {
        IMaybe<int> maybe1 = Maybe.Create(value: 1);

        // with where result true
        IMaybe<int> result =
            from value1 in maybe1
            where Maybe.Create(value1 > 0)
            select value1 + 2;
        switch (result)
        {
            case Some<int> some:
                Assert.Equal(expected: 3, actual: some.Value);
                break;
            default:
                Assert.Fail("Expected Some<int> but got None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to use where for a Maybe single parameter with where result false")]
    public void ExpressionSyntaxWhere4()
    {
        IMaybe<int> maybe1 = Maybe.Create(value: 1);

        IMaybe<int> result =
            from value1 in maybe1
            where Maybe.Create(value1 > 2)
            select value1 + 2;
        switch (result)
        {
            case Some<int>:
                Assert.Fail("Expected None");
                break;
            case INone err:
                err.Message.Should().Contain("failed");
                break;
            default:
                Assert.Fail("Expected None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to use where expression syntax for n parameters")]
    public void ExpressionSyntaxWhere5()
    {
        IMaybe<int> maybe1 = Maybe.Create(value: 1);
        IMaybe<int> maybe2 = Maybe.Create(value: 2);
        IMaybe<int> maybe3 = Maybe.Create(value: 3);
        IMaybe<int> maybe4 = Maybe.Create(value: 4);
        IMaybe<int> maybe5 = Maybe.Create(value: 5);

        IMaybe<int> result =
            from value1 in maybe1
            from value2 in maybe2
            from value3 in maybe3
            from value4 in maybe4
            from value5 in maybe5
            where value5 > value1 && value5 > value2
            select value1 + value2 + value3 + value4 + value5;

        switch (result)
        {
            case Some<int> some:
                Assert.Equal(expected: 15, actual: some.Value);
                break;
            default:
                Assert.Fail("Expected Some<int> but got None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to mix sync and async where expression syntax for n parameters")]
    public async Task ExpressionSyntaxWhereAsyncMixN1()
    {
        IMaybe<int> maybe1 = Maybe.Create(value: 2);
        Task<IMaybe<int>> maybe2 = Task.FromResult(Maybe.Create(value: 1));
        Task<IMaybe<int>> maybe3 = Task.FromResult(Maybe.Create(value: 3));
        Task<IMaybe<int>> maybe4 = Task.FromResult(Maybe.Create(value: 4));
        IMaybe<int> maybe5 = Maybe.Create(value: 5);

        IMaybe<int> result = await
            (from value1 in maybe1
             from value2 in maybe2
             from value3 in maybe3
             from value4 in maybe4
             from value5 in maybe5
             where value5 > value1 && value5 > value2
             select value1 + value2 + value3 + value4 + value5);

        switch (result)
        {
            case Some<int> some:
                Assert.Equal(expected: 15, actual: some.Value);
                break;
            default:
                Assert.Fail("Expected Some<int> but got None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to mix sync and async where expression syntax for n parameters where false")]
    public async Task ExpressionSyntaxWhereAsyncMixN2()
    {
        IMaybe<int> maybe1 = Maybe.Create(value: 2);
        Task<IMaybe<int>> maybe2 = Task.FromResult(Maybe.Create(value: 1));
        Task<IMaybe<int>> maybe3 = Task.FromResult(Maybe.Create(value: 3));
        Task<IMaybe<int>> maybe4 = Task.FromResult(Maybe.Create(value: 4));
        IMaybe<int> maybe5 = Maybe.Create(value: 5);

        IMaybe<int> result = await
            (from value1 in maybe1
             from value2 in maybe2
             from value3 in maybe3
             from value4 in maybe4
             from value5 in maybe5
             where value5 < value1
             select value1 + value2 + value3 + value4 + value5);

        switch (result)
        {
            case Some<int> some:
                Assert.Fail("None Expected");
                break;
            case INone err:
                err.Message.Should().Contain("failed");
                break;
            default:
                Assert.Fail("Expected None");
                break;
        }
    }

    [Fact(DisplayName = "Should allow to mix sync and async where async expression for n parameters")]
    public async Task ExpressionSyntaxWhereAsyncMixN3()
    {
        IMaybe<int> maybe1 = Maybe.Create(value: 2);
        Task<IMaybe<int>> maybe2 = Task.FromResult(Maybe.Create(value: 1));
        Task<IMaybe<int>> maybe3 = Task.FromResult(Maybe.Create(value: 3));
        Task<IMaybe<int>> maybe4 = Task.FromResult(Maybe.Create(value: 4));
        IMaybe<int> maybe5 = Maybe.Create(value: 5);

        Task<IMaybe<int>> maybe7 =
            from value1 in maybe1
            from value5 in maybe5
            where Task.FromResult(value5 > value1)
            select value1 + value5;

        IMaybe<int> result = await
            (from value1 in maybe1
             from value2 in maybe2
             from value3 in maybe3
             from value4 in maybe4
             from value7 in maybe7
             where Task.FromResult(value7 > value2)
             select value1 + value2 + value3 + value4 + value7);

        switch (result)
        {
            case Some<int> some:
                some.Value.Should().Be(17);
                break;
            case INone err:
                err.Message.Should().Be("Expected Some<int>(17) but got Error");
                break;
            default:
                Assert.Fail("Expected 17");
                break;
        }
    }
}
