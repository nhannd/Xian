#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class VisitSummary : DataContractBase, IEquatable<VisitSummary>
	{
		public VisitSummary()
		{
		}

		public VisitSummary(
			EntityRef visitRef,
			PatientProfileSummary patient,
			CompositeIdentifierDetail visitNumber,
			EnumValueInfo patientClass,
			EnumValueInfo patientType,
			EnumValueInfo admissionType,
			EnumValueInfo status,
			DateTime? admitTime,
			DateTime? dischargeTime,
			FacilitySummary facility,
			LocationSummary currentLocation,
			string currentRoom,
			string currentBed)
		{
			this.VisitRef = visitRef;
			this.Patient = patient;
			this.VisitNumber = visitNumber;
			this.PatientClass = patientClass;
			this.PatientType = patientType;
			this.AdmissionType = admissionType;
			this.Status = status;
			this.AdmitTime = admitTime;
			this.DischargeTime = dischargeTime;
			this.Facility = facility;
			this.CurrentLocation = currentLocation;
			this.CurrentRoom = currentRoom;
			this.CurrentBed = currentBed;
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

		public bool Equals(VisitSummary visitSummary)
		{
			if (visitSummary == null) return false;
			return Equals(VisitRef, visitSummary.VisitRef);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as VisitSummary);
		}

		public override int GetHashCode()
		{
			return VisitRef.GetHashCode();
		}
	}
}
