using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	[DataContract]
	public class MergeOrderRequest : DataContractBase
	{
		public MergeOrderRequest(EntityRef sourceOrderRef, EntityRef destinationOrderRef)
		{
			this.SourceOrderRef = sourceOrderRef;
			this.DestinationOrderRef = destinationOrderRef;
		}

		[DataMember]
		public EntityRef SourceOrderRef;

		[DataMember]
		public EntityRef DestinationOrderRef;

		[DataMember]
		public bool DryRun;
	}
}
