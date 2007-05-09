using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class GetWorklistCountResponse : DataContractBase
    {
        public GetWorklistCountResponse(int itemCount)
        {
            this.ItemCount = itemCount;
        }

        [DataMember]
        public int ItemCount;
    }
}
