using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	[DataContract]
	public class MergeOrderResponse : DataContractBase
	{
		/// <summary>
		/// If a dry-run was requested and succeeded, specifies what the merged order would look like.
		/// </summary>
		[DataMember]
		public OrderDetail DryRunMergedOrder;

		/// <summary>
		/// If a dry-run was requested and failed, specifies the failure reason.
		/// </summary>
		[DataMember]
		public string DryRunFailureReason;
	}
}
