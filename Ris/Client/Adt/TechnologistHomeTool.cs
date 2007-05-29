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
    [ButtonAction("launch", "global-toolbars/Go/Technologist Home")]
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
                    SR.TitleTechnologistHome,
                    delegate(IApplicationComponent c) { _workspace = null; });
            }
            else
            {
                _workspace.Activate();
            }
        }

        private IApplicationComponent BuildComponent()
        {
            FolderExplorerComponent folderComponent = new FolderExplorerComponent(new TechnologistFolderExplorerToolExtensionPoint());
            TechnologistPreviewComponent previewComponent = new TechnologistPreviewComponent();

            folderComponent.SelectedItemsChanged += delegate(object sender, EventArgs args)
            {
                ModalityWorklistItem item = folderComponent.SelectedItems.Item as ModalityWorklistItem;
                previewComponent.WorklistItem = item;
            };

            return new SplitComponentContainer(
                new SplitPane("Folders", folderComponent, 1.0f),
                new SplitPane("Preview", previewComponent, 1.0f),
                SplitOrientation.Vertical);
        }
    }
}
