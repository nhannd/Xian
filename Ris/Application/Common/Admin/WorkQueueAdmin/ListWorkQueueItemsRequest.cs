#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin
{
	[DataContract]
	public class ListWorkQueueItemsRequest : ListRequestBase
	{
		public ListWorkQueueItemsRequest()
		{
		}

		public ListWorkQueueItemsRequest(SearchResultPage page)
			: base(page)
		{
		}

		[DataMember]
		public DateTime? ScheduledStartTime;

		[DataMember]
		public DateTime? ScheduledEndTime;

		[DataMember]
		public DateTime? ProcessedStartTime;

		[DataMember]
		public DateTime? ProcessedEndTime;

		[DataMember]
		public List<string> Types;

		[DataMember]
		public List<EnumValueInfo> Statuses;
	}
}