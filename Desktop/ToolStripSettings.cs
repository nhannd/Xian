#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using System.ComponentModel;
using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Stores general settings for toolbars and menus.
	/// </summary>
	[SettingsGroupDescription("Stores general settings for toolbars and menus.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	public sealed partial class ToolStripSettings {}
}