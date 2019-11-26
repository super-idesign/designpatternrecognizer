﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDesign.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Checks
{
    public static class FieldChecks
    {
        /// <summary>
        /// Return a boolean based on if the given field is an expected type
        /// </summary>
        /// <param name="fieldSyntax">The field witch it should check</param>
        /// <param name="type">The expected type</param>
        /// <returns></returns>
        public static bool CheckFieldType(this IField fieldSyntax, string type)
        {
            return fieldSyntax.GetFieldType().ToString().IsEqual(type);
        }


        /// <summary>
        /// Return a boolean based on if the given field has an expected modifier
        /// </summary>
        /// <param name="field">The field witch it should check</param>
        /// <param name="modifier">The expected modifier</param>
        /// <returns></returns>
        public static bool CheckMemberModifier(this IField field, string modifier)
        {
            return field.GetModifiers().Where(x => x.ToString().IsEqual(modifier)).Any();
        }
    }
}
