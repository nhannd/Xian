using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class SearchResponse : DataContractBase
    {
        public SearchResponse(List<RegistrationWorklistItem> worklistItems)
        {
            this.WorklistItems = worklistItems;
        }

        [DataMember]
        public List<RegistrationWorklistItem> WorklistItems;
    }
}
