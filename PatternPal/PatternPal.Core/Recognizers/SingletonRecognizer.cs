﻿#region
using PatternPal.SyntaxTree.Models;
using static PatternPal.Core.Checks.CheckBuilder;
#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided file is an implementation
/// of the singleton pattern.
/// </summary>
/// <remarks>
/// Requirements to fulfill the pattern:<br/>
/// a) has no public/internal constructor<br/>
/// b) has at least one private/protected constructor<br/>
/// c) has a static, private field with the same type as the class<br/>
/// d0) has a static, public/internal method that acts as a constructor in the following way<br/>
///     d1) if called and there is no instance saved in the private field, then it calls the private constructor<br/>
///     d2) if called and there is an instance saved in the private field it returns this instance<br/>
/// <br/>
/// Optional requirement client:<br/>
/// a) calls the method that acts as a constructor of the singleton class<br/>
/// </remarks>
internal class SingletonRecognizer : IRecognizer
{
    /// <summary>
    /// A method which creates a lot of <see cref="ICheck"/>s that each adheres to the requirements a singleton pattern needs to have implemented.
    /// It returns the requirements in a tree structure stated per class.
    /// </summary>
    public IEnumerable<ICheck> Create()
    {
        // Step 1: Checks for requirements Singleton a & b
        ICheck onlyPrivateConstructor =
            OnlyPrivateConstructor(out ConstructorCheck privateConstructorCheck);

        // Step 2: Check for requirement Singleton c
        FieldCheck staticPrivateFieldOfTypeClass =
            StaticPrivateFieldOfTypeClass();

        // Step 3: Check for requirement Singleton d0-d2
        ICheck checkMethodActsAsConstructorBehaviour = CheckMethodAcsAsConstructorBehaviour(
            privateConstructorCheck,
            staticPrivateFieldOfTypeClass,
            out MethodCheck hasStaticPublicInternalMethod);

        // Step 4: Check for requirement Client a
        MethodCheck checkClientA = ClientCallsMethodActsAsConstructor(hasStaticPublicInternalMethod);

        yield return Class(
            Priority.Low,
            onlyPrivateConstructor,
            staticPrivateFieldOfTypeClass,
            checkMethodActsAsConstructorBehaviour
        );

        yield return Class(
            Priority.Low,
            checkClientA
        );
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine a constructor is only.
    /// <see langword="private"/>.
    /// </summary>
    internal ICheck OnlyPrivateConstructor(out ConstructorCheck privateConstructorCheck)
    {
        return privateConstructorCheck = Constructor(
            Priority.Knockout,
            Modifiers(
                Priority.Knockout,
                Modifier.Private
            )
        );

        //NotCheck noPuclicConstructorCheck = Not(
        //    Priority.Knockout,
        //    Constructor(
        //        Priority.Knockout,
        //        Any(
        //            Priority.Knockout,
        //            Modifiers(
        //                Priority.Knockout,
        //                Modifier.Public),
        //            Modifiers(
        //                Priority.Knockout,
        //                Modifier.Internal),
        //            Modifiers(
        //                Priority.Knockout,
        //                Modifier.Protected
        //            )
        //        )
        //    )
        //);

        //return All(Priority.Low,
        //    privateConstructorCheck,
        //    noPuclicConstructorCheck);
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine that there is a field which
    /// is private and static in a given class.
    /// </summary>
    internal FieldCheck StaticPrivateFieldOfTypeClass()
    {
        return Field(
            Priority.Knockout,
            Modifiers(
                Priority.Knockout,
                Modifier.Static,
                Modifier.Private
            ),
            Type(
                Priority.Knockout,
                ICheck.GetCurrentEntity
            )
        );
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine that the class has a static and public/internal method
    /// </summary>
    internal MethodCheck HasStaticPublicInternalMethod()
    {
        return Method(
            Priority.Knockout,
            Modifiers(
                Priority.Knockout,
                Modifier.Static
            ),
            Any(
                Priority.Knockout,
                Modifiers(
                    Priority.Knockout,
                    Modifier.Public
                ),
                Modifiers(
                    Priority.Knockout,
                    Modifier.Internal
                )
            )
        );
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine that if a specific field is empty it calls the private constructor
    /// </summary>
    internal RelationCheck CallsPrivateConstructor(ConstructorCheck constructor)
    {
        //Right now it only checks if the constructor is called somewhere in a method, not at which conditions
        return Uses(
            Priority.Mid,
            constructor.Result
        );

        /*TODO: fix and use 'correct' implementation below
        //return Method(
        //    Priority.Mid,
        //    new IfThenOperatorCheck(
        //        Priority.Mid,
        //        new List<ICheck>(),
        //        new List<ICheck> {
        //            Uses(
        //                Priority.Mid,
        //                constructor.Result
        //            )
        //        }
        //    )
        //);*/
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine that there exists a method which adheres to the requirement of Singleton d1
    /// </summary>
    internal MethodCheck CheckSingletonD1(MethodCheck getInstanceMethod, ICheck checkNoInstanceConstructor)
    {
        return Method(
            Priority.Mid,
            getInstanceMethod,
            checkNoInstanceConstructor
        );
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine that if a specific field is not empty it returns that field
    /// </summary>
    internal ICheck ReturnsPrivateField(FieldCheck checkSingletonC)
    {
        //Right now it only checks if the field is called somewhere in a method and if the return type is the same as the class, not at which conditions
        return All(
            Priority.Mid,
            Uses(
                Priority.Mid,
                checkSingletonC.Result
            ),
            Type(
                Priority.Knockout,
                ICheck.GetCurrentEntity
            )
        );

        /*TODO: fix and use 'correct' implementation below
        //return Method(
        //    Priority.Mid,
        //    new IfThenOperatorCheck(
        //        Priority.Mid,
        //        new List<ICheck>(),
        //        new List<ICheck> {
        //            Uses(
        //                Priority.Mid,
        //                checkSingletonC.Result
        //            )
        //        }
        //    )
        //);*/
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine that there exists a method which adheres to the requirement of Singleton d2
    /// </summary>
    internal MethodCheck CheckSingletonD2(MethodCheck getInstanceMethod, ICheck checkInstanceConstructor)
    {
        return Method(
            Priority.Mid,
            getInstanceMethod,
            checkInstanceConstructor
        );
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that checks if the behaviour of an method in the singleton class adheres to requirements Singleton d0, d1 and d2
    /// </summary>
    internal ICheck CheckMethodAcsAsConstructorBehaviour(
        ConstructorCheck privateConstructorCheck,
        FieldCheck staticPrivateFieldOfTypeClass,
        out MethodCheck hasStaticPublicInternalMethod)
    {
        // check d0
        hasStaticPublicInternalMethod = HasStaticPublicInternalMethod();

        // check d1
        MethodCheck checkSingletonD1 =
            CheckSingletonD1(hasStaticPublicInternalMethod, CallsPrivateConstructor(privateConstructorCheck));

        //TODO: implement
        ICheck checkInstanceConstructor = ReturnsPrivateField(staticPrivateFieldOfTypeClass);
        MethodCheck checkSingletonD2 =
            CheckSingletonD2(hasStaticPublicInternalMethod, checkInstanceConstructor);

        return All(
            Priority.Low,
            hasStaticPublicInternalMethod,
            checkSingletonD1,
            checkSingletonD2);
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine if the client class calls the method which acts as a constructor in Singleton
    /// </summary>
    internal MethodCheck ClientCallsMethodActsAsConstructor(MethodCheck getInstanceMethod)
    {
        return Method(
            Priority.Mid,
            Uses(
                Priority.Mid,
                getInstanceMethod.Result
            )
        );
    }
}
