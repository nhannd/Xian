using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.Modality
{
    [DataContract]
    public class ModalitySummary : DataContractBase
    {
        [DataMember]
        public EntityRef ModalityRef;

        [DataMember]
        public string Id;

        [DataMember]
        public string Name;

        [DataMember]
        public bool Active;
    }
}
