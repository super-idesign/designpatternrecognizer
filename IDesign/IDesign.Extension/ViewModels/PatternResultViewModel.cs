﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using IDesign.Core.Models;
using IDesign.Recognizers.Abstractions;
using SyntaxTree.Abstractions.Entities;

namespace IDesign.Extension.ViewModels
{
    public class PatternResultViewModel
    {
        public PatternResultViewModel(RecognitionResult result)
        {
            Result = result;
        }

        public RecognitionResult Result { get; set; }
        public string PatternName => Result.Pattern.Name;
        public int Score => Result.Result.GetScore();

        public List<Named> Childs
        {
            get
            {
                var result = new List<Named>();
                if (Improvements.Any())
                {
                    result.Add(new PatternResultPartViewModel("Improvements ", Improvements.ToList()));
                }

                if (Requirements.Any())
                {
                    result.Add(new PatternResultPartViewModel("All requirements ", Requirements.ToList()));
                }

                return result;
            }
        }

        public SolidColorBrush Color => GetColor(Result.Result.GetScore());

        public IEnumerable<object> Requirements => Result.Result.GetResults().Select(x => new CheckResultViewModel(x));

        public IEnumerable<object> Improvements =>
            AddImprovementsFromResults(Result.Result.GetResults(), new List<CheckResultViewModel>());

        public IEntity EntityNode { get; internal set; }

        public SolidColorBrush GetColor(int score)
        {
            return score < 40 ? Brushes.Red : score < 80 ? Brushes.Yellow : Brushes.Green;
        }

        public FeedbackType GetFeedbackType()
        {
            var score = Result.Result.GetScore();
            if (score < 40)
            {
                return FeedbackType.Incorrect;
            }

            if (score < 80)
            {
                return FeedbackType.SemiCorrect;
            }

            return FeedbackType.Correct;
        }

        public IList<CheckResultViewModel> AddImprovementsFromResults(
            IEnumerable<ICheckResult> results,
            IList<CheckResultViewModel> destination
        )
        {
            foreach (var result in results)
            {
                var childFeedback = result.GetChildFeedback();
                if (childFeedback.Any())
                {
                    AddImprovementsFromResults(childFeedback, destination);
                }

                else if (result.GetFeedbackType() == FeedbackType.Incorrect)
                {
                    destination.Add(new CheckResultViewModel(result));
                }
            }

            return destination;
        }
    }

    public interface Named
    {
        string Name { get; }
    }

    public class PatternResultPartViewModel : Named
    {
        public PatternResultPartViewModel(string name, List<object> childViewModels)
        {
            Name = name;
            ChildViewModels = childViewModels;
        }

        public List<object> ChildViewModels { get; set; }
        public string Name { get; set; }
    }
}
