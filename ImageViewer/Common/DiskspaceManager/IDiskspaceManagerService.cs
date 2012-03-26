#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.DiskspaceManager
{
    // TODO (Marmot): refactor for new configuration. Rename to "Configuration" because that's what
    // it's for, and because it needs to work "offline", just like the DICOM server.
	[ServiceContract(ConfigurationName = "IDiskspaceManagerService")]
	public interface IDiskspaceManagerService
	{
		[OperationContract]
		DiskspaceManagerServiceInformation GetServiceInformation();

		[OperationContract]
		void UpdateServiceConfiguration(DiskspaceManagerServiceConfiguration newConfiguration);
	}
}
