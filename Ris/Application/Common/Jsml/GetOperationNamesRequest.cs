using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Jsml
{
    [DataContract]
    public class GetOperationNamesRequest : DataContractBase
    {
        public GetOperationNamesRequest(string serviceContractName)
        {
            this.ServiceContractName = serviceContractName;
        }

        /// <summary>
        /// The name of the service contract to query.  This must be an assembly-qualified name.
        /// </summary>
        [DataMember]
        public string ServiceContractName;
    }
}
