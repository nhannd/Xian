using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    [MenuAction("launch", "global-menus/Go/Radiologist Home")]
    [ButtonAction("launch", "global-toolbars/Go/Radiologist Home")]
    [Tooltip("launch", "Radiologist Home")]
    [IconSet("launch", IconScheme.Colour, "Icons.RadiologistHomeToolSmall.png", "Icons.RadiologistHomeToolMedium.png", "Icons.RadiologistHomeToolLarge.png")]
    [ClickHandler("launch", "Launch")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class HomeTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    BuildComponent(),
                    SR.TitleRadiologistHome,
                    delegate(IApplicationComponent c) { _workspace = null; });
            }
            else
            {
                _workspace.Activate();
            }
        }

        private IApplicationComponent BuildComponent()
        {
            FolderExplorerComponent folderComponent = new FolderExplorerComponent(new ReportingFolderExplorerToolExtensionPoint());
            ReportingPreviewComponent previewComponent = new ReportingPreviewComponent();

            folderComponent.SelectedItemsChanged += delegate(object sender, EventArgs args)
            {
                ReportingWorklistItem item = folderComponent.SelectedItems.Item as ReportingWorklistItem;
                previewComponent.WorklistItem = item;
            };

            return new SplitComponentContainer(
                new SplitPane("Folders", folderComponent, 1.0f),
                new SplitPane("Preview", previewComponent, 1.0f),
                SplitOrientation.Vertical);
        }
    }
}
