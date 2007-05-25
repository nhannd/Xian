using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientBiography
{
    [DataContract]
    public class ListOrdersForPatientResponse : DataContractBase
    {
        public ListOrdersForPatientResponse(List<OrderSummary> orders)
        {
            this.Orders = orders;
        }

        [DataMember]
        public List<OrderSummary> Orders;
    }
}
