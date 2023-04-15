﻿using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Results.UnitTests;

// Documentation on testing generators can be found here:
// https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md#unit-testing-of-generators
public sealed class ErrorResultGeneratorTest
{
    [Test(Description = "Records with the ErrorResult attribute should generates the expected code")]
    public void CodeIsGenerated()
    {
        Compilation inputCompilation = CreateCompilation(
            """
            using Results;

            namespace MyCode
            {
                public class Program
                {
                    public static void Main(string[] args)
                    {
                    }
                }

                [ErrorResult]
                public sealed partial record MyError
                {
                }
            }
            """);

        // directly create an instance of the generator
        // (Note: in the compiler this is loaded from an assembly, and created via reflection at runtime)
        ErrorResultGenerator generator = new();

        // Create the driver that will control the generation, passing in our generator
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the generation pass
        // Note: the generator driver itself is immutable, and all calls return
        // an updated version of the driver that you should use for subsequent calls.
        driver = driver.RunGeneratorsAndUpdateCompilation(
            inputCompilation,
            out Compilation outputCompilation,
            out ImmutableArray<Diagnostic> diagnostics);

        // Assert the resulting compilation:
        // there were no diagnostics created by the generators
        diagnostics.IsEmpty.Should().BeTrue();
        // we have two syntax trees, the original 'user' provided one, and the one added by the generator
        outputCompilation.SyntaxTrees.Count().Should().Be(2);
        // verify the compilation with the added source has no diagnostics (this means there are no problems)
        outputCompilation.GetDiagnostics().Should().BeEmpty();

        // Get the results:
        GeneratorDriverRunResult runResult = driver.GetRunResult();

        // The runResult contains the combined results of all generators passed to the driver
        runResult.Diagnostics.Should().BeEmpty();
        runResult.GeneratedTrees.Single().ToString().Should().Be(
            """
            // <auto-generated />
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using Results;

            namespace MyCode
            {
                public partial record MyError : IErrorResult
                {
                    public MyError(string message, IEnumerable<ErrorResultDetail> errors)
                    {
                        Message = message;
                        Errors = errors.ToList();
                    }

                    public MyError(string message) : this(message, Array.Empty<ErrorResultDetail>())
                    {
                    }

                    public string Message { get; }

                    public IReadOnlyCollection<ErrorResultDetail> Errors { get; }

                    public string GetDisplayMessage()
                    {
                        StringBuilder sb = new(Message);
                        foreach (ErrorResultDetail detail in Errors)
                        {
                            sb = sb.Append(Environment.NewLine);
                            sb = sb.Append(detail.Code).Append(value: ": ").Append(detail.Details);
                        }

                        return sb.ToString();
                    }

                    public IResult<TOut> Cast<TOut>()
                    {
                        return new MyError<TOut>(Message, Errors);
                    }
                }

                public partial record MyError<T> : IErrorResult<T>
                {
                    public MyError(string message, IEnumerable<ErrorResultDetail> errors)
                    {
                        Message = message;
                        Errors = errors.ToList();
                    }

                    public MyError(string message) : this(message, Array.Empty<ErrorResultDetail>())
                    {
                    }

                    public string Message { get; }

                    public IReadOnlyCollection<ErrorResultDetail> Errors { get; }

                    public string GetDisplayMessage()
                    {
                        StringBuilder sb = new(Message);
                        foreach (ErrorResultDetail detail in Errors)
                        {
                            sb = sb.Append(Environment.NewLine);
                            sb = sb.Append(detail.Code).Append(value: ": ").Append(detail.Details);
                        }

                        return sb.ToString();
                    }

                    public IResult<TOut> Cast<TOut>()
                    {
                        return new MyError<TOut>(Message, Errors);
                    }

                    public IResult ToResult()
                    {
                        return new MyError(Message, Errors);
                    }
                }
            }

            """);
    }

    private static Compilation CreateCompilation(string source)
        => CSharpCompilation.Create(
            "compilation",
            new[] { CSharpSyntaxTree.ParseText(source) },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(ErrorResultGenerator).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "netstandard.dll")),
                MetadataReference.CreateFromFile(typeof(Object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(
                    Path.Combine(
                        Path.GetDirectoryName(typeof(IEnumerator).GetTypeInfo().Assembly.Location)!,
                        "System.Runtime.dll")),
                MetadataReference.CreateFromFile(
                    Path.Combine(
                        Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location)!,
                        "System.Collections.dll"))
            },
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
}