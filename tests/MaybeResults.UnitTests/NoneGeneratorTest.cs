﻿using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MaybeResults.Test;

public sealed class NoneGeneratorTest
{
    [Fact(DisplayName = "Records with the None attribute should generates the expected code")]
    public void CodeIsGenerated()
    {
        Compilation inputCompilation = CreateCompilation(
            source: """
                    using MaybeResults;

                    namespace MyCode
                    {
                        public class Program
                        {
                            public static void Main(string[] args)
                            {
                            }
                        }

                        [None]
                        public sealed partial record MyError
                        {
                        }
                    }
                    """);

        // directly create an instance of the generator
        // (Note: in the compiler this is loaded from an assembly, and created via reflection at runtime)
        NoneGenerator generator = new();

        // Create the driver that will control the generation, passing in our generator
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the generation pass
        // Note: the generator driver itself is immutable, and all calls return
        // an updated version of the driver that you should use for subsequent calls.
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation: inputCompilation,
            outputCompilation: out Compilation outputCompilation,
            diagnostics: out ImmutableArray<Diagnostic> diagnostics);

        // Assert the resulting compilation:
        // there were no diagnostics created by the generators
        _ = diagnostics.IsEmpty.Should().BeTrue();
        // we have two syntax trees, the original 'user' provided one, and the one added by the generator
        _ = outputCompilation.SyntaxTrees.Count().Should().Be(expected: 2);
        // verify the compilation with the added source has no diagnostics (this means there are no problems)
        _ = outputCompilation.GetDiagnostics().Should().BeEmpty();

        // Get the results:
        GeneratorDriverRunResult runResult = driver.GetRunResult();

        // The runResult contains the combined results of all generators passed to the driver
        _ = runResult.Diagnostics.Should().BeEmpty();
        string actualGenerated = runResult.GeneratedTrees.Single().ToString();
        _ = actualGenerated.Should()
            .Be(
                expected:
                """
                // <auto-generated />
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using MaybeResults;
                using IMaybe = MaybeResults.IMaybe;

                namespace MyCode
                {
                    public partial record MyError : INone
                    {
                        /// <summary>
                        /// Public constructor that takes two parameters the message and the details.
                        /// This constructor it's provided for convenience but the equivalent factory method is recommend since it returns
                        /// and IMaybe instead of the concrete type.
                        /// </summary>
                        /// <param name="message">A generic string that describes the problem.</param>
                        /// <param name="details">A list of error details that are composed of a Code and a Detail.</param>
                        public MyError(string message, IEnumerable<NoneDetail> details)
                        {
                            Message = message ?? throw new ArgumentNullException(nameof(message));
                            Details = details?.ToList() ?? throw new ArgumentNullException(nameof(details));
                        }

                        /// <summary>
                        /// Public constructor that only takes a message. Details are initialized to an empty array.
                        /// This constructor it's provided for convenience but the equivalent factory method is recommend since it returns
                        /// an IMaybe instead of the concrete type.
                        /// </summary>
                        /// <param name="message">A generic string that describes the problem.</param>
                        public MyError(string message) : this(message, Array.Empty<NoneDetail>())
                        {
                        }

                        /// <summary>
                        /// This is the recommend way to create an instance of this class.
                        /// It takes two parameters the message and the details.
                        /// </summary>
                        /// <param name="message">A generic string that describes the problem.</param>
                        /// <param name="details">A list of details that are composed of a Code and a Description.</param>
                        /// <returns>An instance of the class abstracted as an IMaybe.</returns>
                        public static IMaybe Create(string message, IEnumerable<NoneDetail> details)
                        {
                            return new MyError(message, details);
                        }

                        /// <summary>
                        /// This is the recommend way to create an instance of this class.
                        /// It only takes a message. Error details are initialized to an empty array.
                        /// </summary>
                        /// <param name="message">A generic string that describes the problem.</param>
                        /// <returns>An instance of the class abstracted as an IMaybe.</returns>
                        public static IMaybe Create(string message)
                        {
                            return new MyError(message);
                        }

                        public string Message { get; }

                        public IReadOnlyCollection<NoneDetail> Details { get; }

                        /// <summary>
                        /// Useful method to get a string representation of the error.
                        /// It concatenates the Message with the Details list.
                        /// </summary>
                        /// <returns>A string representation of message and the details.</returns>
                        public string GetDisplayMessage()
                        {
                            StringBuilder sb = new(Message);
                            foreach (NoneDetail detail in Details)
                            {
                                sb = sb.Append(Environment.NewLine);
                                sb = sb.Append(detail.Code).Append(value: ": ").Append(detail.Description);
                            }

                            return sb.ToString();
                        }

                        /// <summary>
                        /// This method is used to cast Maybe<T> to a different type.
                        /// You can find examples on how it's used in the extension methods for Map, FlatMap and Action.
                        /// https://github.com/wtorricos/MaybeResults/blob/main/src/Maybe/MaybeExtensions.cs
                        ///
                        /// For example:
                        ///     <![CDATA[
                        ///     IMaybe<string> GetMaybe(int value)
                        ///     {
                        ///         IMaybe<int> intMaybe = Maybe.Create(value);
                        ///
                        ///         return intMaybe switch
                        ///         {
                        ///              Some<int> success => Maybe.Create(success.Value.ToString()),
                        ///
                        ///              // In case of none we need to cast it to comply with the method signature.
                        ///              INone error => error.Cast<string>(),
                        ///         }
                        ///     }
                        ///     ]]>
                        /// </summary>
                        /// <typeparam name="TOut">The type to cast to.</typeparam>
                        /// <returns>The same result but with a different type parameter.</returns>
                        public IMaybe<TOut> Cast<TOut>()
                        {
                            return MyError<TOut>.Create(Message, Details);
                        }
                    }

                    public partial record MyError<T> : INone<T>
                    {
                        /// <summary>
                        /// Public constructor that takes two parameters the message and the details.
                        /// This constructor it's provided for convenience but the equivalent factory method is recommend since it returns
                        /// an IMaybe instead of the concrete type.
                        /// </summary>
                        /// <param name="message">A generic string that describes the problem.</param>
                        /// <param name="details">A list of details that are composed of a Code and a Description.</param>
                        public MyError(string message, IEnumerable<NoneDetail> details)
                        {
                            Message = message ?? throw new ArgumentNullException(nameof(message));
                            Details = details?.ToList() ?? throw new ArgumentNullException(nameof(details));
                        }

                        /// <summary>
                        /// Public constructor that only takes a message. Error details are initialized to an empty array.
                        /// This constructor it's provided for convenience but the equivalent factory method is recommend since it returns
                        /// an IMaybe instead of the concrete type.
                        /// </summary>
                        /// <param name="message">A generic string that describes the problem.</param>
                        public MyError(string message) : this(message, Array.Empty<NoneDetail>())
                        {
                        }

                        /// <summary>
                        /// This is the recommend way to create an instance of this class.
                        /// It takes two parameters the message and the details.
                        /// </summary>
                        /// <param name="message">A generic string that describes the problem.</param>
                        /// <param name="details">A list of error details that are composed of a Code and a Description.</param>
                        /// <returns>An instance of the class abstracted as an IMaybe.</returns>
                        public static IMaybe<T> Create(string message, IEnumerable<NoneDetail> details)
                        {
                            return new MyError<T>(message, details);
                        }

                        /// <summary>
                        /// This is the recommend way to create an instance of this class.
                        /// It only takes a message. Error details are initialized to an empty array.
                        /// </summary>
                        /// <param name="message">A generic string that describes the problem.</param>
                        /// <returns>An instance of the class abstracted as an IMaybe.</returns>
                        public static IMaybe<T> Create(string message)
                        {
                            return new MyError<T>(message);
                        }

                        public string Message { get; }

                        public IReadOnlyCollection<NoneDetail> Details { get; }

                        /// <summary>
                        /// Useful method to get a string representation of the error.
                        /// It concatenates the Message with the Error list.
                        /// </summary>
                        /// <returns>A string representation of the message and the details.</returns>
                        public string GetDisplayMessage()
                        {
                            StringBuilder sb = new(Message);
                            foreach (NoneDetail detail in Details)
                            {
                                sb = sb.Append(Environment.NewLine);
                                sb = sb.Append(detail.Code).Append(value: ": ").Append(detail.Description);
                            }

                            return sb.ToString();
                        }

                        /// <summary>
                        /// This method is used to cast the result to a different type.
                        /// You can find examples on how it's used in the extension methods for Map, FlatMap and Action.
                        /// https://github.com/wtorricos/MaybeResults/blob/main/src/Maybe/MaybeExtensions.cs
                        ///
                        /// For example:
                        ///     <![CDATA[
                        ///     IMaybe<string> GetMaybe(int value)
                        ///     {
                        ///         IMaybe<int> intMaybe = Maybe.Create(value);
                        ///
                        ///         return intMaybe switch
                        ///         {
                        ///              Some<int> success => Maybe.Create(success.Value.ToString()),
                        ///
                        ///              // In the case of an error we need to cast it to comply with the method signature.
                        ///              INone error => error.Cast<string>(),
                        ///         }
                        ///     }
                        ///     ]]>
                        /// </summary>
                        /// <typeparam name="TOut">The type to cast to.</typeparam>
                        /// <returns>The same result but with a different type parameter.</returns>
                        public IMaybe<TOut> Cast<TOut>()
                        {
                            return MyError<TOut>.Create(Message, Details);
                        }

                        /// <summary>
                        /// <![CDATA[ Method that allows us to go from IMaybe<T> to IMaybe. ]]>
                        /// </summary>
                        /// <returns>An IMaybe.</returns>
                        public IMaybe ToMaybe()
                        {
                            return MyError.Create(Message, Details);
                        }
                    }
                }

                """);
    }

    static CSharpCompilation CreateCompilation(string source)
        => CSharpCompilation.Create(
            assemblyName: "compilation",
            syntaxTrees: [CSharpSyntaxTree.ParseText(source)],
            references:
            [
                MetadataReference.CreateFromFile(typeof(NoneGenerator).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(
                    Path.Combine(
                        path1: RuntimeEnvironment.GetRuntimeDirectory(),
                        path2: "netstandard.dll")),
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(
                    Path.Combine(
                        path1: Path.GetDirectoryName(typeof(IEnumerator).GetTypeInfo().Assembly.Location)!,
                        path2: "System.Runtime.dll")),
                MetadataReference.CreateFromFile(
                    Path.Combine(
                        path1: Path.GetDirectoryName(typeof(GCSettings).GetTypeInfo().Assembly.Location)!,
                        path2: "System.Collections.dll"))
            ],
            options: new(OutputKind.ConsoleApplication));
}
