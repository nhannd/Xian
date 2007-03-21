using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.PatientReconcilliation
{
    [DataContract]
    public class PatientProfileMatch : DataContractBase
    {
        [DataMember]
        public PatientProfileSummary PatientProfile;

        [DataMember]
        public int Score;
    }
}
