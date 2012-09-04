#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class MergeExternalPractitionerRequest : DataContractBase
	{
		[DataMember]
		public EntityRef RightPractitionerRef;

		[DataMember]
		public EntityRef LeftPractitionerRef;

		[DataMember]
		public PersonNameDetail Name;

		[DataMember]
		public string LicenseNumber;

		[DataMember]
		public string BillingNumber;

		[DataMember]
		public Dictionary<string, string> ExtendedProperties;

		[DataMember]
		public EntityRef DefaultContactPointRef;

		[DataMember]
		public List<EntityRef> DeactivatedContactPointRefs;

		[DataMember]
		public Dictionary<EntityRef, EntityRef> ContactPointReplacements;

		/// <summary>
		/// If true, no merge will actually be performed.  Instead, the server will return some estimated
		/// measures of the cost of the merge operation if it were to be performed.
		/// </summary>
		[DataMember]
		public bool EstimateCostOnly;
	}
}
