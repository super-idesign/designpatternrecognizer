﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IDesign.StepByStep.Resources.Instructions {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class StrategyInstructions {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal StrategyInstructions() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("IDesign.StepByStep.Resources.Instructions.StrategyInstructions", typeof(StrategyInstructions).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create an abstract class. For example, ‘Duck’. The behaviour will be added later..
        /// </summary>
        public static string _1 {
            get {
                return ResourceManager.GetString("1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create subclasses that implement the previously created abstract class. For example, ‘RunnerDuck’ and ‘RubberDuck’..
        /// </summary>
        public static string _2 {
            get {
                return ResourceManager.GetString("2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to To the abstract class, add a method called “Perform[behaviour-name]”. For example, ‘PerformQuack’..
        /// </summary>
        public static string _3 {
            get {
                return ResourceManager.GetString("3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create an interface (for example, ‘QuackBehaviour’) that defines the behaviour you want to implement later. For example, ‘public string Quack();’..
        /// </summary>
        public static string _4 {
            get {
                return ResourceManager.GetString("4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Add to the abstract class (previously referred to as ‘Duck’) a property. The type of this property must be the interface that defines the behaviour you want to implement. For example, ‘QuackBehaviour’..
        /// </summary>
        public static string _5 {
            get {
                return ResourceManager.GetString("5", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to In the abstract class (named “Duck” in a previous example), implement the method that performs the behaviour. It must call the behaviour method through the behaviour property (for example, QuackBehaviour.Quack();).
        /// </summary>
        public static string _6 {
            get {
                return ResourceManager.GetString("6", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create a class which implements the behaviour interface, here you will add the specific implementation of the behaviour. (for example LoudQuackBehaviour or NoQuackBehaviour).
        /// </summary>
        public static string _7 {
            get {
                return ResourceManager.GetString("7", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to In the constructor of the subclass that implemented the abstract class (for example RunnerDuck or RubberDuck) instantiate the behaviour as the specific behaviour class you want to use..
        /// </summary>
        public static string _8 {
            get {
                return ResourceManager.GetString("8", resourceCulture);
            }
        }
    }
}
