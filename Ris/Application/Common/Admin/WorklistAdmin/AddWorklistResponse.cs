using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class AddWorklistResponse : DataContractBase
    {
        public AddWorklistResponse(WorklistSummary addedWorklistSummary)
        {
            AddedWorklistSummary = addedWorklistSummary;
        }

        [DataMember]
        public WorklistSummary AddedWorklistSummary;
    }
}