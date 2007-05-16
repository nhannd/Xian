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
	[IconSet("activate", IconScheme.Colour, "Icons.AddServerToolSmall.png", "Icons.AddServerToolMedium.png", "Icons.AddServerToolLarge.png")]
    [ExtensionOf(typeof(AENavigatorToolExtensionPoint))]
    public class AddServerTool : AENavigatorTool
    {
        public AddServerTool()
        {
        }

        private void AddNewServer()
        {
            ServerTree serverTree = this.Context.ServerTree;
            this.Context.UpdateType = (int)ServerUpdateType.Add;
            DicomServerEditComponent editor = new DicomServerEditComponent(serverTree);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, SR.TitleAddNewServer);
			this.Context.UpdateType = (int)ServerUpdateType.None; 
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
			this.Enabled = !this.Context.ServerTree.CurrentNode.IsServer;
        }
    }
}
