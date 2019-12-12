﻿using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers.Models
{
    public class Method : IMethod
    {
        public Method(MethodDeclarationSyntax method)
        {
            MethodDeclaration = method;
        }

        public MethodDeclarationSyntax MethodDeclaration { get; set; }

        public BlockSyntax GetBody()
        {
            return MethodDeclaration.Body;
        }

        public SyntaxTokenList GetModifiers()
        {
            return MethodDeclaration.Modifiers;
        }

        public string GetName()
        {
            return MethodDeclaration.Identifier.ToString();
        }

        public string GetReturnType()
        {
            return MethodDeclaration.ReturnType.ToString();
        }

        public string GetSuggestionName()
        {
            return GetName() + "()";
        }

        public SyntaxNode GetSuggestionNode()
        {
            return MethodDeclaration;
        }
    }
}