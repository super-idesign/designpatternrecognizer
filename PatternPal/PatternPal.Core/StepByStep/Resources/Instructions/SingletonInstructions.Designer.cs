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
    internal class SingletonInstructions {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SingletonInstructions() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PatternPal.Core.StepByStep.Resources.Instructions.SingletonInstructions", typeof(SingletonInstructions).Assembly);
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
        ///   Looks up a localized string similar to The constructor is private to prevent direct instantiation of the class from outside the class. This ensures that only one object is created..
        /// </summary>
        internal static string Explanation1 {
            get {
                return ResourceManager.GetString("Explanation1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The field is static as a way of accessing the single instance when it is created. .
        /// </summary>
        internal static string Explanation2 {
            get {
                return ResourceManager.GetString("Explanation2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This method will act as a constructor. It calls the constructor and save the object in the field of the previous step. The following calls will return the same object in the field. .
        /// </summary>
        internal static string Explanation3 {
            get {
                return ResourceManager.GetString("Explanation3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Any client that accesses the singleton will work with the same instance of the singleton throughout its lifetime..
        /// </summary>
        internal static string Explanation4 {
            get {
                return ResourceManager.GetString("Explanation4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create a constructor that is only private..
        /// </summary>
        internal static string Step1 {
            get {
                return ResourceManager.GetString("Step1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create a static, private field with the same type as the class..
        /// </summary>
        internal static string Step2 {
            get {
                return ResourceManager.GetString("Step2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create a static, public/internal method that acts as the constructor. When called and the field from the previous step is null call the constructor otherwise return the field..
        /// </summary>
        internal static string Step3 {
            get {
                return ResourceManager.GetString("Step3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create a client class that calls the method from the previous step to retrieve the singleton instance. .
        /// </summary>
        internal static string Step4 {
            get {
                return ResourceManager.GetString("Step4", resourceCulture);
            }
        }
    }
}
