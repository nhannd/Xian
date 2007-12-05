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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;
//using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("launch", "global-menus/Go/Technologist Home")]
    //[ButtonAction("launch", "global-toolbars/Go/Technologist Home")]
    [Tooltip("launch", "Technologist Home")]
	[IconSet("launch", IconScheme.Colour, "Icons.TechnologistHomeToolSmall.png", "Icons.TechnologistHomeToolMedium.png", "Icons.TechnologistHomeToolLarge.png")]
    [ClickHandler("launch", "Launch")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class TechnologistHomeTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    BuildComponent(),
                    SR.TitleTechnologistHome);
                    _workspace.Closed += delegate { _workspace = null; };
            }
            else
            {
                _workspace.Activate();
            }
        }

        private IApplicationComponent BuildComponent()
        {
            TechnologistPreviewComponent previewComponent = new TechnologistPreviewComponent();
            HomePageContainer homePage = new HomePageContainer(new TechnologistFolderExplorerToolExtensionPoint(), previewComponent);

            homePage.ContentsComponent.SelectedItemsChanged += delegate
            {
                ModalityWorklistItem item = homePage.ContentsComponent.SelectedItems.Item as ModalityWorklistItem;
                previewComponent.WorklistItem = item;
            };

            return homePage;
        }
    }
}
