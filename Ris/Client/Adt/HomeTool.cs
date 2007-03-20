using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;

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
                WorklistItem item = folderComponent.SelectedItems.Item as WorklistItem;
                previewComponent.WorklistItem = item;
            };

            return new SplitComponentContainer(
                new SplitPane("Folders", folderComponent, 1.0f),
                new SplitPane("Preview", previewComponent, 1.0f),
                SplitOrientation.Vertical);
        }
    }


    [MenuAction("launch", "global-menus/MenuTools/Registration Home")]
    //[ButtonAction("launch", "global-toolbars/MenuTools/Registration Home")]
    [Tooltip("launch", "Registration Home")]
    [IconSet("launch", IconScheme.Colour, "Icons.HomeToolSmall.png", "Icons.HomeToolMedium.png", "Icons.HomeToolLarge.png")]
    [ClickHandler("launch", "Launch")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class OldHomeTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {

        private IWorkspace _workspace;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OldHomeTool()
        {
        }


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

        public IApplicationComponent BuildComponent()
        {
            WorklistComponent resultComponent = new WorklistComponent();
            FoldersComponent foldersComponent = new FoldersComponent(resultComponent);
            PatientProfilePreviewComponent previewComponent = new PatientProfilePreviewComponent();

            resultComponent.SelectedPatientProfileChanged +=
                delegate(object sender, EventArgs e)
                {
                    previewComponent.PatientProfileRef = resultComponent.SelectedPatientProfile;
                };

            SplitComponentContainer container = new SplitComponentContainer(
                new SplitPane(SR.TitleResults, resultComponent, 0.45f),
                new SplitPane(SR.TitlePreview, previewComponent, 0.55f),
                SplitOrientation.Vertical);

            return new SplitComponentContainer(
                new SplitPane(SR.TitleSearch, foldersComponent, 0.15f),
                new SplitPane(SR.TitleResults, container, 0.85f),
                SplitOrientation.Vertical);
        }
    }
}
