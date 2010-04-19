using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DepartmentAdmin
{
	[DataContract]
	public class AddDepartmentRequest : DataContractBase
	{
		public AddDepartmentRequest(DepartmentDetail detail)
		{
			this.Department = detail;
		}

		[DataMember]
		public DepartmentDetail Department;
	}
}
