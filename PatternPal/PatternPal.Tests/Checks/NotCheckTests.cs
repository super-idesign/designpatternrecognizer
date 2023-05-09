﻿#region

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks;

internal class NotCheckTests
{
    [Test]
    public void Not_Check_Should_Not_Be_Called_Directly()
    {
        NotCheck notCheck = Not(
            Priority.Low,
            Modifiers(
                Priority.Low,
                Modifier.Abstract));

        IClass classEntity = EntityNodeUtils.CreateClass();
        RecognizerContext ctx = new();

        Assert.Throws< UnreachableException >(
            () => notCheck.Check(
                ctx,
                classEntity));
    }

    [Test]
    public Task Single_Modifier_Correct_NotCheck_Test()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();
        RecognizerContext ctx = new();

        CheckCollection checkCollection = Any(
            Priority.Low,
            Not(
                Priority.Low,
                Modifiers(
                    Priority.Low,
                    Modifier.Private)));

        ICheckResult result = checkCollection.Check(
            ctx,
            classEntity);
        return Verifier.Verify(result);
    }

    [Test]
    public Task Single_Modifier_Incorrect_NotCheck_Test()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();
        RecognizerContext ctx = new();

        CheckCollection checkCollection = Any(
            Priority.Low,
            Not(
                Priority.Low,
                Modifiers(
                    Priority.Low,
                    Modifier.Public)));

        ICheckResult result = checkCollection.Check(
            ctx,
            classEntity);
        return Verifier.Verify(result);
    }
}
