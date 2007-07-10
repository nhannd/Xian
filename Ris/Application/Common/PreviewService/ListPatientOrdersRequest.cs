using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.PreviewService
{
    [DataContract]
    public class ListPatientOrdersRequest : DataContractBase
    {
        public ListPatientOrdersRequest(string queryDetailLevel)
        {
            this.QueryDetailLevel = queryDetailLevel;
        }

        public ListPatientOrdersRequest()
        {
            this.QueryDetailLevel = "Order";
        }

        [DataMember]
        public string QueryDetailLevel;
    }
}
