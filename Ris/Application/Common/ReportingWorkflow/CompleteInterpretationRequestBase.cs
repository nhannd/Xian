using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public abstract class CompleteInterpretationRequestBase : SaveReportRequest
    {
        public CompleteInterpretationRequestBase(EntityRef reportingStepRef, string reportContent, EntityRef supervisorRef)
            : base(reportingStepRef, reportContent, supervisorRef)
        {
        }
    }
}
