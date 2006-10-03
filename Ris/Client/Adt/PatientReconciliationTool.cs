using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    //[MenuAction("apply", "global-menus/Patient/Reconcile...")]
    [MenuAction("apply", "patientpreview-menu/Reconcile")]
    [MenuAction("doSomething", "patientpreview-menu/DoSomething")]
    //[ButtonAction("apply", "global-toolbars/Patient/PatientReconciliationTool")]
    //[Tooltip("apply", "Place tooltip text here")]
    //[IconSet("apply", IconScheme.Colour, "Icons.PatientReconciliationToolSmall.png", "Icons.PatientReconciliationToolMedium.png", "Icons.PatientReconciliationToolLarge.png")]
    [ClickHandler("apply", "Apply")]
    [ClickHandler("doSomething", "DoSomething")]

    [ExtensionOf(typeof(PatientPreviewToolExtensionPoint))]
    public class PatientReconciliationTool : Tool<IPatientPreviewToolContext>
    {
        private PatientReconciliationComponent _component;

        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public PatientReconciliationTool()
        {
        }

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            if (_component == null)
            {
                _component = new PatientReconciliationComponent();
                ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    _component,
                    "Patient Reconciliation",
                    delegate(IApplicationComponent c) { _component = null; });
            }
        }

        public void DoSomething()
        {
            Platform.ShowMessageBox("Something!");
        }
    }
}
