#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class MergeDuplicateContactPointResponse : DataContractBase
	{
		public MergeDuplicateContactPointResponse(ExternalPractitionerContactPointSummary mergedContactPoint)
		{
			MergedContactPoint = mergedContactPoint;
		}

		public MergeDuplicateContactPointResponse(long costEstimate)
		{
			this.CostEstimate = costEstimate;
		}

		[DataMember]
		public ExternalPractitionerContactPointSummary MergedContactPoint;

		[DataMember]
		public long CostEstimate;


	}
}
