using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.PatientReconciliation
{
    [DataContract]
    public class ListProfilesForPatientsRequest : DataContractBase
    {
        /// <summary>
        /// The set of patients that will be reconciled
        /// </summary>
        [DataMember]
        public List<EntityRef> PatientRefs;
    }
}
