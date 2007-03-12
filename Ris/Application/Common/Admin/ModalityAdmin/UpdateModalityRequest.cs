using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin
{
    [DataContract]
    public class UpdateModalityRequest : DataContractBase
    {
        [DataMember]
        public EntityRef ModalityRef;

        [DataMember]
        public ModalityDetail ModalityDetail;
    }
}
