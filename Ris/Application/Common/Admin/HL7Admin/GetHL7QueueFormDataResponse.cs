using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.HL7Admin
{
    [DataContract]
    public class GetHL7QueueFormDataResponse : DataContractBase
    {
        [DataMember]
        public string[] DirectionChoices;

        [DataMember]
        public string[] StatusCodeChoices;

        [DataMember]
        public string[] PeerChoices;

        [DataMember]
        public string[] MessageTypeChoices;

        [DataMember]
        public string[] MessageEventChoices;

        [DataMember]
        public string[] MessageVersionChoices;

        [DataMember]
        public string[] MessageFormatChoices;
    }
}
