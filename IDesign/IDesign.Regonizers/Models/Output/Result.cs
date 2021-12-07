﻿using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Recognizers.Models.Output
{
    public class Result : IResult
    {
        public Dictionary<IEntityNode, string> RelatedSubTypes = new Dictionary<IEntityNode, string>();
        public List<ICheckResult> Results { get; set; } = new List<ICheckResult>();

        public int GetScore()
        {
            var total = Results.Sum(x => x.GetTotalChecks());
            var green = Results.Sum(x => x.GetScore());
            return total <= 0 ? 0 : (int)(green / total * 100f);
        }

        public IList<ICheckResult> GetResults()
        {
            return Results;
        }

        public IDictionary<IEntityNode, string> GetRelatedSubTypes()
        {
            return RelatedSubTypes;
        }
    }
}
