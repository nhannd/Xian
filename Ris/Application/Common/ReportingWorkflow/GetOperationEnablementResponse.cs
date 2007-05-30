using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class GetOperationEnablementResponse : DataContractBase
    {
        public GetOperationEnablementResponse(IDictionary<string, bool> dictionary)
        {
            this.OperationEnablementDictionary = dictionary as Dictionary<string, bool>;
        }

        [DataMember]
        public Dictionary<string, bool> OperationEnablementDictionary;
    }
}
