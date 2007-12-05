#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    [MenuAction("launch", "global-menus/Go/Radiologist Home", "Launch")]
    //[ButtonAction("launch", "global-toolbars/Go/Radiologist Home", "Launch")]
    [Tooltip("launch", "Radiologist Home")]
    [IconSet("launch", IconScheme.Colour, "Icons.RadiologistHomeToolSmall.png", "Icons.RadiologistHomeToolMedium.png", "Icons.RadiologistHomeToolLarge.png")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class HomeTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        BuildComponent(),
                        SR.TitleRadiologistHome);
                    _workspace.Closed += delegate { _workspace = null; };
                }
                catch (Exception e)
                {
                    // could not launch component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _workspace.Activate();
            }
        }

        private static IApplicationComponent BuildComponent()
        {
            ReportingPreviewComponent previewComponent = new ReportingPreviewComponent();
            HomePageContainer homePage = new HomePageContainer(new ReportingFolderExplorerToolExtensionPoint(), previewComponent);

            homePage.ContentsComponent.SelectedItemsChanged += delegate
            {
                ReportingWorklistItem item = homePage.ContentsComponent.SelectedItems.Item as ReportingWorklistItem;
                previewComponent.WorklistItem = item;
            };

            return homePage;
        }
    }
}
