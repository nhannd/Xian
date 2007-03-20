using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin
{
    [DataContract]
    public class AddModalityRequest : DataContractBase
    {
        public AddModalityRequest(ModalityDetail detail)
        {
            this.ModalityDetail = detail;
        }

        [DataMember]
        public ModalityDetail ModalityDetail;
    }
}
