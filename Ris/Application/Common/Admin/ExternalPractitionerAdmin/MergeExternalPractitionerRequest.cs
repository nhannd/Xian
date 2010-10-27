#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class MergeExternalPractitionerRequest : DataContractBase
	{
		/// <summary>
		/// This data contract identifies exactly which contact point within an order is to be replaced
		/// </summary>
		[DataContract]
		public class AffectedOrderRecipientSummary : DataContractBase, IVersionedEquatable<AffectedOrderRecipientSummary>
		{
			public AffectedOrderRecipientSummary(EntityRef orderRef, EntityRef contactPointRef)
			{
				this.OrderRef = orderRef;
				this.ContactPointRef = contactPointRef;
			}

			[DataMember]
			public EntityRef OrderRef { get; private set; }

			[DataMember]
			public EntityRef ContactPointRef { get; private set; }

			public bool Equals(AffectedOrderRecipientSummary other)
			{
				return Equals(other, false);
			}

			public bool Equals(object other, bool ignoreVersion)
			{
				return Equals(other as AffectedOrderRecipientSummary, ignoreVersion);
			}

			public bool Equals(AffectedOrderRecipientSummary other, bool ignoreVersion)
			{
				if (other == null) return false;
				return Equals(other.OrderRef, other.ContactPointRef, ignoreVersion);
			}

			public bool Equals(EntityRef orderRef, EntityRef contactPointRef, bool ignoreVersion)
			{
				if (!EntityRef.Equals(this.OrderRef, orderRef, ignoreVersion)) return false;
				if (!EntityRef.Equals(this.ContactPointRef, contactPointRef, ignoreVersion)) return false;
				return true;
			}
		}

		[DataMember]
		public EntityRef DuplicatePractitionerRef;

		[DataMember]
		public ExternalPractitionerDetail MergedPractitioner;

		[DataMember]
		public Dictionary<AffectedOrderRecipientSummary, EntityRef> ContactPointReplacements;
	}
}
