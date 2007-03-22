using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class LoadPatientSearchComponentFormDataResponse : DataContractBase
    {
        public LoadPatientSearchComponentFormDataResponse(List<EnumValueInfo> sexChoices)
        {
            this.SexChoices = sexChoices;
        }

        [DataMember]
        public List<EnumValueInfo> SexChoices;
    }
}
