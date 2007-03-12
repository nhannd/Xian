using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin
{
    [DataContract]
    public class ListAllModalitiesResponse : DataContractBase
    {
        public ListAllModalitiesResponse(List<ModalitySummary> modalities)
        {
            this.Modalities = modalities;
        }

        [DataMember]
        public List<ModalitySummary> Modalities;
    }

}
