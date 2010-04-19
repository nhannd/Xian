using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DepartmentAdmin
{
	[DataContract]
	public class UpdateDepartmentRequest : DataContractBase
	{
		public UpdateDepartmentRequest(DepartmentDetail detail)
		{
			this.Department = detail;
		}

		[DataMember]
		public DepartmentDetail Department;
	}
}
