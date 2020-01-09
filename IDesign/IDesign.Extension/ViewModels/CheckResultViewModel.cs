﻿using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using IDesign.CommonResources;

namespace IDesign.Extension.ViewModels
{
    public class CheckResultViewModel
    {
        public CheckResultViewModel(ICheckResult result)
        {
            Result = result;
            Message = CommonResources.ResourceUtils.ResultToString(Result);
        }

        public ICheckResult Result { get; set; }
        public string Message { get; set; }
        public float Score => Result.GetScore();
        public FeedbackType Type => Result.GetFeedbackType();

        public ImageMoniker Icon
        {
            get
            {
                switch (Type)
                {
                    case FeedbackType.Correct:
                        return KnownMonikers.StatusOK;
                    case FeedbackType.SemiCorrect:
                        return KnownMonikers.StatusExcluded;
                    case FeedbackType.Incorrect:
                        return KnownMonikers.StatusError;
                }

                return KnownMonikers.StatusHelp;
            }
        }

        public IEnumerable<CheckResultViewModel> SubResults =>
            Result.GetChildFeedback().Select(x => new CheckResultViewModel(x));
    }
}
