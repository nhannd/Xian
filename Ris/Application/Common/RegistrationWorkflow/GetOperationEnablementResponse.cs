using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class GetOperationEnablementResponse : DataContractBase
    {
        public GetOperationEnablementResponse(Dictionary<string, bool> dictionary)
        {
            this.OperationEnablementDictionary = dictionary;
        }

        [DataMember]
        public Dictionary<string, bool> OperationEnablementDictionary;
    }
}
