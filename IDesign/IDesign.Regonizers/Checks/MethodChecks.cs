﻿using System;
using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers.Checks
{
    public static class MethodChecks
    {
        /// <summary>
        ///     Function thats checks the returntype of a method.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="returnType">The expected return type</param>
        /// <returns></returns>
        public static bool CheckReturnType(this IMethod methodSyntax, string returnType)
        {
            return methodSyntax.GetReturnType().IsEqual(returnType);
        }

        /// <summary>
        ///     Return a boolean based on if the given method is creational.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns></returns>
        public static bool CheckCreationalFunction(this IMethod methodSyntax)
        {
            return methodSyntax.GetBody().DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any();
        }

        /// <summary>
        ///     Return a boolean based on if the given member has an expected modifier.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="modifier">The expected modifier</param>
        /// <returns></returns>
        public static bool CheckModifier(this IMethod methodSyntax, string modifier)
        {
            return methodSyntax.GetModifiers().Any(x => x.ToString().IsEqual(modifier));
        }

        /// <summary>
        ///     Return a boolean based on if the given method creates an object with the given type.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="creationType">The expected creational type</param>
        /// <returns></returns>
        public static bool CheckCreationType(this IMethod methodSyntax, string creationType)
        {
            var body = methodSyntax.GetBody();
            if (body == null)
                return false;
            var creations = body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
            foreach (var creationExpression in creations)
                if (creationExpression.Type is IdentifierNameSyntax name &&
                    name.Identifier.ToString().IsEqual(creationType))
                    return true;

            return false;
        }

        /// <summary>
        ///     Return a boolean based on if the given method returns a object it creates.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns></returns>
        public static bool CheckReturnTypeSameAsCreation(this IMethod methodSyntax)
        {
            return methodSyntax.CheckCreationType(methodSyntax.GetReturnType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name="currentClass"></param>
        /// <returns></returns>
        public static bool IsInterfaceMethod(this IMethod methodSyntax, IEntityNode currentClass)
        {
            return currentClass.ClassImlementsInterfaceMethod(methodSyntax);
        }

        //helper functions
        /// <summary>
        ///     Return a list of all types that this function makes as strings.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns>all types that are created</returns>
        public static IEnumerable<string> GetCreatedTypes(this IMethod methodSyntax)
        {
            var result = new List<string>();
            try
            {
                var creations = methodSyntax.GetBody().DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                foreach (var creation in creations)
                {
                    var identifiers = creation.DescendantNodes().OfType<IdentifierNameSyntax>();
                    result.AddRange(identifiers.Select(y => y.Identifier.ToString()));
                }
            }
            catch (Exception e)
            {
                _ = e.Message;
            }
            return result;
        }

        /// <summary>
        /// Return a boolean based on if the method has the same name as the given string
        /// </summary>
        /// <param name="methodSyntax">The given method which should have the name</param>
        /// <param name="name">The name the method should have</param>
        /// <returns></returns>
        public static bool CheckMethodIdentifier(this IMethod methodSyntax, string name)
        {
            return (methodSyntax.GetName().Equals(name) && methodSyntax.GetType() == typeof(Method));
        }

        /// <summary>
        /// Return a boolean based on if the Method parameters are the same as the given string
        /// </summary>
        /// <param name="methodSyntax">The method it should check the parameters from</param>
        /// <param name="parameters">The given parameters types it should have</param>
        /// <returns>The method has the same parameters as the given string</returns>
        public static bool CheckMethodParameterTypes(this IMethod methodSyntax, string parameters)
        {
            return methodSyntax.GetParameter().ToString().Equals(parameters);
        }

        /// <summary>
        /// Return a boolean based on if the given method is the same type as the other method
        /// </summary>
        /// <param name="methodSyntax">The method it should check</param>
        /// <param name="compareMethod">The given method it should compare to</param>
        /// <returns>The methods are the same type</returns>
        public static bool IsEquals(this IMethod methodSyntax, IMethod compareMethod)
        {
            return (methodSyntax.CheckMethodIdentifier(compareMethod.GetName())
                && methodSyntax.CheckMethodParameterTypes(compareMethod.GetParameter().ToString())
                && methodSyntax.CheckReturnType(compareMethod.GetReturnType()));
        }
    }
}
