﻿using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers.Models
{
    public class Constructormethod : IMethod
    {
        public Constructormethod(ConstructorDeclarationSyntax constructor)
        {
            Constructor = constructor;
        }

        public ConstructorDeclarationSyntax Constructor { get; set; }

        public BlockSyntax GetBody()
        {
            return Constructor.Body;
        }

        public SyntaxTokenList GetModifiers()
        {
            return Constructor.Modifiers;
        }

        public string GetName()
        {
            return Constructor.Identifier.ToString();
        }

        public string GetReturnType()
        {
            return "void";
        }

        public string GetSuggestionName()
        {
            return GetName() + "()";
        }

        public SyntaxNode GetSuggestionNode()
        {
            return Constructor;
        }
    }
}