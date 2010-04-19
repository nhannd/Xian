using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DepartmentAdmin
{
	[DataContract]
	public class DeleteDepartmentRequest : DataContractBase
	{
		public DeleteDepartmentRequest(EntityRef departmentRef)
		{
			DepartmentRef = departmentRef;
		}

		[DataMember]
		public EntityRef DepartmentRef;
	}
}
