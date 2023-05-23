﻿using PatternPal.SyntaxTree.Abstractions.Entities;

namespace PatternPal.Tests.Checks;

[TestFixture]
public class TypeCheckTests
{
    [Test]
    public Task Type_Check_Returns_Correct_Result()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();
        
        TypeCheck typeCheck = new(
            Priority.Low,
            OneOf<ICheck, GetCurrentEntity >.FromT1(
                ICheck.GetCurrentEntity) );

        IRecognizerContext ctx = RecognizerContext4Tests.WithEntity(classEntity );

        ICheckResult result = typeCheck.Check(
            ctx,
            classEntity);
        return Verifier.Verify(result);
    }

    [Test]
    public Task Type_Check_Returns_Incorrect_Result()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();
        IMethod method = EntityNodeUtils.CreateMethod();

        TypeCheck typeCheck = new(
            Priority.Low,
            OneOf< ICheck, GetCurrentEntity >.FromT1(
                ICheck.GetCurrentEntity) );

        IRecognizerContext ctx = RecognizerContext4Tests.WithEntity(classEntity);

        ICheckResult result = typeCheck.Check(
            ctx,
            method);
        return Verifier.Verify(result);
    }
}
