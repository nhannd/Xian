#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Configuration
{
	public sealed class ConfigurationDialog
	{

		private ConfigurationDialog()
		{
		}

		/// <summary>
		/// Shows all <see cref="IConfigurationPage"/>s returned by extensions of <see cref="IConfigurationPageProvider"/>
		/// in a dialog, with a navigable tree to select the pages.
		/// </summary>
		public static ApplicationComponentExitCode Show(IDesktopWindow desktopWindow)
		{
			return Show(desktopWindow, null);
		}

		/// <summary>
		/// Shows all <see cref="IConfigurationPage"/>s returned by extensions of <see cref="IConfigurationPageProvider"/>
		/// in a dialog, with a navigable tree to select the pages.
		/// </summary>
		public static ApplicationComponentExitCode Show(IDesktopWindow desktopWindow, string initialPageIdentifier)
		{
			ConfigurationDialogComponent container = new ConfigurationDialogComponent(initialPageIdentifier);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(desktopWindow, container, SR.TitleMenuOptions);

			return exitCode;
		}
	}
}
