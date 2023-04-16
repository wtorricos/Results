using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Results;

// Based on https://andrewlock.net/exploring-dotnet-6-part-9-source-generator-updates-incremental-generators/
[Generator(LanguageNames.CSharp)]
public sealed class ErrorResultGenerator : IIncrementalGenerator
{
    const string ErrorResultAttribute = "Results.ErrorResultAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 1. predicate: We only want to filter possible syntax targets for generation in a light way.
        // 2. transform: For syntax nodes that pass the first stage, we filter only the ones we care about.
        // 3. where: is an optimized version for the generators pipeline that filters null values.
        IncrementalValuesProvider<RecordDeclarationSyntax> recordDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => IsSyntaxTargetForGeneration(s),
                static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)
            .Select(static (m, _) => m!);

        IncrementalValueProvider<(Compilation, ImmutableArray<RecordDeclarationSyntax>)> compilationAndClasses =
            context.CompilationProvider.Combine(recordDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses,
            static (spc, source) => Emit(spc, source.Item2));
    }

    //It's important for this first stage in the pipeline to be very fast and not to allocate, as it will be called a lot.
    // As best as I can tell, this predicate will be called every time you make a change in your editor.
    static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is RecordDeclarationSyntax { AttributeLists.Count: > 0 };

    // Filter the syntax nodes to only the ones we care about.
    static RecordDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        // we know the node is a RecordDeclarationSyntax thanks to IsSyntaxTargetForGeneration
        RecordDeclarationSyntax recordDeclarationSyntax = (RecordDeclarationSyntax)context.Node;

        // loop through all the attributes on the record
        foreach (AttributeSyntax attributeSyntax in recordDeclarationSyntax.AttributeLists
                     .SelectMany(attributeList => attributeList.Attributes))
        {
            string? fullName = context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol?.ContainingType.ToDisplayString();

            // Is the attribute the [LoggerMessage] attribute?
            if (fullName == ErrorResultAttribute)
                return recordDeclarationSyntax;
        }

        // we didn't find the attribute we were looking for
        return null;
    }

    static void Emit(SourceProductionContext context, ImmutableArray<RecordDeclarationSyntax> records)
    {
        if (records.IsDefaultOrEmpty)
            return; // nothing to do yet

        IEnumerable<RecordDeclarationSyntax> distinctRecords = records.Distinct();

        foreach (RecordDeclarationSyntax record in distinctRecords)
        {
            string generatedCode = ToGeneratedCode(record);
            context.AddSource($"{record.Identifier.Text}.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
        }
    }

    static string ToGeneratedCode(RecordDeclarationSyntax record)
    {
        string namespaceName = GetNamespace(record);
        string recordName = record.Identifier.Text;

        // Note: Do not use """ here, it will break the generated code.
        // language=cs
        return $@"// <auto-generated />
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Results;

namespace {namespaceName}
{{
    public partial record {recordName} : IErrorResult
    {{
        /// <summary>
        /// Public constructor that takes two parameters the message and the errors.
        /// This constructor it's provided for convenience but the equivalent factory method is recommend since it returns
        /// and IResult instead of the concrete type.
        /// </summary>
        /// <param name=""message"">A generic string that describes the problem.</param>
        /// <param name=""errors"">A list of error details that are composed of a Code and a Detail.</param>
        public {recordName}(string message, IEnumerable<ErrorResultDetail> errors)
        {{
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Errors = errors?.ToList() ?? throw new ArgumentNullException(nameof(errors));
        }}

        /// <summary>
        /// Public constructor that only takes a message. Error details are initialized to an empty array.
        /// This constructor it's provided for convenience but the equivalent factory method is recommend since it returns
        /// and IResult instead of the concrete type.
        /// </summary>
        /// <param name=""message"">A generic string that describes the problem.</param>
        public {recordName}(string message) : this(message, Array.Empty<ErrorResultDetail>())
        {{
        }}

        /// <summary>
        /// This is the recommend way to create an instance of this class.
        /// It takes two parameters the message and the errors.
        /// </summary>
        /// <param name=""message"">A generic string that describes the problem.</param>
        /// <param name=""errors"">A list of error details that are composed of a Code and a Detail.</param>
        /// <returns>An instance of the class abstracted as an IResult.</returns>
        public static IResult Create(string message, IEnumerable<ErrorResultDetail> errors)
        {{
            return new {recordName}(message, errors);
        }}

        /// <summary>
        /// This is the recommend way to create an instance of this class.
        /// It only takes a message. Error details are initialized to an empty array.
        /// </summary>
        /// <param name=""message"">A generic string that describes the problem.</param>
        /// <returns>An instance of the class abstracted as an IResult.</returns>
        public static IResult Create(string message)
        {{
            return new {recordName}(message);
        }}

        public string Message {{ get; }}

        public IReadOnlyCollection<ErrorResultDetail> Errors {{ get; }}

        /// <summary>
        /// Useful method to get a string representation of the error.
        /// It concatenates the Message with the Error list.
        /// </summary>
        /// <returns>A string representation of the error.</returns>
        public string GetDisplayMessage()
        {{
            StringBuilder sb = new(Message);
            foreach (ErrorResultDetail detail in Errors)
            {{
                sb = sb.Append(Environment.NewLine);
                sb = sb.Append(detail.Code).Append(value: "": "").Append(detail.Details);
            }}

            return sb.ToString();
        }}

        /// <summary>
        /// This method is used to cast the result to a different type.
        /// You can find examples on how it's used in the extension methods for Map, FlatMap and Action.
        /// https://github.com/wtorricos/Results/blob/main/src/Results/ResultExtensions.cs
        ///
        /// For example:
        ///     IResult<string> GetResult(int value)
        ///     {{
        ///         IResult<int> intResult = Result.Success(value);
        ///
        ///         return intResult switch
        ///         {{
        ///              SuccessResult<int> success => Result.Success(success.Data.ToString()),
        ///
        ///              // In the case of an error we need to cast it to comply with the method signature.
        ///              IErrorResult error => error.Cast<string>(),
        ///         }}
        ///     }}
        /// </summary>
        /// <typeparam name=""TOut"">The type to cast to.</typeparam>
        /// <returns>The same error but with a different type parameter.</returns>
        public IResult<TOut> Cast<TOut>()
        {{
            return {recordName}<TOut>.Create(Message, Errors);
        }}
    }}

    public partial record {recordName}<T> : IErrorResult<T>
    {{
        /// <summary>
        /// Public constructor that takes two parameters the message and the errors.
        /// This constructor it's provided for convenience but the equivalent factory method is recommend since it returns
        /// and IResult instead of the concrete type.
        /// </summary>
        /// <param name=""message"">A generic string that describes the problem.</param>
        /// <param name=""errors"">A list of error details that are composed of a Code and a Detail.</param>
        public {recordName}(string message, IEnumerable<ErrorResultDetail> errors)
        {{
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Errors = errors?.ToList() ?? throw new ArgumentNullException(nameof(errors));
        }}

        /// <summary>
        /// Public constructor that only takes a message. Error details are initialized to an empty array.
        /// This constructor it's provided for convenience but the equivalent factory method is recommend since it returns
        /// and IResult instead of the concrete type.
        /// </summary>
        /// <param name=""message"">A generic string that describes the problem.</param>
        public {recordName}(string message) : this(message, Array.Empty<ErrorResultDetail>())
        {{
        }}

        /// <summary>
        /// This is the recommend way to create an instance of this class.
        /// It takes two parameters the message and the errors.
        /// </summary>
        /// <param name=""message"">A generic string that describes the problem.</param>
        /// <param name=""errors"">A list of error details that are composed of a Code and a Detail.</param>
        /// <returns>An instance of the class abstracted as an IResult.</returns>
        public static IResult<T> Create(string message, IEnumerable<ErrorResultDetail> errors)
        {{
            return new {recordName}<T>(message, errors);
        }}

        /// <summary>
        /// This is the recommend way to create an instance of this class.
        /// It only takes a message. Error details are initialized to an empty array.
        /// </summary>
        /// <param name=""message"">A generic string that describes the problem.</param>
        /// <returns>An instance of the class abstracted as an IResult.</returns>
        public static IResult<T> Create(string message)
        {{
            return new {recordName}<T>(message);
        }}

        public string Message {{ get; }}

        public IReadOnlyCollection<ErrorResultDetail> Errors {{ get; }}

        /// <summary>
        /// Useful method to get a string representation of the error.
        /// It concatenates the Message with the Error list.
        /// </summary>
        /// <returns>A string representation of the error.</returns>
        public string GetDisplayMessage()
        {{
            StringBuilder sb = new(Message);
            foreach (ErrorResultDetail detail in Errors)
            {{
                sb = sb.Append(Environment.NewLine);
                sb = sb.Append(detail.Code).Append(value: "": "").Append(detail.Details);
            }}

            return sb.ToString();
        }}

        /// <summary>
        /// This method is used to cast the result to a different type.
        /// You can find examples on how it's used in the extension methods for Map, FlatMap and Action.
        /// https://github.com/wtorricos/Results/blob/main/src/Results/ResultExtensions.cs
        ///
        /// For example:
        ///     IResult<string> GetResult(int value)
        ///     {{
        ///         IResult<int> intResult = Result.Success(value);
        ///
        ///         return intResult switch
        ///         {{
        ///              SuccessResult<int> success => Result.Success(success.Data.ToString()),
        ///
        ///              // In the case of an error we need to cast it to comply with the method signature.
        ///              IErrorResult error => error.Cast<string>(),
        ///         }}
        ///     }}
        /// </summary>
        /// <typeparam name=""TOut"">The type to cast to.</typeparam>
        /// <returns>The same error but with a different type parameter.</returns>
        public IResult<TOut> Cast<TOut>()
        {{
            return {recordName}<TOut>.Create(Message, Errors);
        }}

        /// <summary>
        /// Method that allows us to go from IResult<T> to IResult.
        /// </summary>
        /// <returns>An IResult.</returns>
        public IResult ToResult()
        {{
            return {recordName}.Create(Message, Errors);
        }}
    }}
}}
";
    }

    // Taken from https://andrewlock.net/creating-a-source-generator-part-5-finding-a-type-declarations-namespace-and-type-hierarchy/#finding-the-namespace-for-a-class-syntax
    // determine the namespace the class/enum/struct is declared in, if any
    static string GetNamespace(BaseTypeDeclarationSyntax syntax)
    {
        // If we don't have a namespace at all we'll return an empty string
        // This accounts for the "default namespace" case
        string nameSpace = string.Empty;

        // Get the containing syntax node for the type declaration
        // (could be a nested type, for example)
        SyntaxNode? potentialNamespaceParent = syntax.Parent;

        // Keep moving "out" of nested classes etc until we get to a namespace
        // or until we run out of parents
        while (potentialNamespaceParent is not null
               and not NamespaceDeclarationSyntax
               and not FileScopedNamespaceDeclarationSyntax)
            potentialNamespaceParent = potentialNamespaceParent.Parent;

        // Build up the final namespace by looping until we no longer have a namespace declaration
        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            // We have a namespace. Use that as the type
            nameSpace = namespaceParent.Name.ToString();

            // Keep moving "out" of the namespace declarations until we
            // run out of nested namespace declarations
            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                    break;

                // Add the outer namespace as a prefix to the final namespace
                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                namespaceParent = parent;
            }
        }

        // return the final namespace
        return nameSpace;
    }
}
