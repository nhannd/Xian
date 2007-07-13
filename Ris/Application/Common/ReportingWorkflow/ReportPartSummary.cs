using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ReportPartSummary : DataContractBase
    {
        [DataMember]
        public EntityRef ReportPartRef;

        [DataMember]
        public string Index;

        [DataMember]
        public string Content;
    }
}
