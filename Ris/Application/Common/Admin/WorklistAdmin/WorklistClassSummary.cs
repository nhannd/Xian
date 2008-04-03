using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class WorklistClassSummary : DataContractBase
    {
        public WorklistClassSummary(string className, string displayName, string categoryName, string description,
            string procedureTypeGroupClassName, bool supportsTimeWindow)
        {
            ClassName = className;
            DisplayName = displayName;
            CategoryName = categoryName;
            ProcedureTypeGroupClassName = procedureTypeGroupClassName;
            SupportsTimeWindow = supportsTimeWindow;
            Description = description;
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
        public bool SupportsTimeWindow;
    }
}
