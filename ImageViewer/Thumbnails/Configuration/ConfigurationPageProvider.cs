#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Thumbnails.Configuration
{
	[ExtensionOf(typeof (ConfigurationPageProviderExtensionPoint))]
	public sealed class ConfigurationPageProvider : IConfigurationPageProvider
	{
		public IEnumerable<IConfigurationPage> GetPages()
		{
			if (PermissionsHelper.IsInRole(AuthorityTokens.ViewerVisible))
				yield return new ConfigurationPage(ThumbnailsConfigurationComponent.Path, new ThumbnailsConfigurationComponent());
		}
	}
}