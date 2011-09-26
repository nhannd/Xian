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
	/// <summary>
	/// Helper class to get the current SOP instance publishing configuration.
	/// </summary>
	public static class PublishingConfiguration
	{
		/// <summary>
		/// Gets a value indicating whether or not created SOP instances are to be published to the Source AE as specified in the study's headers.
		/// </summary>
		public static bool PublishLocalToSourceAE
		{
			get { return PublishingSettings.Default.PublishLocalToSourceAE; }
		}

		/// <summary>
		/// Gets a value indicating whether or not created SOP instances are to be published to the Default Servers.
		/// </summary>
		public static bool PublishToDefaultServers
		{
			get { return PublishingSettings.Default.PublishToDefaultServers; }
		}
	}
}