using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DepartmentAdmin
{
	[DataContract]
	public class ListDepartmentsResponse : DataContractBase
	{
		public ListDepartmentsResponse(List<DepartmentSummary> Departments)
		{
			this.Departments = Departments;
		}

		[DataMember]
		public List<DepartmentSummary> Departments;
	}
}
