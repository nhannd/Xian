#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class WorkQueueItemSummary : DataContractBase
	{
		[DataMember]
		public EntityRef WorkQueueItemRef;

		[DataMember]
		public DateTime CreationTime;

		[DataMember]
		public DateTime ScheduledTime;

		[DataMember]
		public DateTime? ExpirationTime;

		[DataMember]
		public string User;

		[DataMember]
		public string Type;

		[DataMember]
		public EnumValueInfo Status;

		[DataMember]
		public DateTime? ProcessedTime;

		[DataMember]
		public int FailureCount;

		[DataMember]
		public string FailureDescription;
	}
}