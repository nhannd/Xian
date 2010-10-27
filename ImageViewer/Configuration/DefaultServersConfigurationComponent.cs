#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Specialized;
using ClearCanvas.ImageViewer.Configuration.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration
{
	public class DefaultServersConfigurationComponent : ServerTreeConfigurationComponent
	{
		public DefaultServersConfigurationComponent()
			:base(SR.DescriptionDefaultServers)
		{
		}

		public override void Save()
		{
			DefaultServerSettings.Default.DefaultServerPaths = new StringCollection();

			foreach (string path in SelectedServerPaths)
				DefaultServerSettings.Default.DefaultServerPaths.Add(path);
			
			DefaultServerSettings.Default.Save();
		}
	}
}
