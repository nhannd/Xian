#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class MergeDuplicateContactPointRequest : DataContractBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="duplicate">The duplicate contact point to remove.</param>
		/// <param name="original">The original contact point to keep.</param>
		public MergeDuplicateContactPointRequest(
			ExternalPractitionerContactPointSummary duplicate, 
			ExternalPractitionerContactPointSummary original)
		{
			this.Duplicate = duplicate;
			this.Original = original;
		}

		[DataMember]
		public ExternalPractitionerContactPointSummary Duplicate;

		[DataMember]
		public ExternalPractitionerContactPointSummary Original;
	}
}
