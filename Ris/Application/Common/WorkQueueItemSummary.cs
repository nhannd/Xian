using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class WorkQueueItemSummary : DataContractBase
	{
		[DataMember]
		public EntityRef WorkQueueRef;

		[DataMember]
		public DateTime CreationTime;

		[DataMember]
		public DateTime ScheduledTime;

		[DataMember]
		public DateTime? ExpirationTime;

		[DataMember]
		public string User;

		[DataMember]
		public EnumValueInfo Type;

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