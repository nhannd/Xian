using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class GetWorklistCountResponse : DataContractBase
    {
        public GetWorklistCountResponse(int itemCount)
        {
            this.ItemCount = itemCount;
        }

        [DataMember]
        public int ItemCount;
    }
}
