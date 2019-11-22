﻿using IDesign.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace IDesign.Checks
{
    public static class MethodChecks
    {
        /// <summary>
        /// Function thats checks the returntype of a method
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="returnType">The expected return type</param>
        /// <returns>
        public static bool CheckReturnType(this IMethod method, string returnType)
        /// </returns>
        {
            return method.GetReturnType().ToString().IsEqual(returnType);
        }


        /// <summary>
        /// Return a boolean based on if the given member has an expected modifier
        /// </summary>
        /// <param name="membersyntax">The member witch it should check</param>
        /// <param name="modifier">The expected modifier</param>
        /// <returns></returns>
        /// 
        /// <summary>
        /// Return a boolean based on if the given method is creational
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns></returns>
        public static bool CheckCreationalFunction(this MethodDeclarationSyntax methodSyntax)
        { 
            return methodSyntax.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any();
        public static bool CheckMemberModifier(this IMethod method, string modifier)
        {
            return method.GetModifiers().Where(x => x.ToString().IsEqual(modifier)).Any();
        }
    }
}
