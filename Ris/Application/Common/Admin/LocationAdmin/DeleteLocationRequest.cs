using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.LocationAdmin
{
	[DataContract]
	public class DeleteLocationRequest : DataContractBase
	{
		public DeleteLocationRequest(EntityRef locationRef)
		{
			this.LocationRef = locationRef;
		}

		[DataMember]
		public EntityRef LocationRef;
	}
}
