using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class SearchResponse : DataContractBase
    {
        public SearchResponse(List<ModalityWorklistItem> worklistItems)
        {
            this.WorklistItems = worklistItems;
        }

        [DataMember]
        public List<ModalityWorklistItem> WorklistItems;
    }
}
