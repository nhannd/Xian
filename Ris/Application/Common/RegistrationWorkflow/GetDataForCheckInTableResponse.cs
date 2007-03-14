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
        List<CheckInTableItem> CheckInTableItems;
    }

    [DataContract]
    public class CheckInTableItem : DataContractBase
    {
        public CheckInTableItem(string rpName, DateTime? schedulingDate, string orderingFacility)
        {
            this.RequestedProcedureName = rpName;
            this.SchedulingDate = schedulingDate;
            this.OrderingFacility = orderingFacility;
        }

        [DataMember]
        public string RequestedProcedureName;

        [DataMember]
        public DateTime? SchedulingDate;

        [DataMember]
        public string OrderingFacility;
    }
}
