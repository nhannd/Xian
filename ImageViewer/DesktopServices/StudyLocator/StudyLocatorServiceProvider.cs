#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
					return new Configuration.StudyLocator();
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