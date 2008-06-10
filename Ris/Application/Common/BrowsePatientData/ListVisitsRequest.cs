using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.BrowsePatientData
{
	[DataContract]
	public class ListVisitsRequest : DataContractBase
	{
		public ListVisitsRequest(EntityRef patientRef)
		{
			PatientRef = patientRef;
		}

		[DataMember]
		public EntityRef PatientRef;
	}
}
