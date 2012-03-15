#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ClearCanvas.Common.Serialization;
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
