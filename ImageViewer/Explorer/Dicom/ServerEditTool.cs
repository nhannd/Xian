using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [ButtonAction("activate", "dicomaenavigator-toolbar/Edit")]
    [MenuAction("activate", "dicomaenavigator-contextmenu/Edit")]
    [ClickHandler("activate", "EditServer")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "Edit")]
    [IconSet("activate", IconScheme.Colour, "Icons.Edit.png", "Icons.Edit.png", "Icons.Edit.png")]
    [ExtensionOf(typeof(AENavigatorToolExtensionPoint))]
    public class ServerEditTool : AENavigatorTool
    {
        public ServerEditTool()
        {
        }

        private void EditServer()
        {
            DicomServerTree _dicomServerTree = this.Context.DicomAEServerTree;
            this.Context.UpdateType = (int)ServerUpdateType.Edit;
            if (_dicomServerTree.CurrentServer.IsServer)
            {
                DicomServerEditComponent editor = new DicomServerEditComponent(_dicomServerTree);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, "Edit Server");
            }
            else
            {
                DicomServerGroupEditComponent editor = new DicomServerGroupEditComponent(_dicomServerTree, ServerUpdateType.Edit);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, "Edit Group");
            }
            return;
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
            this.Enabled = !this.Context.DicomAEServerTree.CurrentServer.ServerName.Equals(AENavigatorComponent.MyDatastoreTitle)
                            && !this.Context.DicomAEServerTree.CurrentServer.ServerName.Equals(AENavigatorComponent.MyServersTitle);
        }
    }
}
