#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class ListExternalPractitionersResponse : DataContractBase
	{
		public ListExternalPractitionersResponse(List<ExternalPractitionerSummary> pracs)
			: this(pracs, -1)
		{
		}

		public ListExternalPractitionersResponse(List<ExternalPractitionerSummary> pracs, int itemCount)
		{
			this.Practitioners = pracs;
			this.ItemCount = itemCount;
		}

		[DataMember]
		public List<ExternalPractitionerSummary> Practitioners;

		[DataMember]
		public int ItemCount;
	}
}
