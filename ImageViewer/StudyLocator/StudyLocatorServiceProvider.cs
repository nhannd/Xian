using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.StudyLocator
{
	[ExtensionOf(typeof(ServiceProviderExtensionPoint))]
	public class StudyLocatorServiceProvider : IServiceProvider
	{
		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IStudyRootQuery))
				return new StudyRootQueryServiceClient();

			return null;
		}

		#endregion
	}
}