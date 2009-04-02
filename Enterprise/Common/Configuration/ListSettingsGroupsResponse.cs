using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	[DataContract]
	public class ListSettingsGroupsResponse : DataContractBase
	{
		public ListSettingsGroupsResponse(List<SettingsGroupDescriptor> groups)
		{
			Groups = groups;
		}

		[DataMember]
		public List<SettingsGroupDescriptor> Groups;
	}
}
