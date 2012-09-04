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

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class MergeDuplicatePractitionerRequest : DataContractBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="duplicate">The duplicate practitioners to remove.</param>
		/// <param name="original">The original record to keep.</param>
		public MergeDuplicatePractitionerRequest(ExternalPractitionerSummary duplicate, ExternalPractitionerSummary original)
		{
			this.Duplicate = duplicate;
			this.Original = original;
		}

		[DataMember]
		public ExternalPractitionerSummary Duplicate;

		[DataMember]
		public ExternalPractitionerSummary Original;
	}
}
