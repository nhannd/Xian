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
	public class MergeExternalPractitionerResponse : DataContractBase
	{

		public MergeExternalPractitionerResponse(ExternalPractitionerSummary mergedPractitioner)
		{
			MergedPractitioner = mergedPractitioner;
		}

		public MergeExternalPractitionerResponse(long costEstimate)
		{
			this.CostEstimate = costEstimate;
		}

		[DataMember]
		public ExternalPractitionerSummary MergedPractitioner;

		[DataMember]
		public long CostEstimate;
	}
}
