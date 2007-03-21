using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientReconcilliation
{
    [DataContract]
    public class LoadPatientProfileDiffRequest : DataContractBase
    {
        public LoadPatientProfileDiffRequest(EntityRef leftProfile, EntityRef rightProfile)
        {
            this.LeftProfileRef = leftProfile;
            this.RightProfileRef = rightProfile;
        }

        [DataMember]
        public EntityRef LeftProfileRef;

        [DataMember]
        public EntityRef RightProfileRef;
    }
}
