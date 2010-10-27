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

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class MergeExternalPractitionerResponse : DataContractBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="updatedOriginal">The updated data for the original record.</param>
		public MergeExternalPractitionerResponse(ExternalPractitionerSummary updatedOriginal)
		{
			this.UpdatedOriginal = updatedOriginal;
		}

		[DataMember]
		public ExternalPractitionerSummary UpdatedOriginal;
	}
}
