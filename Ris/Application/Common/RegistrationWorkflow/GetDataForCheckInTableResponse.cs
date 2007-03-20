using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class GetDataForCheckInTableResponse : DataContractBase
    {
        public GetDataForCheckInTableResponse(List<CheckInTableItem> items)
        {
            this.CheckInTableItems = items;
        }

        [DataMember]
        public List<CheckInTableItem> CheckInTableItems;
    }
}
