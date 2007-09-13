using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class LoadOrderDetailRequest : DataContractBase
    {
        public LoadOrderDetailRequest(EntityRef orderRef)
        {
            this.OrderRef = orderRef;
        }

        public LoadOrderDetailRequest(string accessionNumber)
        {
            this.AccessionNumber = accessionNumber;
        }

        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public string AccessionNumber;
    }
}
