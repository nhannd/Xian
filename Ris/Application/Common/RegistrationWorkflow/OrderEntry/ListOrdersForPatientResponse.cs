using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
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
