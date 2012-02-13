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
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class PresetVoiLutConfigurationPageProvider : IConfigurationPageProvider
	{
		public PresetVoiLutConfigurationPageProvider()
		{

		}

		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();
			if (PermissionsHelper.IsInRole(AuthorityTokens.ViewerVisible))
				listPages.Add(new ConfigurationPage<PresetVoiLutConfigurationComponent>("TitleTools/TitleWindowLevel"));
			return listPages.AsReadOnly();
		}

		#endregion
	}
}