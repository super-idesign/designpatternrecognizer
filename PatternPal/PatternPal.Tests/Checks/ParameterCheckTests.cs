﻿#region
using static PatternPal.Core.Checks.CheckBuilder;
#endregion

namespace PatternPal.Tests.Checks;

[TestFixture]
public class ParameterCheckTests
{
    [Test]
    public Task Multiple_Same_parameters_Correct()
    {
        SyntaxGraph graph = EntityNodeUtils.CreateMethodWithParamaters();
        RecognizerContext ctx = new() { Graph = graph };

        // Obtain the StringTestFunction method (3 parameters)
        IMethod stringNode =
            graph.GetAll()["StringTest"].GetMethods().FirstOrDefault(
                x => x.GetName() == "StringTestFunction");

        // Obtain the IntTest method (1 StringTest parameter)
        IMethod intNode =
            graph.GetAll()["IntTest"].GetMethods().FirstOrDefault(
                x => x.GetName() == "IntTestFunction");

        // Create same typecheck for two different parameters and one other type parameter
        TypeCheck typeIntNode1 = new TypeCheck(
            Priority.Low,
            OneOf<Func<List<INode>>, GetCurrentEntity>.FromT0(
                () => new List<INode>
                {
                    ctx.Graph.Relations.GetEntityByName(intNode.GetReturnType())
                }));
        TypeCheck typeIntNode2 = typeIntNode1;
        TypeCheck typeStringNode = new TypeCheck(
            Priority.Low,
            OneOf<Func<List<INode>>, GetCurrentEntity>.FromT0(
                () => new List<INode>
                {
                    ctx.Graph.Relations.GetEntityByName(stringNode.GetReturnType())
                }));

        List<TypeCheck> collectiontest = new List<TypeCheck>
        {
            typeIntNode1,
            typeIntNode2,
            typeStringNode
        };

        ParameterCheck usedParamCheck = new ParameterCheck(
            Priority.Low,
            collectiontest);

        MethodCheck method3 = new MethodCheck(
            Priority.Low,
            new List<ICheck>
            {
                usedParamCheck
            });

        ICheckResult res = method3.Check(ctx, stringNode);
        return Verifier.Verify(res);
    }

    [Test]
    public Task Parameter_Check_Method_Wrong_Type()
    {
        SyntaxGraph graph = EntityNodeUtils.CreateMethodWithParamaters();
        RecognizerContext ctx = new() { Graph = graph };

        // Obtain the StringTestFunction method (3 parameters)
        IMethod stringNode =
            graph.GetAll()["StringTest"].GetMethods().FirstOrDefault(
                x => x.GetName() == "StringTestFunction");

        // Obtain the IntTest method (1 StringTest parameter)
        IMethod intNode =
            graph.GetAll()["IntTest"].GetMethods().FirstOrDefault(
                x => x.GetName() == "IntTestFunction");

        // TypeCheck of the StringTestFunction method (return type is StringTest)
        TypeCheck typeIntNode = new TypeCheck(
            Priority.Low,
            OneOf<Func<List<INode>>, GetCurrentEntity>.FromT0(
                () => new List<INode> { ctx.Graph.Relations.GetEntityByName(intNode.GetReturnType()) }));
        var test = ctx.Graph.Relations.GetEntityByName(intNode.GetReturnType());

        List<TypeCheck> collectiontest = new List<TypeCheck>
        {
            typeIntNode
        };

        ParameterCheck usedParamCheck = new ParameterCheck(
            Priority.Low,
            collectiontest);

        MethodCheck method3 = new MethodCheck(
            Priority.Low,
            new List<ICheck>
            {
                usedParamCheck
            });

        ICheckResult res = method3.Check(ctx, intNode);
        return Verifier.Verify(res);
    }

    [Test]
    public Task Parameter_Check_Method_Same_Type()
    {
        SyntaxGraph graph = EntityNodeUtils.CreateMethodWithParamaters();
        RecognizerContext ctx = new() { Graph = graph };

        // Obtain the StringTestFunction method (0 parameters)
        IMethod stringNode = 
            graph.GetAll()["StringTest"].GetMethods().FirstOrDefault(
                x => x.GetName() == "StringTestFunction");

        // Obtain the IntTest method (1 StringTest parameter)
        IMethod intNode = 
            graph.GetAll()["IntTest"].GetMethods().FirstOrDefault(
                x => x.GetName() == "IntTestFunction");

        // TypeCheck of the StringTestFunction method (return type is StringTest)
        TypeCheck typeStringNode = new TypeCheck(
            Priority.Low,
            OneOf<Func<List<INode>>, GetCurrentEntity>.FromT0(
                () => new List<INode> { ctx.Graph.Relations.GetEntityByName(stringNode.GetReturnType()) }));

        List<TypeCheck> collectiontest = new List<TypeCheck>
        {
            typeStringNode
        };

        ParameterCheck usedParamCheck = new ParameterCheck(
            Priority.Low,
            collectiontest);

        MethodCheck method3 = new MethodCheck(
            Priority.Low,
            new List<ICheck>
            {
                usedParamCheck
            });

        ICheckResult res = method3.Check(ctx, intNode);
        return Verifier.Verify(res);
    }


    // TODO add method in test file without parameters.
    [Test]
    public Task Parameter_Check_No_Parameters()
    {
        SyntaxGraph graph = EntityNodeUtils.CreateMethodWithParamaters();
        RecognizerContext ctx = new() { Graph = graph };

        // Obtain method with 0 parameters from syntax graph.
        IMethod stringNode =
            graph.GetAll()["StringTest"].GetMethods().FirstOrDefault(
                x => x.GetName() == "StringTestFunction");

        // Empty list of typechecks because check returns when checking parameters.
        ParameterCheck usedParamCheck =
            new ParameterCheck(Priority.Low, new List<TypeCheck> { });

        ICheckResult res = usedParamCheck.Check(ctx, stringNode);
        return Verifier.Verify(res);
    }
}
