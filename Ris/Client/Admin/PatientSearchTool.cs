using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Admin
{
    [ExtensionPoint()]
    public class PatientSearchToolViewExtensionPoint : ExtensionPoint<IToolView>
    {
    }

    [ToolView(typeof(PatientSearchToolViewExtensionPoint), "Find Patient", ToolViewDisplayHint.DockLeft, "IsViewActive", "ViewActivationChanged")]

    [ExtensionOf(typeof(PatientAdminToolExtensionPoint))]
    public class PatientSearchTool : Tool
    {
        public bool IsViewActive
        {
            get { return true; }
            set { }
        }

        public event EventHandler ViewActivationChanged
        {
            add { }
            remove { }
        }

        public void Search()
        {
            PatientAdminComponent component = ((PatientAdminToolContext)this.Context).Component;
            component.SetCriteria();
        }
    }
}
