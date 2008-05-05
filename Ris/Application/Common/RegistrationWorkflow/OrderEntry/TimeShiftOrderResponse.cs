using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	[DataContract]
	public class TimeShiftOrderResponse : DataContractBase
	{
		public TimeShiftOrderResponse(OrderSummary order)
		{
			Order = order;
		}

		[DataMember]
		public OrderSummary Order;
	}
}
