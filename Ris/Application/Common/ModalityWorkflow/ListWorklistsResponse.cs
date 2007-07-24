using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class ListWorklistsResponse : DataContractBase
    {
        public ListWorklistsResponse(List<WorklistSummary> worklists)
        {
            Worklists = worklists;
        }

        [DataMember]
        public List<WorklistSummary> Worklists;
    }
}