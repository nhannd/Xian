using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin
{
    [DataContract]
    public class EnumerationSummary : DataContractBase
    {
        public EnumerationSummary()
        {

        }

        public EnumerationSummary(string assemblyQualifiedClassName, string displayName, bool canAddRemoveValues)
        {
            this.AssemblyQualifiedClassName = assemblyQualifiedClassName;
            this.DisplayName = displayName;
            this.CanAddRemoveValues = canAddRemoveValues;
        }

        [DataMember]
        public string DisplayName;

        [DataMember]
        public string AssemblyQualifiedClassName;

        [DataMember]
        public bool CanAddRemoveValues;
    }
}
