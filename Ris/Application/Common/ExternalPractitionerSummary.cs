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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ExternalPractitionerSummary : DataContractBase, ICloneable, IEquatable<ExternalPractitionerSummary>
	{
		public ExternalPractitionerSummary(
			EntityRef pracRef,
			PersonNameDetail personNameDetail,
			string licenseNumber,
			string billingNumber,
			bool isVerified,
			DateTime? lastVerifiedTime,
			DateTime? lastEditedTime,
			bool deactivated)
		{
			this.PractitionerRef = pracRef;
			this.Name = personNameDetail;
			this.LicenseNumber = licenseNumber;
			this.BillingNumber = billingNumber;
			this.IsVerified = isVerified;
			this.LastVerifiedTime = lastVerifiedTime;
			this.LastEditedTime = lastEditedTime;
			this.Deactivated = deactivated;
		}

		public ExternalPractitionerSummary()
		{
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
		public bool Deactivated;

		public bool Equals(ExternalPractitionerSummary externalPractitionerSummary)
		{
			return externalPractitionerSummary != null && Equals(PractitionerRef, externalPractitionerSummary.PractitionerRef);
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || Equals(obj as ExternalPractitionerSummary);
		}

		public override int GetHashCode()
		{
			return PractitionerRef.GetHashCode();
		}

		#region ICloneable Members

		public object Clone()
		{
			return new ExternalPractitionerSummary(
				this.PractitionerRef,
				(PersonNameDetail)this.Name.Clone(),
				this.LicenseNumber,
				this.BillingNumber,
				this.IsVerified,
				this.LastVerifiedTime,
				this.LastEditedTime,
				this.Deactivated);
		}

		#endregion
	}
}
