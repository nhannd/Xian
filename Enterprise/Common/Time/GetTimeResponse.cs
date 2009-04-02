using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Time
{
	[DataContract]
	public class GetTimeResponse : DataContractBase
	{
		public GetTimeResponse(DateTime time)
		{
			Time = time;
		}

		[DataMember]
		public DateTime Time;
	}
}
