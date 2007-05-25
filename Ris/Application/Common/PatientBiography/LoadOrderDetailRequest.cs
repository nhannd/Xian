using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientBiography
{
    [DataContract]
    public class LoadOrderDetailRequest : DataContractBase
    {
        public LoadOrderDetailRequest(EntityRef orderRef)
        {
            this.OrderRef = orderRef;
        }

        [DataMember]
        public EntityRef OrderRef;
    }
}
