#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Configuration.Tools
{
	/// <summary>
	/// A tool that, when executed, will show a configuration dialog.
	/// </summary>
	[MenuAction("show", "global-menus/MenuTools/MenuOptions", "Show", KeyStroke = XKeys.Control | XKeys.O)]
	[Tooltip("show", "MenuOptions")]
	[IconSet("show", "Icons.OptionsToolSmall.png", "Icons.OptionsToolMedium.png", "Icons.OptionsToolLarge.png")]
	[GroupHint("show", "Application.Options")]

	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class OptionsTool : Tool<IDesktopToolContext>
	{
		/// <summary>
		/// Called by the framework when the user clicks the "Options" menu item.
		/// </summary>
		public void Show()
		{
			try
			{
				ConfigurationDialog.Show(this.Context.DesktopWindow);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
