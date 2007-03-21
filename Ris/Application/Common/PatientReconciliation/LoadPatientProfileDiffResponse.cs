using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientReconcilliation
{
    [DataContract]
    public class LoadPatientProfileDiffResponse : DataContractBase
    {
        [DataMember]
        public PatientProfileDiff ProfileDiff;
    }
}
