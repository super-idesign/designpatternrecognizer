﻿using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;

namespace IDesign.Recognizers
{
    public class AdapterRecognizer : IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();
           
            var objectAdapterResult = GetObjectAdapterCheck(entityNode).Check(entityNode);
            var classAdapterResult = GetInheritanceAdapterCheck(entityNode).Check(entityNode);

            if ((float)objectAdapterResult.GetTotalChecks() / objectAdapterResult.GetScore() < (float)classAdapterResult.GetTotalChecks() / classAdapterResult.GetScore())
                result.Results = objectAdapterResult.GetChildFeedback().ToList();
            else
                result.Results = classAdapterResult.GetChildFeedback().ToList();
            return result;
        }

        private GroupCheck<IEntityNode, IEntityNode> GetObjectAdapterCheck(IEntityNode entityNode)
        {
            IRelation currentRelation = null;
            string currentField = null;
            var adapterCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                GetImplementsInterfaceOrExtendsClassCheck(),
                new GroupCheck<IEntityNode, IRelation>(new List<ICheck<IRelation>>
                {
                    //Is used by adapter    
                    new ElementCheck<IRelation>(x =>
                    {
                        currentRelation = x;
                        return x.GetRelationType() == RelationType.Uses;
                    }, "Adaptee is used by adapter"),

                    //Adapter has an adaptee field
                    new GroupCheck<IRelation, IField>(new List<ICheck<IField>>
                    {
                        new ElementCheck<IField>(x =>
                        {
                            currentField = x.GetName();
                            return x.CheckFieldType(new List<string>{ currentRelation.GetDestination().GetName() });
                        }, "Field has adaptee as type", 2),
                        //Every method uses the adaptee 
                        new GroupCheck<IField, IMethod>(new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckFieldIsUsed(currentField), "Method uses adpatee", 2),
                            new ElementCheck<IMethod>(x => !x.CheckReturnType(currentField), "Method does not return adaptee", 1),
                            new ElementCheck<IMethod>(x => x.IsInterfaceMethod(entityNode) || x.CheckModifier("override"),
                                "Method overrides or is ipmlemented", 1),

                        }, x => entityNode.GetMethods(), "Is used in every adapter method", GroupCheckType.Median)

                    }, x => entityNode.GetFields(), "Adapter has an adaptee", GroupCheckType.Median)
                }, node => node.GetRelations(), "Has adaptee")
            }, x => new List<IEntityNode> { entityNode }, "Object adapter", GroupCheckType.All);


            return adapterCheck;
        }

        private GroupCheck<IEntityNode, IEntityNode> GetInheritanceAdapterCheck(IEntityNode entityNode)
        {
            IRelation currentRelation = null;
            var adapterCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                GetImplementsInterfaceOrExtendsClassCheck()                ,
                new GroupCheck<IEntityNode, IRelation>(new List<ICheck<IRelation>>
                {
                    //Is used by adapter (parent)    
                    new ElementCheck<IRelation>(x =>
                    {
                        currentRelation = x;
                        return x.GetRelationType() == RelationType.Extends ||
                               x.GetRelationType() == RelationType.Implements
                            ;
                    }, "Extends an the adapter", 2),

                        new GroupCheck<IRelation, IMethod>(new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckIfMethodCallsMethodInNode(currentRelation.GetDestination()), "Method uses adpatee", 2),
                            new ElementCheck<IMethod>(x => !x.CheckReturnType(currentRelation.GetDestination().GetName()), "Method does not return adaptee", 1),
                            new ElementCheck<IMethod>(x => x.IsInterfaceMethod(entityNode) || x.CheckModifier("override"),
                                "Method is overriden or implemted", 1),

                        }, x => entityNode.GetMethods(), "Every method calls adaptee", GroupCheckType.Median)

                }, node => node.GetRelations(), "Has adaptee")
            }, x => new List<IEntityNode> { entityNode }, "Class adapter", GroupCheckType.All);

            return adapterCheck;
        }

        private ElementCheck<IEntityNode> GetImplementsInterfaceOrExtendsClassCheck()
        {
            return new ElementCheck<IEntityNode>(
                    x => x.GetRelations().Any(y => y.GetRelationType() == RelationType.Implements || y.GetRelationType() == RelationType.Extends),
                    "Implements interface or extends class");
        }
    }
}