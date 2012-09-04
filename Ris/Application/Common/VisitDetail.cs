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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class VisitDetail : DataContractBase
	{
		public VisitDetail()
		{
			this.VisitNumber = new CompositeIdentifierDetail();
			this.Locations = new List<VisitLocationDetail>();
			this.Practitioners = new List<VisitPractitionerDetail>();
			this.AmbulatoryStatuses = new List<EnumValueInfo>();
			this.ExtendedProperties = new Dictionary<string, string>();
		}

		[DataMember]
		public EntityRef VisitRef;

		[DataMember]
		public PatientProfileSummary Patient;

		[DataMember]
		public CompositeIdentifierDetail VisitNumber;

		[DataMember]
		public EnumValueInfo PatientClass;

		[DataMember]
		public EnumValueInfo PatientType;

		[DataMember]
		public EnumValueInfo AdmissionType;

		[DataMember]
		public EnumValueInfo Status;

		[DataMember]
		public DateTime? AdmitTime;

		[DataMember]
		public DateTime? DischargeTime;

		[DataMember]
		public FacilitySummary Facility;

		[DataMember]
		public LocationSummary CurrentLocation;

		[DataMember]
		public string CurrentRoom;

		[DataMember]
		public string CurrentBed;

		[DataMember]
		public string DischargeDisposition;

		[DataMember]
		public List<VisitLocationDetail> Locations;

		[DataMember]
		public List<VisitPractitionerDetail> Practitioners;

		[DataMember]
		public bool VipIndicator;

		[DataMember]
		public string PreadmitNumber;

		[DataMember]
		public List<EnumValueInfo> AmbulatoryStatuses;

		[DataMember]
		public Dictionary<string, string> ExtendedProperties;

		public VisitSummary GetSummary()
		{
			return new VisitSummary(this.VisitRef, this.Patient, this.VisitNumber,
				this.PatientClass, this.PatientType, this.AdmissionType,
				this.Status, this.AdmitTime, this.DischargeTime, this.Facility, this.CurrentLocation, this.CurrentRoom, this.CurrentBed);
		}
	}
}
