using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.BrowsePatientData
{
	[DataContract]
	public class VisitListItem : DataContractBase
	{
		public VisitListItem()
		{
		}

		#region Visit

		[DataMember]
		public EntityRef VisitRef;

		[DataMember]
		public CompositeIdentifierDetail VisitNumber;

		[DataMember]
		public EnumValueInfo PatientClass;

		[DataMember]
		public EnumValueInfo PatientType;

		[DataMember]
		public EnumValueInfo AdmissionType;

		[DataMember]
		public EnumValueInfo VisitStatus;

		[DataMember]
		public DateTime? AdmitTime;

		[DataMember]
		public DateTime? DischargeTime;

		[DataMember]
		public FacilitySummary VisitFacility;

		[DataMember]
		public string PreadmitNumber;

		#endregion
	}
}
