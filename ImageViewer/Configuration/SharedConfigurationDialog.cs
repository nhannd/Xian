#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Configuration
{
    /// <summary>
    /// An extension point for <see cref="IConfigurationPageProvider"/>s.
    /// </summary>
    [ExtensionPoint]
    public sealed class SharedConfigurationPageProviderExtensionPoint : ExtensionPoint<IConfigurationPageProvider>
    {
    }

	public static class SharedConfigurationDialog
	{
	    /// <summary>
        /// Shows all <see cref="IConfigurationPage"/>s returned by extensions of
        /// <see cref="SharedConfigurationPageProviderExtensionPoint"/>
        /// in a dialog, with a navigable tree to select the pages.
		/// </summary>
		public static ApplicationComponentExitCode Show(IDesktopWindow desktopWindow)
		{
			return Show(desktopWindow, null);
		}

		/// <summary>
        /// Shows all <see cref="IConfigurationPage"/>s returned by extensions of
        /// <see cref="SharedConfigurationPageProviderExtensionPoint"/>
		/// in a dialog, with a navigable tree to select the pages.
		/// </summary>
		public static ApplicationComponentExitCode Show(IDesktopWindow desktopWindow, string initialPageIdentifier)
		{
            var container = new ConfigurationDialogComponent(GetPages(), initialPageIdentifier);
            var exitCode = ApplicationComponent.LaunchAsDialog(desktopWindow, container, SR.TitleSharedConfiguration);

			return exitCode;
		}

        private static IEnumerable<IConfigurationPage> GetPages()
        {
            try
            {
                return new SharedConfigurationPageProviderExtensionPoint().CreateExtensions()
                    .Cast<IConfigurationPageProvider>().SelectMany(p => p.GetPages()).ToList();
            }
            catch (NotSupportedException)
            {
                return new IConfigurationPage[0];
            }
        }
	}
}
