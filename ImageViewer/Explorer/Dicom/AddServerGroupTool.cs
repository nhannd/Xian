using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomaenavigator-toolbar/ToolbarAddServerGroup", "AddNewServerGroup")]
	[MenuAction("activate", "dicomaenavigator-contextmenu/MenuAddServerGroup", "AddNewServerGroup")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipAddServerGroup")]
	[IconSet("activate", IconScheme.Colour, "Icons.AddServerGroupToolSmall.png", "Icons.AddServerGroupToolMedium.png", "Icons.AddServerGroupToolLarge.png")]
    [ExtensionOf(typeof(AENavigatorToolExtensionPoint))]
    public class AddServerGroupTool : AENavigatorTool
    {
        public AddServerGroupTool()
        {
        }

        private void AddNewServerGroup()
        {
            ServerTree _serverTree = this.Context.ServerTree;
            this.Context.UpdateType = (int)ServerUpdateType.Add;
            DicomServerGroupEditComponent editor = new DicomServerGroupEditComponent(_serverTree, ServerUpdateType.Add);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, SR.TitleAddServerGroup);
			this.Context.UpdateType = (int)ServerUpdateType.None; 
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
			this.Enabled = this.Context.ServerTree.CurrentNode.IsServerGroup;
        }
    }
}
