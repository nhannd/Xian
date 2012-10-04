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

namespace MyPlugin.TextEditor
{
	[ExtensionOf(typeof (ConfigurationPageProviderExtensionPoint))]
	public class ConfigPageProvider : IConfigurationPageProvider
	{
		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> list = new List<IConfigurationPage>();
			list.Add(new ConfigurationPage<TextEditorConfigComponent>("TextEditor"));
			return list.AsReadOnly();
		}
	}
}