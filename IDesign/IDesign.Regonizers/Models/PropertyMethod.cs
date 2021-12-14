﻿using System.Collections.Generic;
﻿using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers.Models
{
    public class PropertyMethod : IMethod
    {
        public PropertyMethod(PropertyDeclarationSyntax property, AccessorDeclarationSyntax accesor)
        {
            Property = property;
            Accesor = accesor;
        }

        public PropertyDeclarationSyntax Property { get; set; }
        public AccessorDeclarationSyntax Accesor { get; set; }

        public BlockSyntax GetBody()
        {
            return Accesor.Body;
        }

        public SyntaxTokenList GetModifiers()
        {
            return Property.Modifiers;
        }

        public string GetName()
        {
            return Property.Identifier.ToString();
        }

        public IEnumerable<ParameterSyntax> GetParameters()
        {
            return new List<ParameterSyntax>();
        }

        public IEnumerable<string> GetParameterTypes()
        {
            return new List<string>();
        }

        public ParameterListSyntax GetParameter()
        {
            return null;
        }

        public string GetReturnType()
        {
            return Property.Type.ToString();
        }

        public string GetSuggestionName()
        {
            return $"{GetName()}_get";
        }

        public SyntaxNode GetSuggestionNode()
        {
            return Property;
        }

        public IEnumerable<string> GetArguments()
        {
            return new List<string>();
        }
    }
}