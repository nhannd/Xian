using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("show", "global-menus/Admin/Patient/Find...")]
    [ButtonAction("show", "global-toolbars/PatientAdminToolbar/Find")]
    [Tooltip("show", "Find Patient")]
    [IconSet("show", IconScheme.Colour, "Icons.FindPatientMedium.png", "Icons.FindPatientMedium.png", "Icons.FindPatientMedium.png")]
    [ClickHandler("show", "Show")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class PatientSearchTool : DesktopTool
    {

        private PatientSearchComponent _searchComponent;

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Show()
        {
            if (_searchComponent == null)
            {
                _searchComponent = new PatientSearchComponent();

                ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
                    _searchComponent,
                    "Find Patient",
                    ShelfDisplayHint.DockLeft,
                    delegate(IApplicationComponent component) { _searchComponent = null; });
            }
        }

    }
}
