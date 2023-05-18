﻿using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Utils;

namespace PatternPal.SyntaxTree.Models.Members.Property
{
    public class Property : AbstractNode, IProperty
    {
        private readonly IEntity _parent;
        internal readonly PropertyDeclarationSyntax propertyDeclarationSyntax;

        public Property(PropertyDeclarationSyntax node, IEntity parent) : base(node, parent?.GetRoot())
        {
            propertyDeclarationSyntax = node;
            _parent = parent;
        }

        public override string GetName()
        {
            return propertyDeclarationSyntax.Identifier.Text;
        }

        public IEnumerable<IModifier> GetModifiers()
        {
            return propertyDeclarationSyntax.Modifiers.ToModifiers();
        }

        public IField GetField()
        {
            return new PropertyField(this);
        }

        public TypeSyntax GetPropertyType()
        {
            return propertyDeclarationSyntax.Type;
        }

        public IEntity GetParent()
        {
            return _parent;
        }

        public SyntaxNode GetReturnType()
        {
            return GetPropertyType();
        }

        public bool HasGetter()
        {
            if (propertyDeclarationSyntax.ExpressionBody != null)
            {
                return true;
            }

            if (propertyDeclarationSyntax.AccessorList == null)
            {
                return false;
            }

            return propertyDeclarationSyntax.AccessorList.Accessors.Any(SyntaxKind.GetAccessorDeclaration);
        }

        public bool HasSetter()
        {
            if (propertyDeclarationSyntax.AccessorList == null)
            {
                return false;
            }

            return propertyDeclarationSyntax.AccessorList.Accessors.Any(SyntaxKind.SetAccessorDeclaration);
        }

        public IMethod GetGetter()
        {
            return new PropertyGetMethod(
                this, propertyDeclarationSyntax.AccessorList?
                    .Accessors.First(s => s.Kind() == SyntaxKind.GetAccessorDeclaration)
            );
        }

        public IMethod GetSetter()
        {
            return new PropertySetMethod(
                this, propertyDeclarationSyntax.AccessorList?
                    .Accessors.First(s => s.Kind() == SyntaxKind.SetAccessorDeclaration)
            );
        }

        public bool IsField()
        {
            if (propertyDeclarationSyntax.AccessorList == null)
            {
                return false;
            }

            var accessors = propertyDeclarationSyntax.AccessorList.Accessors;
            return accessors.Any(SyntaxKind.GetAccessorDeclaration) &&
                   accessors.All(a => a.Body == null && a.ExpressionBody == null);
        }
    }
}
