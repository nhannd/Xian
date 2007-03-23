using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("launch", "global-menus/Go/Registration Home")]
    [ButtonAction("launch", "global-toolbars/Go/Registration Home")]
    [Tooltip("launch", "Registration Home")]
    [IconSet("launch", IconScheme.Colour, "Icons.HomeToolSmall.png", "Icons.HomeToolMedium.png", "Icons.HomeToolLarge.png")]
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
                    SR.TitleRegistrationHome,
                    delegate(IApplicationComponent c) { _workspace = null; });
            }
            else
            {
                _workspace.Activate();
            }
        }

        private IApplicationComponent BuildComponent()
        {
            FolderExplorerComponent folderComponent = new FolderExplorerComponent(new RegistrationFolderExplorerToolExtensionPoint());
            RegistrationPreviewComponent previewComponent = new RegistrationPreviewComponent();

            folderComponent.SelectedItemsChanged += delegate(object sender, EventArgs args)
            {
                RegistrationWorklistItem item = folderComponent.SelectedItems.Item as RegistrationWorklistItem;
                previewComponent.WorklistItem = item;
            };

            return new SplitComponentContainer(
                new SplitPane("Folders", folderComponent, 1.0f),
                new SplitPane("Preview", previewComponent, 1.0f),
                SplitOrientation.Vertical);
        }
    }
}
