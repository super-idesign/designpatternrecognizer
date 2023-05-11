﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Abstractions.Root;
using PatternPal.SyntaxTree.Models.Members.Constructor;
using PatternPal.SyntaxTree.Models.Members.Method;
using PatternPal.SyntaxTree.Models.Members.Property;
using PatternPal.SyntaxTree.Utils;

namespace PatternPal.SyntaxTree.Models.Entities
{
    public abstract class AbstractEntity : AbstractNode, IEntity
    {
        private readonly List<TypeSyntax> _bases;
        private readonly List<IEntity> _entities = new();

        private readonly List<IMethod> _methods = new();

        private readonly IEntitiesContainer _parent;
        private readonly List<IProperty> _properties = new();
        private readonly TypeDeclarationSyntax _typeDeclarationSyntax;

        protected AbstractEntity(TypeDeclarationSyntax node, IEntitiesContainer parent) : base(
            node, parent.GetRoot()
        )
        {
            _typeDeclarationSyntax = node;
            _parent = parent;

            foreach (var member in node.Members)
            {
                switch (member)
                {
                    case MethodDeclarationSyntax method:
                        _methods.Add(new Method(method, this));
                        break;
                    case PropertyDeclarationSyntax property:
                        _properties.Add(new Property(property, this));
                        break;
                    case TypeDeclarationSyntax type:
                        _entities.Add(type.ToEntity(this));
                        break;
                }
            }

            _bases = node.BaseList?.Types.Select(t => t.Type).ToList() ?? new List<TypeSyntax>();
        }

        public TypeDeclarationSyntax GetTypeDeclarationSyntax()
        {
            return _typeDeclarationSyntax;
        }

        public override string GetName()
        {
            return _typeDeclarationSyntax.Identifier.Text;
        }

        public IEnumerable<IModifier> GetModifiers()
        {
            return _typeDeclarationSyntax.Modifiers.ToModifiers();
        }

        public IEnumerable<IMethod> GetMethods()
        {
            return _methods.AsReadOnly();
        }

        public IEnumerable<IProperty> GetProperties()
        {
            return _properties.AsReadOnly();
        }

        public virtual IEnumerable<IMember> GetMembers() { return _methods.Cast<IMember>().Concat(_properties); }

        public IEnumerable<IMember> GetAllMembers()
        {
            var members = GetMembers().ToList();
            
            foreach (var property in members.OfType<Property>()) {
                if (property.IsField()) members.Add(property.GetField());
                if (property.HasGetter()) members.Add(property.GetGetter());
                if (property.HasSetter()) members.Add(property.GetSetter());
            }
            
            foreach (var constructor in members.OfType<Constructor>())
            {
                members.Add(constructor.AsMethod());
            }
            
            return members;
        }

        public IEnumerable<IEntity> GetEntities()
        {
            return _entities.AsReadOnly();
        }

        public IEnumerable<TypeSyntax> GetBases()
        {
            return _bases;
        }

        public abstract EntityType GetEntityType();

        public Dictionary<string, IEntity> GetAllEntities()
        {
            return _entities.OfType<IEntitiesContainer>()
                .Select(e => e.GetAllEntities())
                .Append(_entities.ToDictionary(e => e.GetFullName()))
                .SelectMany(d => d)
                .ToDictionary(p => p.Key, p => p.Value);
        }

        public string GetFullName()
        {
            if (_parent is INamespace names)
            {
                return $"{names.GetNamespace()}.{GetName()}";
            }

            return GetName();
        }

        public IEnumerable<Relation> GetRelations(RelationTargetKind type)
        {
            return _parent.GetRoot().GetRelations(this, type);
        }

        public virtual IEnumerable<IMethod> GetAllMethods()
        {
            var methods = new List<IMethod>(GetMethods());
            foreach (var property in GetProperties())
            {
                if (property.HasGetter())
                {
                    methods.Add(property.GetGetter());
                }

                if (property.HasSetter())
                {
                    methods.Add(property.GetSetter());
                }
            }

            return methods.AsReadOnly();
        }

        public virtual IEnumerable<IField> GetAllFields()
        {
            return GetProperties()
                .Where(p => p.IsField())
                .Select(p => p.GetField());
        }

        public string GetNamespace()
        {
            if (GetParent() is INamedEntitiesContainer name)
            {
                return $"{name.GetNamespace()}.{GetName()}";
            }

            return GetName();
        }

        public IEntitiesContainer GetParent()
        {
            return _parent;
        }

        public override string ToString()
        {
            return GetName();
        }

        public virtual IEnumerable<INode> GetChildren()
        {
            return _entities.Cast<INode>()
                .Concat(_methods)
                .Concat(_properties);
        }
    }
}
