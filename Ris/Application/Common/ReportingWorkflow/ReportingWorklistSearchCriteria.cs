using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ReportingWorklistSearchCriteria : DataContractBase
    {
        //TODO: the exact reporting ps search criteria not defined
        [DataMember]
        public string ActivityStatusCode;
    }
}
