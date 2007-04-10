using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class GetDataForCancelOrderTableResponse : DataContractBase
    {
        public GetDataForCancelOrderTableResponse(List<CancelOrderTableItem> items, List<EnumValueInfo> cancelReasonChoices)
        {
            this.CancelOrderTableItems = items;
            this.CancelReasonChoices = cancelReasonChoices;
        }

        [DataMember]
        public List<CancelOrderTableItem> CancelOrderTableItems;

        [DataMember]
        public List<EnumValueInfo> CancelReasonChoices;
    }
}
