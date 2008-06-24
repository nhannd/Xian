using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin
{
	[DataContract]
	public class DeleteModalityRequest : DataContractBase
	{
		public DeleteModalityRequest(EntityRef modalityRef)
		{
			this.ModalityRef = modalityRef;
		}

		[DataMember]
		public EntityRef ModalityRef;
	}
}
