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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Modality
{
    [MenuAction("show", "global-menus/Go/Technologist Home")]
    [ClickHandler("show", "Show")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class LaunchFolderExplorerTool : Tool<IDesktopToolContext>
    {
        public void Show()
        {
            FolderExplorerComponent folderComponent = new FolderExplorerComponent(new ModalityFolderExplorerToolExtensionPoint());
            AcquisitionWorkflowPreviewComponent previewComponent = new AcquisitionWorkflowPreviewComponent();

            folderComponent.SelectedItemsChanged += delegate(object sender, EventArgs args)
            {
                WorklistQueryResult item = folderComponent.SelectedItems.Item as WorklistQueryResult;
                //previewComponent.WorklistItem = item;
            };

            SplitComponentContainer split = new SplitComponentContainer(
                new SplitPane("Folders", folderComponent, 1.0f),
                new SplitPane("Preview", previewComponent, 1.0f),
                SplitOrientation.Vertical);

            ApplicationComponent.LaunchAsWorkspace(
                this.Context.DesktopWindow,
                split,
                "Technologist Home",
                null);
        }
    }

    [ExtensionPoint]
    public class ModalityFolderExplorerToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    [ExtensionOf(typeof(ModalityFolderExplorerToolExtensionPoint))]
    public class ModalityWorkflowFolderSystemTool : Tool<IFolderExplorerToolContext>
    {
        private ModalityWorkflowFolderSystem _folderSystem;

        public ModalityWorkflowFolderSystemTool()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            _folderSystem = new ModalityWorkflowFolderSystem(this.Context);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_folderSystem != null) _folderSystem.Dispose();
            }
        }
    }
}
