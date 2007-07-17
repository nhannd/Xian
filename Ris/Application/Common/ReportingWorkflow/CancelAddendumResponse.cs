using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class CancelAddendumResponse : DataContractBase
    {
        public CancelAddendumResponse(EntityRef addendumStepRef)
        {
            this.AddendumStepRef = addendumStepRef;
        }

        [DataMember]
        public EntityRef AddendumStepRef;
    }
}
