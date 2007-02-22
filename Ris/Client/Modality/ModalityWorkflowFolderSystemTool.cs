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
