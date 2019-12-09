using IDesign.Recognizers.Abstractions;
using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;

namespace IDesign.Recognizers
{
    public class SingletonRecognizer : IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();

            var methodChecks = new List<ICheck<IMethod>>
            {
                new ElementCheck<IMethod>(x => x.CheckReturnType(entityNode.GetName()), "Incorrect return type"),
                new ElementCheck<IMethod>(x => x.CheckModifier("static"), "Is not static"),
                new ElementCheck<IMethod>(x => x.CheckReturnTypeSameAsCreation(),
                    "Return type isnt the same as created")
            };

            var singletonCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new GroupCheck<IEntityNode, IMethod>(methodChecks, x => x.GetMethods(), "Has GetInstance()")
            }, x => new List<IEntityNode> { entityNode }, "Singleton");


            var r = singletonCheck.Check(entityNode);

            result.Results = r.GetChildFeedback().ToList();
            return result;
        }
    }
}