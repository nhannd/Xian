using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [ButtonAction("activate", "dicomaenavigator-toolbar/ToolbarAddServer")]
    [MenuAction("activate", "dicomaenavigator-contextmenu/MenuAddServer")]
    [ClickHandler("activate", "AddNewServer")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipAddServer")]
    [IconSet("activate", IconScheme.Colour, "Icons.Add.png", "Icons.Add.png", "Icons.Add.png")]
    [ExtensionOf(typeof(AENavigatorToolExtensionPoint))]
    public class ServerAddTool : AENavigatorTool
    {
        public ServerAddTool()
        {
        }

        private void AddNewServer()
        {
            ServerTree serverTree = this.Context.ServerTree;
            this.Context.UpdateType = (int)ServerUpdateType.Add;
            DicomServerEditComponent editor = new DicomServerEditComponent(serverTree);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, SR.TitleAddNewServer);
            return;
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
                this.Enabled = !this.Context.ServerTree.CurrentNode.IsServer;
        }
    }
}
