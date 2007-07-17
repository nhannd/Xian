using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class CreateAddendumRequest : DataContractBase
    {
        public CreateAddendumRequest(ReportingWorklistItem item)
        {
            this.WorklistItem = item;
        }

        [DataMember]
        public ReportingWorklistItem WorklistItem;
    }
}
