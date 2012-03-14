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
	public partial class DiskspaceManagerServiceClient : ClientBase<IDiskspaceManagerService>, IDiskspaceManagerService
	{
		public DiskspaceManagerServiceClient()
		{
		}

		public DiskspaceManagerServiceInformation GetServiceInformation()
		{
			return base.Channel.GetServiceInformation();
		}

		public void UpdateServiceConfiguration(DiskspaceManagerServiceConfiguration newConfiguration)
		{
			base.Channel.UpdateServiceConfiguration(newConfiguration);
		}
	}
}
