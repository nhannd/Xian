using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin
{
    [DataContract]
    public class UpdateModalityResponse : DataContractBase
    {
        public UpdateModalityResponse(ModalitySummary summary)
        {
            this.Modality = summary;
        }

        [DataMember]
        public ModalitySummary Modality;
    }
}
