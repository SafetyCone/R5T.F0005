using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using R5T.T0132;

using SyntaxFactory = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace R5T.F0005
{
    /// <summary>
    /// Initial, parse, simple, syntax generator.
    /// No steps are allowed beyond parsing text, and returning an instance of the desired type from the parse result.
    /// No:
    /// * Checking that return type is not null.
    /// * Check that parsing was a success (if it is not, it should fail).
    /// </summary>
    [FunctionalityMarker]
    public partial interface ISyntaxParser : IFunctionalityMarker
    {
        public ClassDeclarationSyntax ParseClass(string text)
        {
            var output = this.ParseMemberDeclaration(text) as ClassDeclarationSyntax;
            return output;
        }

        public CompilationUnitSyntax ParseCompilationUnit(string text)
        {
            var output = SyntaxFactory.ParseCompilationUnit(text);
            return output;
        }

        public ConstructorDeclarationSyntax ParseConstructor(string text)
        {
            var output = this.ParseMemberDeclaration(text) as ConstructorDeclarationSyntax;
            return output;
        }

        public MemberDeclarationSyntax ParseMemberDeclaration(string text)
        {
            var output = SyntaxFactory.ParseMemberDeclaration(text);
            return output;
        }

        public DocumentationCommentTriviaSyntax ParseDocumentation(string text)
        {
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(text);

            var compilationUnit = syntaxTree.GetRoot() as CompilationUnitSyntax;

            var trivia = compilationUnit.DescendantTrivia()
                .Where(x => x.HasStructure)
                .Single();

            var documentation = trivia.GetStructure() as DocumentationCommentTriviaSyntax;

            return documentation;
        }

        public XmlNodeSyntax[] ParseDocumentationLine(string text)
        {
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(text);

            var firstTrivia = syntaxTree
                .GetRoot()
                .DescendantTrivia().First();

            var structure = firstTrivia.GetStructure();

            var xmlElements = structure.ChildNodes()
                .Cast<XmlNodeSyntax>()
                .ToArray();

            return xmlElements;
        }

        public InterfaceDeclarationSyntax ParseInterface(string text)
        {
            var output = SyntaxFactory.ParseMemberDeclaration(text) as InterfaceDeclarationSyntax;
            return output;
        }

        /// <summary>
        /// Note, this is a wrapper around <see cref="SyntaxFactory.ParseMemberDeclaration(string, int, Microsoft.CodeAnalysis.ParseOptions?, bool)"/>.
        /// As a result, it produces method body blocks with open braces that include a trailing new line.
        /// This is non-standard.
        /// </summary>
        public MethodDeclarationSyntax ParseMethod(string text)
        {
            var output = SyntaxFactory.ParseMemberDeclaration(text) as MethodDeclarationSyntax;
            return output;
        }

        public ParameterSyntax ParseParameter(string text)
        {
            var output = SyntaxFactory.ParseParameterList(text)
                // Emphasize correctness; throw if more or less than one.
                .Parameters.Single();

            return output;
        }

        public PropertyDeclarationSyntax ParseProperty(string text)
        {
            var output = SyntaxFactory.ParseMemberDeclaration(text) as PropertyDeclarationSyntax;
            return output;
        }

        public ReturnStatementSyntax ParseReturnStatement(string text)
        {
            var output = this.ParseStatement(text) as ReturnStatementSyntax;
            return output;
        }

        /// <summary>
        /// Parses text containing a statement.
        /// <example-text-input>Example text input:
        /// <code>
        /// var executionSynchronicityProviderAction = Instances.ServiceAction.AddConstructorBasedExecutionSynchronicityProviderAction(Synchronicity.Synchronous);
        /// </code>
        /// </example-text-input>
        /// </summary>
        public StatementSyntax ParseStatement(string text)
        {
            var output = SyntaxFactory.ParseStatement(text);
            return output;
        }

        /// <summary>
        /// Parses multiple statements using the single statement parsing functionality.
        /// <inheritdoc cref="ParseStatement(string)" path="/summary/example-text-input"/>
        /// </summary>
        public IEnumerable<StatementSyntax> ParseStatements(IEnumerable<string> texts)
        {
            var output = texts.Select(this.ParseStatement);
            return output;
        }

        /// <summary>
        /// Parses text containing multiple statements. Example text input:
        /// <code>
        /// var executionSynchronicityProviderAction = Instances.ServiceAction.AddConstructorBasedExecutionSynchronicityProviderAction(Synchronicity.Synchronous);
        /// var organizationProviderAction = Instances.ServiceAction.AddOrganizationProviderAction(); // Rivet organization.
        /// var rootOutputDirectoryPathProviderAction = Instances.ServiceAction.AddConstructorBasedRootOutputDirectoryPathProviderAction(@"C:\Temp\Output");
        /// </code>
        /// </summary>
        public IEnumerable<StatementSyntax> ParseStatements(string text)
        {
            // Parse as a compilation unit. Statements will each be wrapped in global statements.
            var statements = SyntaxFactory.ParseCompilationUnit(text)
                .GetChildrenOfType<GlobalStatementSyntax>()
                .Select(xGlobalStatement => xGlobalStatement
                    .GetChildAsType<StatementSyntax>());

            return statements;
        }

        public UsingDirectiveSyntax ParseUsingDirective(string text)
        {
            var output = SyntaxFactory.ParseCompilationUnit(text)
                .DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                // Emphasize correctness; throw if multiple.
                .Single();

            return output;
        }

    }
}
