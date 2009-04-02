using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	[DataContract]
	public class ListSettingsPropertiesResponse : DataContractBase
	{
		public ListSettingsPropertiesResponse(List<SettingsPropertyDescriptor> properties)
		{
			Properties = properties;
		}

		[DataMember]
		public List<SettingsPropertyDescriptor> Properties;
	}
}
