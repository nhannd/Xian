using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientReconciliation
{
    [DataContract]
    public class LoadPatientProfileDiffResponse : DataContractBase
    {
        public LoadPatientProfileDiffResponse(PatientProfileDiff diff)
        {
            this.ProfileDiff = diff;
        }

        [DataMember]
        public PatientProfileDiff ProfileDiff;
    }
}
