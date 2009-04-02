using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	[DataContract]
	public class ListSettingsPropertiesRequest : DataContractBase
	{
		public ListSettingsPropertiesRequest(SettingsGroupDescriptor group)
		{
			Group = group;
		}

		[DataMember]
		public SettingsGroupDescriptor Group;
	}
}
