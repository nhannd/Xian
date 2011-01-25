#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Help;
using ClearCanvas.Desktop.Help.UpdateTools.UpdateInformationService;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Help.UpdateTools
{
	[MenuAction("checkForUpdates", "global-menus/MenuHelp/MenuCheckForUpdates", "CheckForUpdates")]
	[GroupHint("checkForUpdates", "Application.Help.Updates")]

	[Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class UpdateTool : Tool<IDesktopToolContext>
	{
		public UpdateTool()
		{
		}

		public void CheckForUpdates()
		{
			using (var service = new UpdateInformationService.UpdateInformationService())
			{
				service.Url = Settings.Default.UpdateInformationServiceUrl;

				var installedProduct = new Product
				                               	{
				                               		Name = ProductInformation.Component,
				                               		Version = ProductInformation.Version.ToString(),
				                               		VersionSuffix = ProductInformation.VersionSuffix,
				                               		Edition = ProductInformation.Edition,
				                               		Release = ProductInformation.Release
				                               	};

				try
				{
					var request = new UpdateInformationRequest {InstalledProduct = installedProduct};
					UpdateInformationResult result = service.GetUpdateInformation(request);
					if (result == null)
						throw new Exception("Bad data received from service.");

					if (!IsValidComponent(result.InstalledProduct) || IsSameComponent(result.InstalledProduct, installedProduct))
					{
						base.Context.DesktopWindow.ShowMessageBox(SR.MessageNoUpdate, MessageBoxActions.Ok);
					}
					else
					{
						var upgrade = result.InstalledProduct;
						string message = String.Format(SR.MessageUpdateAvailable, ToString(upgrade));
						UpdateAvailableForm.Show(message, result.DownloadUrl);
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "The request for update information failed.");
					Context.DesktopWindow.ShowMessageBox(SR.MessageUpdateRequestFailed, MessageBoxActions.Ok);
				}
			}
		}

		private string ToString(Component component)
		{
			var builder = new StringBuilder();

			builder.Append(String.IsNullOrEmpty(component.Name) ? "Unknown" : component.Name);
			builder.AppendFormat(" {0}", String.IsNullOrEmpty(component.Version) ? "?" : component.Version);
			if (!String.IsNullOrEmpty(component.Edition))
				builder.AppendFormat(" {0}", component.Edition);

			if (!String.IsNullOrEmpty(component.VersionSuffix))
				builder.AppendFormat(" {0}", component.VersionSuffix);

			if (!String.IsNullOrEmpty(component.Release))
				builder.AppendFormat(" {0}", component.Release);

			return builder.ToString();

		}

		private bool IsSameComponent(Component component1, Component component2)
		{
			return component1.Name == component2.Name && component1.Version == component2.Version;
		}

		private static bool IsValidComponent(Component component)
		{
			return component != null && !String.IsNullOrEmpty(component.Version);
		}
	}
}
