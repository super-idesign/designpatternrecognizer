﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PatternPal.Core.StepByStep.Resources.Instructions {
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
    internal class DecoratorInstructions {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal DecoratorInstructions() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PatternPal.Core.StepByStep.Resources.Instructions.DecoratorInstructions", typeof(DecoratorInstructions).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Make an interface with a method. We refer to this interface as `Component`..
        /// </summary>
        internal static string Step1 {
            get {
                return ResourceManager.GetString("Step1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Make a class that implements Component. Also implement the method. We refer to this class as `ConcreteComponent`..
        /// </summary>
        internal static string Step2 {
            get {
                return ResourceManager.GetString("Step2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Make an abstract class that implements Component. Give it a field with type Component. We refer to this class as `Decorator`..
        /// </summary>
        internal static string Step3 {
            get {
                return ResourceManager.GetString("Step3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Give Decorator a constructor with a parameter that gets assigned to the field. .
        /// </summary>
        internal static string Step4 {
            get {
                return ResourceManager.GetString("Step4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to In Decorator, call the method of the field in the implementation of the method..
        /// </summary>
        internal static string Step5 {
            get {
                return ResourceManager.GetString("Step5", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Make a class that inherits from Decorator. Override its method by calling the method of Decorator; base.Method(). We refer to this class as `ConcreteDecorator`..
        /// </summary>
        internal static string Step6 {
            get {
                return ResourceManager.GetString("Step6", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Add a method  to ConcreteDecorator providing additional behaviour and call it in the overrided method either before or after the call to the parent&apos;s method..
        /// </summary>
        internal static string Step7 {
            get {
                return ResourceManager.GetString("Step7", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Make a method that instantiates an instance of ConcreteDecorator by passing to its constructor a new instance of ConcreteComponent. Now call the method of the instanciated variable..
        /// </summary>
        internal static string Step8 {
            get {
                return ResourceManager.GetString("Step8", resourceCulture);
            }
        }
    }
}
