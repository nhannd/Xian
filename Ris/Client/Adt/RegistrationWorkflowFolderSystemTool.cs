using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("show", "global-menus/Go/Registration Home")]
    [ClickHandler("show", "Show")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class RegistrationLaunchFolderExplorerTool : Tool<IDesktopToolContext>
    {
        public void Show()
        {
            FolderExplorerComponent folderComponent = new FolderExplorerComponent(new RegistrationFolderExplorerToolExtensionPoint());
            RegistrationPreviewComponent previewComponent = new RegistrationPreviewComponent();

            folderComponent.SelectedItemsChanged += delegate(object sender, EventArgs args)
            {
                RegistrationWorklistItem item = folderComponent.SelectedItems.Item as RegistrationWorklistItem;
                previewComponent.WorklistItem = item;
            };

            SplitComponentContainer split = new SplitComponentContainer(
                new SplitPane("Folders", folderComponent, 1.0f),
                new SplitPane("Preview", previewComponent, 1.0f),
                SplitOrientation.Vertical);

            ApplicationComponent.LaunchAsWorkspace(
                this.Context.DesktopWindow,
                split,
                "Registration Home",
                null);
        }
    }

    [ExtensionPoint]
    public class RegistrationFolderExplorerToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    [ExtensionOf(typeof(RegistrationFolderExplorerToolExtensionPoint))]
    public class RegistrationWorkflowFolderSystemTool : Tool<IFolderExplorerToolContext>
    {
        private RegistrationWorkflowFolderSystem _folderSystem;

        public RegistrationWorkflowFolderSystemTool()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            _folderSystem = new RegistrationWorkflowFolderSystem(this.Context);
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
