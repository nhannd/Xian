using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Jsml
{
    [DataContract]
    public class GetOperationNamesResponse : DataContractBase
    {
        /// <summary>
        /// The names of the operations provided by the service.
        /// </summary>
        [DataMember]
        public string[] OperationNames;
    }
}
