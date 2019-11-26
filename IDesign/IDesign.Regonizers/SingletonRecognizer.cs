﻿using IDesign.Checks;
using IDesign.Models;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Output;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IDesign.Recognizers
{
    public class SingletonRecognizer : IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();
            var methods = entityNode.GetMethods();
            var fields = entityNode.GetFields();
            var constructors = entityNode.GetConstructors();
            var methodChecks = new List<(Predicate<IMethod> check, string suggestionMessage)>()
            {
                (x => x.CheckReturnType(entityNode.GetName()) , "Incorrecte return type"),
                (x => x.CheckModifier("static") , "Is niet static"),
                (x => x.CheckReturnTypeSameAsCreation(), "Return type is niet hetzelfde als wat er gemaakt wordt" )
            };
            CheckElements(result, entityNode.GetMethods(), x => x.GetName(), methodChecks);

            var propertyChecks = new List<(Predicate<IField> check, string suggestionMessage)>()
            {
                (x => x.CheckFieldType(entityNode.GetName()) , "Incorrecte type"),
                (x => x.CheckMemberModifier("static") , "Is niet static"),
                (x => x.CheckMemberModifier("private") , "Is niet private")
            };
            CheckElements(result, entityNode.GetFields(), x => x.GetName(), propertyChecks);

            var constructorChecks = new List<(Predicate<IMethod> check, string suggestionMessage)>()
            {
                (x => !x.CheckModifier("public") , "Is public moet private of protected zijn")
            };
            CheckElements(result, entityNode.GetConstructors(), x => x.GetName(), constructorChecks);




            result.Score = (int)(result.Score / 7f * 100f);

            return result;
        }


        //@TODO Make the next fucntions accessible to other patterns
        private void CheckElements<T>(Result result, IEnumerable<T> elements, Func<T, string> elementName, IEnumerable<(Predicate<T> check, string suggestionMessage)> checks)
        {
            var checkResult = CheckElements(elements, elementName, checks);
            result.Score += checkResult.score;
            result.Suggestions.AddRange(checkResult.suggestions);
        }

        private (IList<ISuggestion> suggestions, int score) CheckElements<T>(IEnumerable<T> elements, Func<T, string> elementName, IEnumerable<(Predicate<T> check, string suggestionMessage)> checks)
        {
            var suggestionList = new List<ISuggestion>();

            var scores = new Dictionary<T, (int score, IList<string> suggestions)>();

            //Give scores to elements
            foreach (var element in elements)
            {
                var score = 0;
                var suggestions = new List<string>();

                foreach (var check in checks)
                {
                    var isValid = check.check(element);
                    score += isValid ? 1 : 0;

                    if (!isValid) suggestions.Add(check.suggestionMessage);
                }

                scores.Add(element, (score, suggestions));
            }

            //Add suggestions for best scored element
            foreach (var elementScore in scores)
            {
                //If element has the highest score
                if (elementScore.Value.score == scores.Values.Select(x => x.score).Max())
                {
                    foreach (var propertySuggestion in elementScore.Value.suggestions)
                    {
                        suggestionList.Add(new Suggestion(elementName(elementScore.Key) + ": " + propertySuggestion));
                    }

                    return (suggestionList, elementScore.Value.score);
                }
            }

            return (suggestionList, 0);
        }
    }
}
