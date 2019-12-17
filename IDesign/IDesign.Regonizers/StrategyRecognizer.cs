﻿using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models;
using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;

namespace IDesign.Recognizers
{
    public class StrategyRecognizer : IRecognizer
    {
        Result result;

        public IResult Recognize(IEntityNode node)
        {
            result = new Result();

            //if node is interface, node is probaly a strategy. else node can be strategy, context or concrete strategy
            if (node.GetEntityNodeType() == EntityNodeType.Interface)
            {
                StrategyChecks(node);
                InterfaceStrategyChecks(node);
            }
            else if (node.GetEntityNodeType() == EntityNodeType.Class)
            {
                //if node is an abstract class, node is probaly a strategy
                if (node.CheckModifier("abstract"))
                {
                    StrategyChecks(node);
                    AbstractClassStrategyChecks(node);
                }
            }
            return result;
        }

        /// <summary>
        ///     Function to check if pattern is strategy
        /// </summary>
        /// <param name="node"></param>
        private void StrategyChecks(IEntityNode node)
        {
            var relations = node.GetRelations();

            //create list with only Extends and Implements relations
            var inheritanceRelations = relations.Where(x => (x.GetRelationType() == RelationType.ImplementedBy) ||
            (x.GetRelationType() == RelationType.ExtendedBy)).ToList();

            foreach (var edge in relations.Where(x => x.GetRelationType() == RelationType.UsedBy).ToList())
            {
                var edgeRelations = edge.GetDestination().GetRelations();
                var usingEdgeRelations = edgeRelations.Where(x => x.GetRelationType() == RelationType.Uses).ToList();
                ContextClassChecks(edge.GetDestination(), usingEdgeRelations);
            }

            foreach (var edge in inheritanceRelations)
            {
                var edgeRelations = edge.GetDestination().GetRelations();

                //create list with only Extends and Implements relations
                var inheritanceEdgeRelations = edgeRelations.Where(x => (x.GetRelationType() == RelationType.Implements) ||
                (x.GetRelationType() == RelationType.Extends)).ToList();
                ConcreteStrategyClassChecks(edge.GetDestination(), inheritanceEdgeRelations);
            }
        }

        /// <summary>
        ///     Function to do checks for a node that is probaly a strategy, implemented as an interface
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private void InterfaceStrategyChecks(IEntityNode node)
        {
            var checkType = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => x.CheckTypeDeclaration(EntityNodeType.Interface),
                    "If using a class, the modifier should be abstract. Otherwise, use an interface")

            }, x => new List<IEntityNode> { node }, "");

            result.Results.Add(checkType.Check(node));
            var checkMethods = new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
            {
                new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "return type should be void")

            }, x => x.GetMethods(), "");
            result.Results.Add(checkMethods.Check(node));
        }

        /// <summary>
        ///     Function to do checks for a node that is probaly a strategy, implemented as an abstract class
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private void AbstractClassStrategyChecks(IEntityNode node)
        {
            //check if node is an interface or an abstract class, cannot be both
            var checkType = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => x.CheckModifier("abstract"),
                    "If using a class, the modifier should be abstract. Otherwise, use an interface")

            }, x => new List<IEntityNode> { node }, "");
            result.Results.Add(checkType.Check(node));

            var abstractStateCheck = new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
            {
                new ElementCheck<IMethod>(x => x.CheckModifier("abstract"),
                    "If using a class, the modifier should be abstract. Otherwise, use an interface"),
                new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "return type should be void"),
                new ElementCheck<IMethod>(x => x.GetBody() == null, "Body should be empty!")
            }, x => x.GetMethods(), "");
            result.Results.Add(abstractStateCheck.Check(node));
        }

        /// <summary>
        ///     Function to do checks for the concrete strategy class
        /// </summary>
        /// <param name="node"></param>
        /// <param name="inheritanceRelations"></param>
        /// <returns></returns>
        private void ConcreteStrategyClassChecks(IEntityNode node, List<IRelation> inheritanceRelations)
        {
            List<string> methodNamesList = new List<string>();

            foreach (var edge in inheritanceRelations)
            {
                var edgeNode = edge.GetDestination();

                //get all methodnames
                foreach (var name in edgeNode.GetMethods())
                {
                    methodNamesList.Add(name.GetName());
                }

                //check if node is an interface or an abstract class
                var check = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                {
                    new ElementCheck<IEntityNode>(x => x.CheckTypeDeclaration(EntityNodeType.Interface) |
                                                       (x.CheckTypeDeclaration(EntityNodeType.Class) &&
                                                        (x.CheckModifier("abstract"))), "message")
                }, x => new List<IEntityNode>() { edgeNode }, "");

                result.Results.Add(check.Check(edgeNode));
            }

            foreach (var methodName in methodNamesList)
            {
                //check if state makes other state in handle method and check if the return type is void
                var createCheck = new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                {
                    new ElementCheck<IMethod>(x => x.GetName() == methodName, "names should be equal!"),
                    new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "return type should be void!"),
                    //TO DO: check of functie de zelfte parameters heeft als de interface/abstracte klasse functie                     
                }, x => x.GetMethods(), "");

                result.Results.Add(createCheck.Check(node));
            }
        }

        /// <summary>
        ///     Function to do checks for the context class
        /// </summary>
        /// <param name="node"></param>
        /// <param name="usingRelations"></param>
        /// <returns></returns>
        private void ContextClassChecks(IEntityNode node, List<IRelation> usingRelations)
        {
            foreach (var edge in usingRelations)
            {
                var edgeNode = edge.GetDestination();

                if ((edgeNode.CheckTypeDeclaration(EntityNodeType.Interface)) |
                    (edgeNode.CheckTypeDeclaration(EntityNodeType.Class) && edgeNode.CheckModifier("abstract")))
                {
                    //check if state makes other state in handle method and check if the return type is void
                    var fieldCheck = new GroupCheck<IEntityNode, IField>(new List<ICheck<IField>>
                    {
                        new ElementCheck<IField>(x => x.CheckFieldType(new List<string>(){ edgeNode.GetName() }),
                            $"{node.GetName()} must be equal to {edgeNode.GetName()}"),
                        new ElementCheck<IField>(x => x.CheckMemberModifier("private"), "modifier must be private")
                    }, x => x.GetFields(), "Context had method where:");
                    result.Results.Add(fieldCheck.Check(node));
                }
            }
        }
    }
}
