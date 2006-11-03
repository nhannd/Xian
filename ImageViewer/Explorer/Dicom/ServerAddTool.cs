using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [ButtonAction("activate", "dicomaenavigator-toolbar/AddServer")]
    [MenuAction("activate", "dicomaenavigator-contextmenu/Add New Server")]
    [ClickHandler("activate", "AddNewServer")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "Add New Server")]
    [IconSet("activate", IconScheme.Colour, "Icons.Add.png", "Icons.Add.png", "Icons.Add.png")]
    [ExtensionOf(typeof(AENavigatorToolExtensionPoint))]
    public class ServerAddTool : AENavigatorTool
    {
        public ServerAddTool()
        {
        }

        private void AddNewServer()
        {
            DicomServerTree _dicomServerTree = this.Context.DicomAEServerTree;
            this.Context.UpdateType = (int)ServerUpdateType.Add;
            DicomServerEditComponent editor = new DicomServerEditComponent(_dicomServerTree);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, "Add New Server");
            return;
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
                this.Enabled = !this.Context.DicomAEServerTree.CurrentServer.IsServer;
        }
    }
}
