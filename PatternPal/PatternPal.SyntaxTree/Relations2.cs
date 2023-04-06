﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Root;
using SyntaxTree.Models.Entities;
using SyntaxTree.Models.Members.Method;

namespace SyntaxTree
{
    public class Relations2
    {
        private static readonly Dictionary<RelationType, RelationType> ReversedTypes =
            new Dictionary<RelationType, RelationType>
            {
                { RelationType.Implements, RelationType.ImplementedBy },
                { RelationType.ImplementedBy, RelationType.Implements },
                { RelationType.Creates, RelationType.CreatedBy },
                { RelationType.CreatedBy, RelationType.Creates },
                { RelationType.Uses, RelationType.UsedBy },
                { RelationType.UsedBy, RelationType.Uses },
                { RelationType.Extends, RelationType.ExtendedBy },
                { RelationType.ExtendedBy, RelationType.Extends }
            };
    
        //private static readonly Dictionary<Relationable, Relationable> ReversedRelationables =
        //    new Dictionary<Relationable, Relationable>
        //    {
        //        { Relationable.Entity, Relationable.Method }, { Relationable.Method, Relationable.Entity }
        //    };

        //List with all relations in graph
        internal List<Relation2> Relations = new List<Relation2>();

        //Dictionary to access relations of entities fast
        internal Dictionary<IEntity, List<Relation2>> EntityRelations = new Dictionary<IEntity, List<Relation2>>();

        //Dictionary to access relations of methods fast
        internal Dictionary<Method, List<Relation2>> MethodRelations = new Dictionary<Method, List<Relation2>>();

        private readonly SyntaxGraph _graph;
        private Dictionary<string, IEntity> _entities;
        private List<Method> _methods = new List<Method>();

        public Relations2(SyntaxGraph graph)
        {
            _graph = graph;
        }

        public void CreateEdges()
        {
            _entities = _graph.GetAll();
            //Get all methods
            _methods.AddRange(_entities.Values.SelectMany(y =>
                y.GetMembers().Where(x => x.GetType() == typeof(Method)).Cast<Method>()));

            foreach (IEntity entity in _entities.Values)
            {
                CreateParentClasses(entity);
                CreateCreationalEdges(entity);
                CreateUsingEdges(entity);
            }
        }

        private void AddRelation(INode node1, INode node2, RelationType type)
        {
            if(node1 == null || node2 == null)
                return;

            Relation2 relation = new Relation2(type);
            Relation2 relationReversed = new Relation2(ReversedTypes[type]);

            switch (node1)
            {
                case IEntity e:
                    relation.Node1Entity = e;
                    relationReversed.Node2Entity = e;
                    break;
                case Method m:
                    relation.Node1Method = m;
                    relationReversed.Node2Method = m;
                    break;
                default:
                    throw new ArgumentException($"Cannot add relations to {node1}");
            }

            switch (node2)
            {
                case IEntity e:
                    relation.Node2Entity = e;
                    relationReversed.Node1Entity = e;
                    break;
                case Method m:
                    relation.Node2Method = m;
                    relationReversed.Node2Method = m;
                    break;
                default:
                    throw new ArgumentException($"Cannot add relations to {node2}");
            }

            if (Relations.Contains(relation))
                return;

            switch (node1)
            {
                case IEntity e:
                    if (!EntityRelations.ContainsKey(e))
                        EntityRelations[e] = new List<Relation2> { relation };
                    else
                        EntityRelations[e].Add(relation);
                    break;
                case Method m:
                    if (!MethodRelations.ContainsKey(m))
                        MethodRelations[m] = new List<Relation2> { relation };
                    else
                        MethodRelations[m].Add(relation);
                    break;
                default:
                    throw new ArgumentException($"Cannot add relations to {node1}");
            }

            switch (node2)
            {
                case IEntity e:
                    if (!EntityRelations.ContainsKey(e))
                        EntityRelations[e] = new List<Relation2> { relation };
                    else
                        EntityRelations[e].Add(relation);
                    break;
                case Method m:
                    if (!MethodRelations.ContainsKey(m))
                        MethodRelations[m] = new List<Relation2> { relation };
                    else
                        MethodRelations[m].Add(relation);
                    break;
                default:
                    throw new ArgumentException($"Cannot add relations to {node1}");
            }

            Relations.Add(relation);
            AddRelation(node2, node1, ReversedTypes[type]);
        }

        private IEntity GetEntityByName(SyntaxNode syntaxNode)
        {
            return _entities.Values.FirstOrDefault(x => x.GetSyntaxNode().IsEquivalentTo(syntaxNode));
        }

        private Method GetMethodByName(SyntaxNode methodDeclaration)
        {
            return _methods.FirstOrDefault(x => x.GetSyntaxNode().IsEquivalentTo(methodDeclaration));
        }

        private void CreateParentClasses(IEntity entity)
        {
            foreach (TypeSyntax type in entity.GetBases())
            {
                //TODO check generic
                //string typeName = type.ToString();

                IEntity edgeNode = GetEntityByName(type);
                if (edgeNode == null)
                {
                    continue;
                }

                RelationType relationType;

                switch (edgeNode.GetEntityType())
                {
                    case EntityType.Class:
                        relationType = RelationType.Extends;
                        break;
                    case EntityType.Interface:
                        relationType = RelationType.Implements;
                        break;
                    default:
                        Console.Error.WriteLine(
                            $"EntityType {edgeNode.GetType()} is not yet supported in DetermineRelations!"
                        );
                        continue;
                }

                AddRelation(entity, edgeNode, relationType, Relationable.Entity);
            }
        }

        private void CreateCreationalEdges(INode node)
        {

            IEnumerable<SyntaxNode> childNodes = node.GetSyntaxNode().DescendantNodes();
            foreach (var creation in childNodes.OfType<ObjectCreationExpressionSyntax>())
            {
                if (creation.Type is IdentifierNameSyntax name)
                {
                    INode node2 = (INode)GetEntityByName(creation) ?? GetMethodByName(creation);

                    AddRelation(node, node2, RelationType.Creates);
                }
            }
        }

        private void CreateUsingEdges(INode node)
        {
            var childNodes = new List<SyntaxNode>();
            foreach (var member in node.GetMembers())
            {
                childNodes.AddRange(member.GetSyntaxNode().DescendantNodes(n => true));
            }

            foreach (var identifier in childNodes.OfType<IdentifierNameSyntax>())
            {
                if (identifier is IdentifierNameSyntax name)
                {
                    AddRelation(entity, RelationType.Uses, name.ToString());
                }
            }
        }
    }
}
