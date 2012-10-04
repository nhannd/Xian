#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace MyPlugin.TextEditor
{
	[SettingsGroupDescription("Stores settings for the text editor.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class TextEditorSettings
	{
		public TextEditorSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}