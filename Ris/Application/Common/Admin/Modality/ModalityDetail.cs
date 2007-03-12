using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.Modality
{
    [DataContract]
    public class ModalityDetail : DataContractBase
    {
        [DataMember]
        public string Id;

        [DataMember]
        public string Name;
    }
}
