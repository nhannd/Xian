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

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class LoadExternalPractitionerForEditRequest : DataContractBase
	{
		public LoadExternalPractitionerForEditRequest()
		{
		}

		public LoadExternalPractitionerForEditRequest(EntityRef pracRef)
			: this(pracRef, false)
		{
		}

		public LoadExternalPractitionerForEditRequest(EntityRef pracRef, bool includeAlerts)
		{
			this.PractitionerRef = pracRef;
			this.IncludeAlerts = includeAlerts;
		}

		[DataMember]
		public EntityRef PractitionerRef;

		[DataMember]
		public bool IncludeAlerts;
	}
}
