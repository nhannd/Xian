using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.DesktopServices.StudyLocator
{
	[ExtensionOf(typeof(ServiceProviderExtensionPoint))]
	public class StudyLocatorServiceProvider : IServiceProvider
	{
		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IStudyRootQuery))
			{
				if (AppDomain.CurrentDomain == DesktopServiceHostTool.HostAppDomain)
				{
					//just return an instance when in the same process/domain.
					return new ImageViewer.StudyLocator.StudyLocator();
				}
				else
				{
					//return the true service client.
					return new StudyRootQueryServiceClient();
				}
			}

			return null;
		}

		#endregion
	}
}