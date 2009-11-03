using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	[DataContract]
	public class QueryCancelOrderWarningsRequest : DataContractBase
	{
		public QueryCancelOrderWarningsRequest(EntityRef orderRef)
		{
			OrderRef = orderRef;
		}

		/// <summary>
		/// Order to check.
		/// </summary>
		[DataMember]
		public EntityRef OrderRef;
	}
}
