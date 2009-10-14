#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
