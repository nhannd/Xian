using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DepartmentAdmin
{
	[DataContract]
	public class AddDepartmentResponse : DataContractBase
	{
		public AddDepartmentResponse(DepartmentSummary summary)
		{
			this.Department = summary;
		}

		[DataMember]
		public DepartmentSummary Department;
	}
}
