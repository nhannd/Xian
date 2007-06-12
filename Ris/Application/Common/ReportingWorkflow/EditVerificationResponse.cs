using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class EditVerificationResponse : DataContractBase
    {
        public EditVerificationResponse(EntityRef stepRef, string reportContent)
        {
            this.ReportingStepRef = stepRef;
            this.ReportContent = reportContent;
        }

        [DataMember]
        public EntityRef ReportingStepRef;

        [DataMember]
        public string ReportContent;
    }
}