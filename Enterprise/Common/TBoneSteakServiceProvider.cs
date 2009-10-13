using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Configuration;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Provides backwards compatability for Workstation 2.0 with Enterprise server 1.5 by mapping
	/// IApplicationConfigurationReadService to IConfigurationService
	/// </summary>
	[ExtensionOf(typeof(ServiceProviderExtensionPoint), Enabled = false)]
	public class TBoneSteakServiceProvider : IServiceProvider
	{
		class Proxy : IApplicationConfigurationReadService
		{
			public ListSettingsGroupsResponse ListSettingsGroups(ListSettingsGroupsRequest request)
			{
				ListSettingsGroupsResponse response = null;
				Platform.GetService<IConfigurationService>(
					service => response = service.ListSettingsGroups(request));
				return response;
			}

			public ListSettingsPropertiesResponse ListSettingsProperties(ListSettingsPropertiesRequest request)
			{
				ListSettingsPropertiesResponse response = null;
				Platform.GetService<IConfigurationService>(
					service => response = service.ListSettingsProperties(request));
				return response;
			}

			public GetConfigurationDocumentResponse GetConfigurationDocument(GetConfigurationDocumentRequest request)
			{
				GetConfigurationDocumentResponse response = null;
				Platform.GetService<IConfigurationService>(
					service => response = service.GetConfigurationDocument(request));
				return response;
			}
		}




		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if(serviceType != typeof(IApplicationConfigurationReadService))
				return null;

			return new Proxy();
		}

		#endregion
	}
}
