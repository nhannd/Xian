using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("launch", "global-menus/MenuTools/MenuToolsMyTools/PatientRegistrationTool")]
    [ButtonAction("launch", "global-toolbars/ToolbarMyTools/PatientRegistrationTool")]
    [Tooltip("launch", "Patient Registration")]
    [IconSet("launch", IconScheme.Colour, "Icons.PatientRegistrationToolSmall.png", "Icons.PatientRegistrationToolMedium.png", "Icons.PatientRegistrationToolLarge.png")]
    [ClickHandler("launch", "Launch")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class PatientRegistrationTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {

        private IWorkspace _workspace;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PatientRegistrationTool()
        {
        }


        public void Launch()
        {
            if (_workspace == null)
            {
                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    BuildComponent(),
                    "Patient Registration",
                    delegate(IApplicationComponent c) { _workspace = null; });
            }
            else
            {
                _workspace.Activate();
            }
        }

        public IApplicationComponent BuildComponent()
        {
            PatientSearchComponent searchComponent = new PatientSearchComponent();
            PatientSearchResultComponent resultComponent = new PatientSearchResultComponent();
            PatientPreviewComponent previewComponent = new PatientPreviewComponent();

            searchComponent.SearchRequested +=
                delegate(object sender, PatientSearchRequestedEventArgs e)
                {
                    resultComponent.SearchCriteria = e.SearchCriteria;
                };

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
                new SplitPane("Search", searchComponent),
                new SplitPane("Results", container),
                SplitOrientation.Vertical);
        }
    }
}
