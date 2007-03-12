using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin
{
    [DataContract]
    public class AddModalityResponse : DataContractBase
    {
        public AddModalityResponse(ModalitySummary modality)
        {
            this.Modality = modality;
        }

        [DataMember]
        public ModalitySummary Modality;
    }
}
