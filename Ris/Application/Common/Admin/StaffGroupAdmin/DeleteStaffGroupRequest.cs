using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin
{
	[DataContract]
	public class DeleteStaffGroupRequest : DataContractBase
	{
		public DeleteStaffGroupRequest(EntityRef staffGroupRef)
		{
			this.StaffGroupRef = staffGroupRef;
		}

		[DataMember]
		public EntityRef StaffGroupRef;
	}
}
