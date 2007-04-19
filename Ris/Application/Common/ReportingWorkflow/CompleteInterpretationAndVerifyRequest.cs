using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class CompleteInterpretationAndVerifyRequest : DataContractBase
    {
        public CompleteInterpretationAndVerifyRequest(ReportingWorklistItem item)
        {
            this.WorklistItem = item;
        }

        [DataMember]
        public ReportingWorklistItem WorklistItem;
    }
}