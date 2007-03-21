using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.PatientReconcilliation
{
    [DataContract]
    public class ListIndirectlyReconciledPatientsRequest : DataContractBase
    {
        /// <summary>
        /// The set of patients that will be reconciled
        /// </summary>
        [DataMember]
        public List<EntityRef> PatientRefsToReconcile;
    }
}
