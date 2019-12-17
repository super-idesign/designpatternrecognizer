﻿using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers
{
    public class FactoryMethodRecognizer : IRecognizer
    {
        Result result;
        public IResult Recognize(IEntityNode entityNode)
        {
            result = new Result();
            var relations = entityNode.GetRelations();

            //create list with only Extends and Implements relations
            var inheritanceRelations = relations.Where(x => (x.GetRelationType() == RelationType.Implements) ||
            (x.GetRelationType() == RelationType.Extends)).ToList();

            //create list with only uses realations
            var usingRelations = relations.Where(x => x.GetRelationType() == RelationType.Uses).ToList();

            //create list with only creates relations
            var createsRelations = relations.Where(x => x.GetRelationType() == RelationType.Creates).ToList();

            if (entityNode.CheckTypeDeclaration(EntityNodeType.Class) && entityNode.CheckModifier("abstract"))
            {
                if (usingRelations.Count > 0)
                {
                    //creator
                    AbstractCreatorChecks(entityNode, usingRelations);
                }
            }
            return result;
        }

        /// <summary>
        ///     Function to check if a node is a product
        /// </summary>
        /// <param name="node"></param>
        private void ProductChecks(IEntityNode node)
        {
            var relations = node.GetRelations();
            var inheritanceRelations = relations.Where(x => (x.GetRelationType() == RelationType.ImplementedBy)
                || (x.GetRelationType() == RelationType.ExtendedBy)).ToList();

            foreach (var edge in inheritanceRelations)
            {
                var inheritanceEdgeRelations = edge.GetDestination().GetRelations().Where(x => (x.GetRelationType() == RelationType.Implements)
               || (x.GetRelationType() == RelationType.Extends)).ToList();
                ConcreteProductChecks(edge.GetDestination(), inheritanceEdgeRelations);
            }
        }

        /// <summary>
        ///     Function to check if a product node is implemented as an abstract class
        /// </summary>
        /// <param name="node"></param>
        private void AbstractProductChecks(IEntityNode node)
        {
            var classCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => (x.CheckModifier("abstract") && (x.CheckTypeDeclaration(EntityNodeType.Class))) , "if using a class the modifier should be abstract otherwise use an interface")
            }, x => new List<IEntityNode> { node }, "Product is:");
            result.Results.Add(classCheck.Check(node));

            var fieldCheck = new GroupCheck<IEntityNode, IField>(new List<ICheck<IField>>
            {
                new ElementCheck<IField>(x => x.CheckMemberModifier("abstract"), "all methods should be abstract!")
            }, x => node.GetFields(), "Field:", GroupCheckType.All);
            result.Results.Add(fieldCheck.Check(node));
        }

        /// <summary>
        ///     Function to check if a product node is an interface
        /// </summary>
        /// <param name="node"></param>
        private void InterFaceProductChecks(IEntityNode node)
        {
            var classCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => x.CheckTypeDeclaration(EntityNodeType.Interface), "if using a class the modifier should be abstract otherwise use an interface")
            }, x => new List<IEntityNode> { node }, "Product is:");
            result.Results.Add(classCheck.Check(node));
        }

        /// <summary>
        ///     Function to check if node is a concrete product
        /// </summary>
        /// <param name="node"></param>
        /// <param name="inheritanceRelations"></param>
        private void ConcreteProductChecks(IEntityNode node, List<IRelation> inheritanceRelations)
        {
            foreach (var edge in inheritanceRelations)
            {
                var edgeNode = edge.GetDestination();
                //check if node is an interface or an abstract class
                var check = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                {
                    new ElementCheck<IEntityNode>(x => x.CheckTypeDeclaration(EntityNodeType.Interface) |
                                                       (x.CheckTypeDeclaration(EntityNodeType.Class) && (x.CheckModifier("abstract"))),"class should implement interface or extends an abstract class")
                }, x => new List<IEntityNode> { edgeNode }, "ConcreteProduct implements/extends:");
                result.Results.Add(check.Check(node));
            }
        }

        /// <summary>
        ///     Function to check if a node is a creator
        /// </summary>
        /// <param name="node"></param>
        /// <param name="usingRelations"></param>
        private void AbstractCreatorChecks(IEntityNode node, List<IRelation> usingRelations)
        {
            var relations = node.GetRelations();

            foreach (var edge in relations)
            {
                var inheritanceRelations = edge.GetDestination().GetRelations().Where(x => (x.GetRelationType() == RelationType.Implements)
                || (x.GetRelationType() == RelationType.Extends)).ToList();

                var edgeRelations = edge.GetDestination().GetRelations();

                //create list with only creates relations
                var createsRelations = edgeRelations.Where(x => x.GetRelationType() == RelationType.Creates).ToList();

                ConcreteCreatorChecks(edge.GetDestination(), inheritanceRelations, createsRelations);
            }

            var classCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => (x.CheckTypeDeclaration(EntityNodeType.Class) && (x.CheckModifier("abstract"))), "class should be abstract!")

            }, x => new List<IEntityNode> { node }, "Class:");
            result.Results.Add(classCheck.Check(node));

            foreach (var edge in usingRelations)
            {
                var edgeNode = edge.GetDestination();
                ProductChecks(edgeNode);
                if (edgeNode.CheckTypeDeclaration(EntityNodeType.Interface))
                {

                    InterFaceProductChecks(edgeNode);

                }
                else if (edgeNode.CheckTypeDeclaration(EntityNodeType.Class) && edgeNode.CheckModifier("abstract"))
                {
                    AbstractProductChecks(edgeNode);
                    var methodCheck = new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                    {
                        new ElementCheck<IMethod>(x => x.CheckModifier("abstract"), "Modifier should be abstract"),
                        new ElementCheck<IMethod>(x => x.CheckModifier("public"), "Modifier should be public"),
                        new ElementCheck<IMethod>(x => x.CheckReturnType(edgeNode.GetName()),$"return type should be equal to {edgeNode.GetName()}"),
                    }, x => x.GetMethods(), "Creator has method where:");
                    result.Results.Add(methodCheck.Check(node));
                }
            }
        }

        /// <summary>
        ///     Function to check if a node is a concrete creator
        /// </summary>
        /// <param name="node"></param>
        /// <param name="inheritanceRelations"></param>
        /// <param name="creationRelations"></param>
        private void ConcreteCreatorChecks(IEntityNode node, List<IRelation> inheritanceRelations, List<IRelation> creationRelations)
        {
            foreach (var edge in inheritanceRelations)
            {
                var edgeNode = edge.GetDestination();
                //check if node is an interface or an abstract class
                var check = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                {
                    new ElementCheck<IEntityNode>(x => x.CheckTypeDeclaration(EntityNodeType.Interface) |
                                                       (x.CheckTypeDeclaration(EntityNodeType.Class) && (x.CheckModifier("abstract"))),"interface or an abstract class")
                }, x => new List<IEntityNode> { edgeNode }, "ConcreteCreator has:");
                result.Results.Add(check.Check(edgeNode));
            }

            foreach (var edge in creationRelations)
            {
                var edgeNode = edge.GetDestination();
                //check if state makes other state in handle method and check if the return type is void
                var createCheck = new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                {
                    //TO DO: check return type
                    new ElementCheck<IMethod>(x => x.CheckCreationType(edgeNode.GetName()), $"{node.GetName()} should return new conrete product") //TO DO: fix dat je het juiste product returnt
                    //TO DO: check of de functie de zelfde naam heeft als de overervende functie                                       
                }, x => x.GetMethods(), "Concrete creator has methods where:");

                result.Results.Add(createCheck.Check(node));
            }
        }
    }
}
