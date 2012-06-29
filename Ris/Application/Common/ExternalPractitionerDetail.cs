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
	public class ExternalPractitionerDetail : DataContractBase, IEquatable<ExternalPractitionerDetail>
	{
		public ExternalPractitionerDetail(
			EntityRef practitionerRef,
			PersonNameDetail personNameDetail,
			string licenseNumber,
			string billingNumber,
			bool isVerified,
			DateTime? lastVerifiedTime,
			DateTime? lastEditedTime,
			List<ExternalPractitionerContactPointDetail> contactPoints,
			Dictionary<string, string> extendedProperties,
			ExternalPractitionerSummary mergeDestination,
			bool isMerged,
			bool deactivated)
		{
			this.PractitionerRef = practitionerRef;
			this.Name = personNameDetail;
			this.LicenseNumber = licenseNumber;
			this.BillingNumber = billingNumber;
			this.IsVerified = isVerified;
			this.LastVerifiedTime = lastVerifiedTime;
			this.LastEditedTime = lastEditedTime;
			this.ContactPoints = contactPoints;
			this.ExtendedProperties = extendedProperties;
			this.MergeDestination = mergeDestination;
			this.IsMerged = isMerged;
			this.Deactivated = deactivated;
		}

		public ExternalPractitionerDetail()
		{
			this.Name = new PersonNameDetail();
			this.ContactPoints = new List<ExternalPractitionerContactPointDetail>();
			this.ExtendedProperties = new Dictionary<string, string>();
		}

		[DataMember]
		public EntityRef PractitionerRef;

		[DataMember]
		public PersonNameDetail Name;

		[DataMember]
		public string LicenseNumber;

		[DataMember]
		public string BillingNumber;

		[DataMember]
		public bool IsVerified { get; private set; }

		[DataMember]
		public DateTime? LastVerifiedTime { get; private set; }

		[DataMember]
		public DateTime? LastEditedTime { get; private set; }

		[DataMember]
		public List<ExternalPractitionerContactPointDetail> ContactPoints;

		[DataMember]
		public Dictionary<string, string> ExtendedProperties;

		[DataMember]
		public ExternalPractitionerSummary MergeDestination;

		[DataMember]
		public bool IsMerged;

		[DataMember]
		public bool Deactivated;

		public ExternalPractitionerSummary CreateSummary()
		{
			return new ExternalPractitionerSummary(
				this.PractitionerRef,
				this.Name,
				this.LicenseNumber,
				this.BillingNumber,
				this.IsVerified,
				this.LastVerifiedTime,
				this.LastEditedTime,
				this.IsMerged,
				this.Deactivated);
		}

		public bool Equals(ExternalPractitionerDetail detail)
		{
			if (detail == null)
				return false;

			return Equals(this.PractitionerRef, detail.PractitionerRef);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) 
				return true;

			return Equals(obj as ExternalPractitionerDetail);
		}

		public override int GetHashCode()
		{
			return this.PractitionerRef != null ? this.PractitionerRef.GetHashCode() : 0;
		}
	}
}
