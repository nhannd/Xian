#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Help;
using ClearCanvas.Desktop.Help.UpdateTools.UpdateInformationService;
using ClearCanvas.Desktop.Tools;

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
			using (UpdateInformationService.UpdateInformationService service = new UpdateInformationService.UpdateInformationService())
			{
				service.Url = Settings.Default.UpdateInformationServiceUrl;

				Product installedProduct = new Product();
				installedProduct.Name = Application.Name;
				installedProduct.Version = Application.Version.ToString();

				try
				{
					UpdateInformationRequest request = new UpdateInformationRequest();
					request.InstalledProduct = installedProduct;
					UpdateInformationResult result = service.GetUpdateInformation(request);
					if (result == null)
						throw new Exception("Bad data received from service.");

					if (!IsValidProduct(result.InstalledProduct) || IsSameProduct(result.InstalledProduct, installedProduct))
					{
						base.Context.DesktopWindow.ShowMessageBox(SR.MessageNoUpdate, Common.MessageBoxActions.Ok);
					}
					else
					{
						string message = String.Format(SR.MessageUpdateAvailable,
							result.InstalledProduct.Name, result.InstalledProduct.Version);

						UpdateAvailableForm.Show(message, result.DownloadUrl);
					}
				}
				catch (Exception e)
				{
					Common.Platform.Log(Common.LogLevel.Warn, e, "The request for update information failed.");
					base.Context.DesktopWindow.ShowMessageBox(SR.MessageUpdateRequestFailed, Common.MessageBoxActions.Ok);
				}
			}
		}

		private static bool IsValidProduct(Product product)
		{
			return product != null && !String.IsNullOrEmpty(product.Version);
		}

		private static bool IsSameProduct(Product product1, Product product2)
		{
			return product1.Name == product2.Name && product1.Version == product2.Version;
		}
	}
}
