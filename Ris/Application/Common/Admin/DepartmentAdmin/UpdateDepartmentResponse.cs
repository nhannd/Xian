using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DepartmentAdmin
{
	[DataContract]
	public class UpdateDepartmentResponse : DataContractBase
	{
		public UpdateDepartmentResponse(DepartmentSummary summary)
		{
			this.Department = summary;
		}

		[DataMember]
		public DepartmentSummary Department;
	}
}
