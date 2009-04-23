using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class WorklistClassSummary : DataContractBase, IEquatable<WorklistClassSummary>
    {
        /// <summary>
        /// No-args constructor required by Oto scripts.
        /// </summary>
        public WorklistClassSummary()
        {
        }

        public WorklistClassSummary(string className, string displayName, string categoryName, string description, 
            string procedureTypeGroupClassName, string procedureTypeGroupClassDisplayName, 
            bool supportsTimeWindow)
        {
            ClassName = className;
            DisplayName = displayName;
            CategoryName = categoryName;
            ProcedureTypeGroupClassName = procedureTypeGroupClassName;
            SupportsTimeWindow = supportsTimeWindow;
            Description = description;
            ProcedureTypeGroupClassDisplayName = procedureTypeGroupClassDisplayName;

            SupportsReportingStaffRoleFilters = false;
        }

        [DataMember]
        public string ClassName;

        [DataMember]
        public string DisplayName;

        [DataMember]
        public string CategoryName;

        [DataMember]
        public string Description;

        [DataMember]
        public string ProcedureTypeGroupClassName;

        [DataMember]
        public string ProcedureTypeGroupClassDisplayName;

        [DataMember]
        public bool SupportsTimeWindow;

        [DataMember]
        public bool SupportsReportingStaffRoleFilters;

        #region IEquatable overrides

        public bool Equals(WorklistClassSummary worklistClassSummary)
        {
            if (worklistClassSummary == null) return false;
            return Equals(ClassName, worklistClassSummary.ClassName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as WorklistClassSummary);
        }

        public override int GetHashCode()
        {
            return ClassName != null ? ClassName.GetHashCode() : 0;
        }

        #endregion
    }
}
