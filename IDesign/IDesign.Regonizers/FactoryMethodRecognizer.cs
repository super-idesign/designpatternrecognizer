﻿using System;
using System.Collections.Generic;
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
        public IResult Recognize(IEntityNode node)
        {
            result = new Result();
            IEntityNode entityNode = null;
            IEntityNode productnode = null;
            var factoryMethodChecks = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                //check if node is abstract class (creator)
                new ElementCheck<IEntityNode>(x => x.CheckTypeDeclaration(EntityNodeType.Class)&& x.CheckModifier("abstract"),new ResourceMessage("NodeModifierAbstract")),
                new ElementCheck<IEntityNode>(x=> x.CheckMinimalAmountOfRelationTypes(RelationType.Uses,1),new ResourceMessage("NodeUses1")),

                //check concretecreators
                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                {
                    //concrete creator
                    new ElementCheck<IEntityNode>(x => {entityNode = x; return x.GetMethods().Any(); }, new ResourceMessage("FactoryConcreteCreatorMethodAny")),

                    //check if node (concrete creator) has creates relations
                    new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                    {
                        //concrete product
                        new ElementCheck<IEntityNode>(x => {productnode = x; return x.GetMethods().Any(); }, new ResourceMessage("FactoryConcreteProductMethodAny")),

                        //check if method in node (concrete creator) creates new node (concrete product)
                        new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckCreationType(productnode.GetName()), new ResourceMessage("FactoryMethodCreateTypeProduct")),
                        }, x => entityNode.GetMethods(), "FactoryAbstractCreatorMethod"),

                        //check if node (concrete product) implements/extends a node (product)
                        new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                        {
                            //product
                            new ElementCheck<IEntityNode>(x => {productnode = x; return x.GetMethods().Any() || x.GetFields().Any(); }, "ProductClass"),

                            new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                            {
                                new ElementCheck<IMethod>(x => x.CheckReturnType(productnode.GetName()), new ResourceMessage("FactoryMethodCreateTypeProduct")),
                            }, x => node.GetMethods(), "FactoryAbstractCreatorMethod")

                        }, x => x.GetRelations().Where(y => (y.GetRelationType().Equals(RelationType.Extends)) ||
                        (y.GetRelationType().Equals(RelationType.Implements)))
                        .Select(y => y.GetDestination()),new ResourceMessage("Child")),

                    }, x=> entityNode.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.Creates))
                    .Select(y => y.GetDestination()), "FactoryCreates")

                }, x => x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.ExtendedBy))
                .Select(y => y.GetDestination()), "FactoryConcreteCreator", GroupCheckType.All),

                //product node
                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                {
                    //product
                    new ElementCheck<IEntityNode>(x => {entityNode = x; return x.GetMethods().Any() || x.GetFields().Any(); }, "ProductClass"),

                    //check if node (creator) has uses relations (method) with return type of interface node (product)
                    new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                    {
                         new ElementCheck<IMethod>(x => x.CheckModifier("public"),new ResourceMessage("MethodModifierPublic")),
                         new ElementCheck<IMethod>(x => x.CheckModifier("abstract"),new ResourceMessage("MethodModifierAbstract")),
                         new ElementCheck<IMethod>(x => x.CheckReturnType(entityNode.GetName()),new ResourceMessage("FactoryMethodReturnTypeProductInterface"))
                    }, x=> node.GetMethods(), "FactoryConcreteCreatorMethod")

                }, x=> x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.Uses))
                .Select(y => y.GetDestination()),"ProductClass", GroupCheckType.All)

            }, x => new List<IEntityNode> { node }, "FactoryAbstractCreator");

            result.Results.Add(factoryMethodChecks.Check(node));
            return result;
        }
    }
}
