﻿#region

using PatternPal.Core.StepByStep;
using PatternPal.Core.StepByStep.Resources.Instructions;
using PatternPal.SyntaxTree.Models;
using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implements the observer pattern
/// </summary>
/// <remarks>
///
///     Requirements for Observer interface:
///         a) is an interface
///         b) has a public or internal `update()` method
///     Requirements for Concrete Observer:
///         a) is a class that implements the `Observer` interface
///     Requirements for Subject interface:
///         a) is an interface
///         b) has a public or internal method that has a parameter with as type `Observer`
///     Requirements for Concrete Subject:
///         a) is a class that implements the `Subject` interface 
///         b) has a private field with as type a `Observer` list, `observers`  
///         c) has a public or internal methods that has a parameter with as type `Observer` that uses the list `observers`
///         d) has a public method that uses the `observers` list and uses the `update()` method in `Observer`
///         e) has either,
///             1. both,
///                 1. a private or protected field or property `mainState`.
///                 2. has a public or internal method that uses `mainState`.
///             2. a public or internal method with a parameter.
///     Requirements for Client:
///         a) creates the Subject
///         b) creates a Concrete Observer
///         c) uses a method as described in `Concrete Subject` c)
///         d) uses the method as described in `Concrete Subject` d)
/// </remarks>
internal class ObserverRecognizer : IRecognizer, IStepByStepRecognizer
{
    /// <inheritdoc cref="IRecognizer" />
    public string Name => "Observer";

    /// <inheritdoc cref="IRecognizer" />
    public Recognizer RecognizerType => Recognizer.Observer;

    private readonly InterfaceCheck _observerInterface;
    private readonly ClassCheck _concreteObserver;
    private readonly InterfaceCheck _subjectInterface;

    private readonly FieldCheck _concreteFieldCheck;
    private readonly MethodCheck _concreteAttachCheck;
    private readonly ClassCheck _concreteSubject;

    private readonly ClassCheck _clientClass;

    public ObserverRecognizer()
    {
        _observerInterface = ObserverInterfaceCheck(out MethodCheck observerInterfaceMethod);
        _concreteObserver = ConcreteObserverCheck();
        _subjectInterface = SubjectInterfaceCheck();

        _concreteFieldCheck = ConcreteSubjectFieldCheck();
        _concreteAttachCheck = ConcreteSubjectAttachCheck();
        MethodCheck concreteUpdateCheck = ConcreteSubjectUpdateCheck(observerInterfaceMethod);
        _concreteSubject = ConcreteSubjectInterfaceCheck(
            _concreteFieldCheck,
            _concreteAttachCheck,
            ConcreteSubjectStateCheck(),
            concreteUpdateCheck
        );

        _clientClass = ClientClassCheck(
            ClientUseMethodCheck(
                _concreteAttachCheck,
                "5c. Use the Attach() method in the Concrete Subject."
                ),
            ClientUseMethodCheck(
                concreteUpdateCheck,
                "5d. Use the Notify() method in the Concrete Subject.")
        );
    }

    /// <inheritdoc />
    public IEnumerable<ICheck> Create()
    {
        // return Observer Interface check
        yield return _observerInterface;
        
        // return Concrete Subscriber check;
        yield return _concreteObserver;

        // return Subject Interface check;
        yield return _subjectInterface;

        // return Concrete Subject class
        yield return _concreteSubject;

        // return Client class
        yield return _clientClass;
    }

    /// <inheritdoc />
    public List<IInstruction> GenerateStepsList()
    {
        return new List<IInstruction>
        {
            new SimpleInstruction(
                ObserverInstructions.Step1,
                ObserverInstructions.Explanation1,
                new List<ICheck>
                {
                    _observerInterface
                }
            ),
            new SimpleInstruction(
                ObserverInstructions.Step2,
                ObserverInstructions.Explanation2,
                new List<ICheck>
                {
                    _observerInterface,
                    _concreteObserver
                }
            ),
            new SimpleInstruction(
                ObserverInstructions.Step3,
                ObserverInstructions.Explanation3,
                new List<ICheck>
                {
                    _observerInterface,
                    _concreteObserver,
                    _subjectInterface
                }
            ),
            new SimpleInstruction(
                ObserverInstructions.Step4,
                ObserverInstructions.Explanation4,
                new List<ICheck>
                {
                    _observerInterface,
                    _concreteObserver,
                    _subjectInterface,
                    ConcreteSubjectInterfaceCheck()
                }
            ),
            new SimpleInstruction(
                ObserverInstructions.Step5,
                ObserverInstructions.Explanation5,
                new List<ICheck>
                {
                    _observerInterface,
                    _concreteObserver,
                    _subjectInterface,
                    ConcreteSubjectInterfaceCheck(
                        _concreteFieldCheck
                    )
                }
            ),
            new SimpleInstruction(
                ObserverInstructions.Step6,
                ObserverInstructions.Explanation6,
                new List<ICheck>
                {
                    _observerInterface,
                    _concreteObserver,
                    _subjectInterface,
                    ConcreteSubjectInterfaceCheck(
                        _concreteFieldCheck,
                        _concreteAttachCheck
                    )

                }
            ),
            new SimpleInstruction(
                ObserverInstructions.Step8,
                ObserverInstructions.Explanation8,
                new List<ICheck>
                {
                    _observerInterface,
                    _concreteObserver,
                    _subjectInterface,
                    ConcreteSubjectInterfaceCheck(
                        _concreteFieldCheck,
                        _concreteAttachCheck,
                        ConcreteSubjectStateFieldOption()
                    )
                }
            ),
            new SimpleInstruction(
                ObserverInstructions.Step7,
                ObserverInstructions.Explanation7,
                new List<ICheck>
                {
                    _observerInterface,
                    _concreteObserver,
                    _subjectInterface,
                    _concreteSubject,

                }
            ),
            new SimpleInstruction(
                ObserverInstructions.Step9,
                ObserverInstructions.Explanation9,
                new List<ICheck>
                {
                    _observerInterface,
                    _concreteObserver,
                    _subjectInterface,
                    _concreteSubject,
                    ClientClassCheck()
                }
            ),
            new SimpleInstruction(
                ObserverInstructions.Step10,
                ObserverInstructions.Explanation10,
                new List<ICheck>
                {
                    _observerInterface,
                    _concreteObserver,
                    _subjectInterface,
                    _concreteSubject,
                    ClientClassCheck(
                        ClientUseMethodCheck(_concreteAttachCheck)
                    )
                }
            ),
            new SimpleInstruction(
                ObserverInstructions.Step11,
                ObserverInstructions.Explanation11,
                new List<ICheck>
                {
                    _observerInterface,
                    _concreteObserver,
                    _subjectInterface,
                    _concreteSubject,
                    _clientClass
                }
            ),
        };
    }

    /// <summary>
    /// A <see cref="InterfaceCheck"/> that holds a <see cref="MethodCheck"/> that verifies the existence of a
    /// method that is not <see cref="Modifier.Private"/> or <see cref="Modifier.Protected"/>. 
    /// </summary>
    /// <returns>A <see cref="InterfaceCheck"/> that checks the requirements of the Observer Interface.</returns>
    private InterfaceCheck ObserverInterfaceCheck(out MethodCheck observerInterfaceMethod)
    {
        observerInterfaceMethod = Method(
            Priority.Knockout,
            "1b. Has a public or protected method, `update()`.",
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

        return Interface(
            Priority.Knockout,
            "1. Observer Interface",
            observerInterfaceMethod
        );
    }

    /// <summary>
    /// A <see cref="ClassCheck"/> that should have a <see cref="RelationType.Implements"/> relation with the
    /// <see cref="_observerInterface"/>.
    /// </summary>
    /// <returns>A <see cref="ClassCheck"/> that checks the requirements of the Concrete Observer.</returns>
    private ClassCheck ConcreteObserverCheck()
    {
        return Class(
            Priority.Knockout,
            "2. Concrete Observer Class",
            Implements(
                Priority.Knockout,
                "2a. Implements the Observer interface.",
                _observerInterface
            )
        );
    }

    /// <summary>
    /// A <see cref="InterfaceCheck"/> with a <see cref="MethodCheck"/> which is <see cref="Modifier.Public"/>
    /// or <see cref="Modifier.Internal"/> and has a <see cref="ParameterCheck"/> that checks for a parameter with
    /// the <see cref="_observerInterface"/>.
    /// </summary>
    /// <returns>A <see cref="InterfaceCheck"/> that checks the requirements of the Subject Interface.</returns>
    private InterfaceCheck SubjectInterfaceCheck()
    {
        return Interface(
            Priority.Knockout,
            "3. Subject Interface",
            Method(
                Priority.Knockout,
                "3a. Has a public or internal method with a parameter with the Observer Interface type.",
                PublicOrInternalCheck(Priority.Knockout),
                Parameters(
                    Priority.Knockout,
                    Type(
                        Priority.Knockout,
                        _observerInterface
                    )
                )
            )
        );
    }

    /// <summary>
    /// A <see cref="ClassCheck"/> that checks whether a class implements the <see cref="_subjectInterface"/> and
    /// the other <paramref name="checks"/>.
    /// </summary>
    /// <param name="checks">An array of <see cref="ICheck"/>s that the class should adhere to.</param>
    /// <returns>A <see cref="ClassCheck"/> that could check the requirements of the Concrete Subject if
    /// they are presented in the <paramref name="checks"/>.</returns>
    private ClassCheck ConcreteSubjectInterfaceCheck(params ICheck[] checks)
    {
        return Class(
            Priority.Knockout,
            "4. Concrete Subject Class",
            checks.Append(
                Implements(
                    Priority.Knockout,
                    "4a. Implements the Subject Interface.",
                    _subjectInterface
                )
            ).ToArray()
        );
    }

    /// <summary>
    /// A <see cref="FieldCheck"/> that checks whether it is <see cref="Modifier.Private"/>.
    /// </summary>
    /// <returns>A <see cref="FieldCheck"/> with a <see cref="ModifierCheck"/> for the Concrete Subject check.</returns>
    private FieldCheck ConcreteSubjectFieldCheck()
    {
        return Field(
            Priority.Knockout,
            "4b. Private field with a Observer List as type.",
            Modifiers(
                Priority.Knockout,
                Modifier.Private
            )
        );
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    private MethodCheck ConcreteSubjectAttachCheck()
    {
        return Method(
            Priority.Knockout,
            "4c. Public or internal method that has a parameter with as type Observer and uses the list observers.",
            PublicOrInternalCheck(Priority.Knockout),
            Parameters(
                Priority.Knockout,
                Type(
                    Priority.Knockout,
                    _observerInterface
                )
            ),
            Uses(
                Priority.Knockout,
                _concreteFieldCheck
            )
        );
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    private ICheck ConcreteSubjectStateCheck()
    {
        MethodCheck methodOption = Method(
            Priority.High,
            PublicOrInternalCheck(Priority.High),
            Parameters(Priority.High)
        );

        return Any(
            Priority.High,
            "4e. Has either a protected or private field or property with a method that uses one of these, or" +
            "has a method with a parameter",
            ConcreteSubjectStateFieldOption(),
            methodOption
        );
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    private ICheck ConcreteSubjectStateFieldOption()
    {
        ICheck modifierCheck = Any(
            Priority.High,
            Modifiers(Priority.High, Modifier.Private),
            Modifiers(Priority.High, Modifier.Protected)
        );

        ICheck fieldOrProperty = Any(
            Priority.High,
            Field(
                Priority.High,
                modifierCheck
            ),
            Property(
                Priority.High,
                modifierCheck
            )
        );

        return All(
            Priority.High,
            fieldOrProperty,
            Method(
                Priority.High,
                PublicOrInternalCheck(Priority.High),
                Uses(
                    Priority.High,
                    fieldOrProperty
                )
            )
        );
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="observerInterfaceMethod"></param>
    /// <returns></returns>
    private MethodCheck ConcreteSubjectUpdateCheck(MethodCheck observerInterfaceMethod)
    {
        return Method(
            Priority.Knockout,
            "4d. Public method that uses the list observers and uses the method in the Observer.",
            Modifiers(
                Priority.Knockout,
                Modifier.Public
            ),
            Uses(
                Priority.Knockout,
                observerInterfaceMethod
            )
        );
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="checks"></param>
    /// <returns></returns>
    private ClassCheck ClientClassCheck(params ICheck[] checks)
    {
        ICheck[] classChecks = checks
            .Append(Creates(Priority.Mid, "5a. Create a Concrete Subject instance.", _concreteSubject))
            .Append(Creates(Priority.Mid, "5b. Create a Concrete Observer instance.", _concreteObserver))
            .ToArray();

        return Class(
            Priority.Mid,
            "5. Client Class",
            classChecks
        );
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    private MethodCheck ClientUseMethodCheck(MethodCheck toUseMethod, string req = null)
    {
        return Method(
            Priority.Low,
            req,
            Uses(
                Priority.Low,
                toUseMethod
            )
        );
    }


    /// <summary>
    /// A method to check whether the instance it checks on is either
    /// <see cref="Modifier.Public"/> or <see cref="Modifier.Internal"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of the check.</param>
    /// <returns>A <see cref="ICheck"/> that holds a check to verify whether an instance is
    /// <see cref="Modifier.Public"/> or <see cref="Modifier.Internal"/>.</returns>
    private ICheck PublicOrInternalCheck(Priority priority)
    {
        return Any(
            priority,
            Modifiers(
                priority,
                Modifier.Public
            ),
            Modifiers(
                priority,
                Modifier.Internal
            )
        );
    }
}
