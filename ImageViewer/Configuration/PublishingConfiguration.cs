#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Configuration
{
	public static class PublishingConfiguration
	{
		public static bool PublishLocalToSourceAE
		{
			get { return PublishingSettings.Default.PublishLocalToSourceAE; }
		}

		public static bool PublishToDefaultServers
		{
			get { return PublishingSettings.Default.PublishToDefaultServers; }
		}
	}
}