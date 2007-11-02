using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class ModifyOrderResponse : DataContractBase
    {
        public ModifyOrderResponse(EntityRef orderRef)
        {
            this.OrderRef = orderRef;
        }

        [DataMember]
        public EntityRef OrderRef;
    }
}