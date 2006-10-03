using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("launch", "global-menus/View/Home")]
    [ButtonAction("launch", "global-toolbars/View/Home")]
    [Tooltip("launch", "Home")]
    [IconSet("launch", IconScheme.Colour, "Icons.PatientRegistrationToolSmall.png", "Icons.PatientRegistrationToolMedium.png", "Icons.PatientRegistrationToolLarge.png")]
    [ClickHandler("launch", "Launch")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class HomeTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {

        private IWorkspace _workspace;

        /// <summary>
        /// Default constructor
        /// </summary>
        public HomeTool()
        {
        }


        public void Launch()
        {
            if (_workspace == null)
            {
                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    BuildComponent(),
                    "Home",
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
            PatientPreviewComponent previewComponent = new PatientPreviewComponent();

            resultComponent.SelectedPatientChanged +=
                delegate(object sender, EventArgs e)
                {
                    previewComponent.Subject = resultComponent.SelectedPatient;
                };

            SplitComponentContainer container = new SplitComponentContainer(
                new SplitPane("Results", resultComponent),
                new SplitPane("Preview", previewComponent),
                SplitOrientation.Vertical);

            return new SplitComponentContainer(
                new SplitPane("Search", foldersComponent),
                new SplitPane("Results", container),
                SplitOrientation.Vertical);
        }
    }
}
