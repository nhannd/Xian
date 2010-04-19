using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DepartmentAdmin
{
	[DataContract]
	public class LoadDepartmentEditorFormDataResponse : DataContractBase
	{
		public LoadDepartmentEditorFormDataResponse(List<FacilitySummary> facilityChoices)
		{
			FacilityChoices = facilityChoices;
		}

		[DataMember]
		public List<FacilitySummary> FacilityChoices;
	}
}
