using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	[DataContract]
	public class ReserveAccessionNumberResponse : DataContractBase
	{
		public ReserveAccessionNumberResponse(string accessionNumber)
		{
			this.AccessionNumber = accessionNumber;
		}

		[DataMember]
		public string AccessionNumber;
	}
}
