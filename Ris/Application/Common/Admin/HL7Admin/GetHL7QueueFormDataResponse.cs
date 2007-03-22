using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.HL7Admin
{
    [DataContract]
    public class GetHL7QueueFormDataResponse : DataContractBase
    {
        [DataMember]
        public List<EnumValueInfo> DirectionChoices;

        [DataMember]
        public List<EnumValueInfo> StatusCodeChoices;

        [DataMember]
        public List<EnumValueInfo> PeerChoices;

        [DataMember]
        public List<string> MessageTypeChoices;

        [DataMember]
        public List<string> MessageEventChoices;

        [DataMember]
        public List<EnumValueInfo> MessageVersionChoices;

        [DataMember]
        public List<EnumValueInfo> MessageFormatChoices;
    }
}
