using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	[DataContract]
	public class QueryCancelOrderWarningsResponse : DataContractBase
	{
		public QueryCancelOrderWarningsResponse(List<string> warnings)
		{
			Warnings = warnings;
		}

		[DataMember]
		public List<string> Warnings;
	}
}
