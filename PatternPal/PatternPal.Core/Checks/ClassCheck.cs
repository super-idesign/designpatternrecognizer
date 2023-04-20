﻿namespace PatternPal.Core.Checks;

internal class ClassCheck : CheckBase
{
    private readonly IEnumerable< ICheck > _checks;

    internal ClassCheck(Priority priority, 
        IEnumerable<ICheck> checks) : base(priority)
    {
        _checks = checks;
    }

    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        IClass classEntity = CheckHelper.ConvertNodeElseThrow< IClass >(node);

        IList< ICheckResult > subCheckResults = new List< ICheckResult >();
        foreach (ICheck check in _checks)
        {
            switch (check)
            {
                case ModifierCheck modifierCheck:
                {
                    subCheckResults.Add(modifierCheck.Check(ctx, classEntity));
                    break;
                }
                case MethodCheck methodCheck:
                {
                    foreach (IMethod method in classEntity.GetMethods())
                    {
                        subCheckResults.Add(methodCheck.Check(ctx, method));
                    }
                    break;
                }
                case FieldCheck fieldCheck:
                {
                    foreach (IField field in classEntity.GetFields())
                    {
                        subCheckResults.Add(fieldCheck.Check(ctx, field));
                    }
                    break;
                }
                case ConstructorCheck constructorCheck:
                {
                    foreach (IConstructor constructor in classEntity.GetConstructors())
                    {
                        subCheckResults.Add(constructorCheck.Check(ctx, constructor));
                    }
                    break;
                }
                case NotCheck:
                {
                    throw new NotImplementedException("Class Check was incorrect");
                }
                case UsesCheck usesCheck:
                {
                    subCheckResults.Add(usesCheck.Check(ctx, classEntity));
                    break;
                }
                default:
                    throw CheckHelper.InvalidSubCheck(
                        this,
                        check);
            }
        }

        return new NodeCheckResult
               {
                   ChildrenCheckResults = subCheckResults,
                   FeedbackMessage = $"Found class '{classEntity}'",
                   Priority = Priority
               };
    }
}
