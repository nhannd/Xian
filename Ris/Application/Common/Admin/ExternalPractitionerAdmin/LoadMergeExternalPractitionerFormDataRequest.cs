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
	public class LoadMergeExternalPractitionerFormDataRequest : DataContractBase
	{
		public LoadMergeExternalPractitionerFormDataRequest()
		{
			this.DeactivatedContactPointRefs = new List<EntityRef>();
		}

		/// <summary>
		/// Specifies the reference of an external practitioner.
		/// Request to return a list of duplicate external practitioners.
		/// </summary>
		[DataMember]
		public EntityRef PractitionerRef;

		/// <summary>
		/// A list of contact point references that will become deactivated.
		/// Request to return a list of orders affected by these contact points.
		/// </summary>
		[DataMember]
		public List<EntityRef> DeactivatedContactPointRefs;
	}
}
