﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.832
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.Ris.Application.Services {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SR {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SR() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ClearCanvas.Ris.Application.Services.SR", typeof(SR).Assembly);
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
        ///   Looks up a localized string similar to A practitioner with the name {0}, {1} already exists.
        /// </summary>
        internal static string ExceptionExternalPractitionerAlreadyExist {
            get {
                return ResourceManager.GetString("ExceptionExternalPractitionerAlreadyExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A facility with code {0} already exists.
        /// </summary>
        internal static string ExceptionFacilityAlreadyExist {
            get {
                return ResourceManager.GetString("ExceptionFacilityAlreadyExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A family name must be provided.
        /// </summary>
        internal static string ExceptionFamilyNameMissing {
            get {
                return ResourceManager.GetString("ExceptionFamilyNameMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Attempt to import entities with the same Id but different names.
        /// </summary>
        internal static string ExceptionImportEntityNameIdMismatch {
            get {
                return ResourceManager.GetString("ExceptionImportEntityNameIdMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A location with the same facility, building, floor, point of care, room and bed already exists.
        /// </summary>
        internal static string ExceptionLocationAlreadyExist {
            get {
                return ResourceManager.GetString("ExceptionLocationAlreadyExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A modality with ID {0} already exists.
        /// </summary>
        internal static string ExceptionModalityAlreadyExist {
            get {
                return ResourceManager.GetString("ExceptionModalityAlreadyExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A patient profile with MRN {0} {1} already exists.
        /// </summary>
        internal static string ExceptionMrnAlreadyExists {
            get {
                return ResourceManager.GetString("ExceptionMrnAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Current user must be associated with a Staff in order to perform this operation.
        /// </summary>
        internal static string ExceptionNoStaffForUser {
            get {
                return ResourceManager.GetString("ExceptionNoStaffForUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A note category with name {0} already exists.
        /// </summary>
        internal static string ExceptionNoteCategoryAlreadyExist {
            get {
                return ResourceManager.GetString("ExceptionNoteCategoryAlreadyExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to find current user.
        /// </summary>
        internal static string ExceptionNoUser {
            get {
                return ResourceManager.GetString("ExceptionNoUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Patient with MRN {0} {1} not found.
        /// </summary>
        internal static string ExceptionPatientNotFound {
            get {
                return ResourceManager.GetString("ExceptionPatientNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A practitioner with the name {0}, {1} already exists.
        /// </summary>
        internal static string ExceptionPractitionerAlreadyExist {
            get {
                return ResourceManager.GetString("ExceptionPractitionerAlreadyExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to At least 2 patients must be specified for reconciliation.
        /// </summary>
        internal static string ExceptionReconciliationRequiresAtLeast2Patients {
            get {
                return ResourceManager.GetString("ExceptionReconciliationRequiresAtLeast2Patients", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A requested procedure type group with the name {0} already exists.
        /// </summary>
        internal static string ExceptionRequestedProcedureTypeGroupNameAlreadyExists {
            get {
                return ResourceManager.GetString("ExceptionRequestedProcedureTypeGroupNameAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A requested procedure type group name is required.
        /// </summary>
        internal static string ExceptionRequestedProcedureTypeGroupNameRequired {
            get {
                return ResourceManager.GetString("ExceptionRequestedProcedureTypeGroupNameRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A staff with the name {0}, {1} already exists.
        /// </summary>
        internal static string ExceptionStaffAlreadyExist {
            get {
                return ResourceManager.GetString("ExceptionStaffAlreadyExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A user with user ID &apos;{0}&apos; already exists.
        /// </summary>
        internal static string ExceptionUserIDAlreadyExists {
            get {
                return ResourceManager.GetString("ExceptionUserIDAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The report content is required to verify this step.
        /// </summary>
        internal static string ExceptionVerifyWithNoReport {
            get {
                return ResourceManager.GetString("ExceptionVerifyWithNoReport", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A worklist with the name {0} already exists.
        /// </summary>
        internal static string ExceptionWorklistNameAlreadyExists {
            get {
                return ResourceManager.GetString("ExceptionWorklistNameAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A worklist name is required.
        /// </summary>
        internal static string ExceptionWorklistNameRequired {
            get {
                return ResourceManager.GetString("ExceptionWorklistNameRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Checked In.
        /// </summary>
        internal static string TextCheckedIn {
            get {
                return ResourceManager.GetString("TextCheckedIn", resourceCulture);
            }
        }
    }
}
