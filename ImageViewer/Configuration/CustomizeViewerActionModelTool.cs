#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Configuration.ActionModel;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Configuration
{
	[MenuAction("customize", "global-menus/MenuTools/MenuCustomize", "Customize")]
	[GroupHint("customize", "Application.Options.Customize")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class CustomizeViewerActionModelTool : ImageViewerTool
	{
		public void Customize()
		{
			try
			{
				TabComponentContainer tabContainer = new TabComponentContainer();

				//tabContainer.Pages.Add(new TabPage("IV Main menu", new ActionModelConfigurationComponent(
				//                                                    "ClearCanvas.Desktop.DesktopWindow",
				//                                                    "global-menus",
				//                                                    this.ImageViewer.ExportedActions,
				//                                                    this.ImageViewer.DesktopWindow)));

				tabContainer.Pages.Add(new TabPage(SR.LabelToolbar, new ActionModelConfigurationComponent(
				                                                    	typeof (DesktopWindow).FullName,
				                                                    	"global-toolbars",
				                                                    	this.ImageViewer.ExportedActions,
				                                                    	this.ImageViewer.DesktopWindow)));

				//tabContainer.Pages.Add(new TabPage("IV Context", new ActionModelConfigurationComponent(
				//                                                    "ClearCanvas.ImageViewer.ImageViewerComponent",
				//                                                    "imageviewer-contextmenu",
				//                                                    this.ImageViewer.ExportedActions,
				//                                                    this.ImageViewer.DesktopWindow)));

				//tabContainer.Pages.Add(new TabPage("MagTool DD", new ActionModelConfigurationComponent(
				//                                                    "ClearCanvas.ImageViewer.Tools.Standard.MagnificationTool",
				//                                                    "magnification-dropdown",
				//                                                    this.ImageViewer.ExportedActions,
				//                                                    this.ImageViewer.DesktopWindow)));

				ApplicationComponentExitCode result = ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					new SimpleComponentContainer(tabContainer),
					SR.TitleCustomizeActionModels);

				if (result == ApplicationComponentExitCode.Accepted)
				{
					foreach (ActionModelConfigurationComponent component in tabContainer.ContainedComponents)
					{
						component.Save();
					}
				}
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, this.Context.DesktopWindow);
			}
		}
	}
}