using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class ResultRecipientSummary : DataContractBase
    {
        public ResultRecipientSummary()
        {
        }

        public ResultRecipientSummary(ExternalPractitionerSummary practitioner, ExternalPractitionerContactPointSummary contactPoint, EnumValueInfo preferredCommunicationMode)
        {
            this.Practitioner = practitioner;
            this.ContactPoint = contactPoint;
            this.PreferredCommunicationMode = preferredCommunicationMode;
        }

        [DataMember]
        public ExternalPractitionerSummary Practitioner;

        [DataMember]
        public ExternalPractitionerContactPointSummary ContactPoint;

        [DataMember]
        public EnumValueInfo PreferredCommunicationMode;
    }
}
