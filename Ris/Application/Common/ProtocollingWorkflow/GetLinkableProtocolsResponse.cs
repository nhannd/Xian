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
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	[DataContract]
	public class GetLinkableProtocolsResponse : DataContractBase
	{
		public GetLinkableProtocolsResponse(List<ReportingWorklistItemSummary> protocolItems)
		{
			this.ProtocolItems = protocolItems;
		}

		[DataMember]
		public List<ReportingWorklistItemSummary> ProtocolItems;
	}
}