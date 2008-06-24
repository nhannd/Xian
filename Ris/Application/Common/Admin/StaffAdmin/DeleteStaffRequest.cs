using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
	[DataContract]
	public class DeleteStaffRequest : DataContractBase
	{
		public DeleteStaffRequest(EntityRef staffRef)
		{
			this.StaffRef = staffRef;
		}

		[DataMember]
		public EntityRef StaffRef;
	}
}
