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
    internal class AdapterInstructions {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AdapterInstructions() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PatternPal.Core.StepByStep.Resources.Instructions.AdapterInstructions", typeof(AdapterInstructions).Assembly);
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
        ///   Looks up a localized string similar to This interface defines some sort of behavior that we will want to adapt a class to..
        /// </summary>
        internal static string Explanation1 {
            get {
                return ResourceManager.GetString("Explanation1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The client cannot use this (possibly third party) class directly because it has an incompatable interface..
        /// </summary>
        internal static string Explanation2 {
            get {
                return ResourceManager.GetString("Explanation2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to To be able to use the Service, we make this class to adapt it to our interface. When called, it &quot;translates&quot; the call to the Service. Its field is private, since the logic should be exposed to the other code as the general interface only. The real implementation of the Service should only be known by the adapter. .
        /// </summary>
        internal static string Explanation3 {
            get {
                return ResourceManager.GetString("Explanation3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The client can now use the logic of the Service as if it does have a compatable interface..
        /// </summary>
        internal static string Explanation4 {
            get {
                return ResourceManager.GetString("Explanation4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Make an interface with a method. We refer to this interface as `ClientInterface`..
        /// </summary>
        internal static string Step1 {
            get {
                return ResourceManager.GetString("Step1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Make a class that has a method. It may not inherit from ClientInterface. We refer to this class as `Service`..
        /// </summary>
        internal static string Step2 {
            get {
                return ResourceManager.GetString("Step2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Make a class that implements ClientInterface. Give it a private field of type Service and instantiate it. Implement the method of ClientInterface by using the method of the field. We refer to this class as `Adapter`..
        /// </summary>
        internal static string Step3 {
            get {
                return ResourceManager.GetString("Step3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Make a class that creates an instance of Adapter. Give it a method that calls the method of the instantiated variable..
        /// </summary>
        internal static string Step4 {
            get {
                return ResourceManager.GetString("Step4", resourceCulture);
            }
        }
    }
}