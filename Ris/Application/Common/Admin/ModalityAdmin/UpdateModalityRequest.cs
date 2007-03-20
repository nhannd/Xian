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
        public UpdateModalityRequest(EntityRef modalityRef, ModalityDetail detail)
        {
            this.ModalityRef = modalityRef;
            this.ModalityDetail = detail;
        }

        [DataMember]
        public EntityRef ModalityRef;

        [DataMember]
        public ModalityDetail ModalityDetail;
    }
}
