#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class ConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			if (PermissionsHelper.IsInRole(AuthorityTokens.KeyImageAdministration))
				yield return new ConfigurationPage("KeyImageConfigurationPath", new KeyImageConfigurationComponent());
		}

		#endregion
	}
}
