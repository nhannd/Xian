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
	[ButtonAction("activate", "dicomaenavigator-toolbar/ToolbarEdit", "EditServer")]
	[MenuAction("activate", "dicomaenavigator-contextmenu/MenuEdit", "EditServer")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipEdit")]
    [IconSet("activate", IconScheme.Colour, "Icons.EditToolSmall.png", "Icons.EditToolMedium.png", "Icons.EditToolLarge.png")]
    [ExtensionOf(typeof(AENavigatorToolExtensionPoint))]
    public class EditServerTool : AENavigatorTool
    {
        public EditServerTool()
        {
        }

        public override void Initialize()
        {
            SetDoubleClickHandler();

            base.Initialize();
        }

        private void EditServer()
        {
            ServerTree serverTree = this.Context.ServerTree;
            this.Context.UpdateType = (int)ServerUpdateType.Edit;

            if (serverTree.CurrentNode.IsServer)
            {
                DicomServerEditComponent editor = new DicomServerEditComponent(serverTree);
				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, SR.TitleEditServer);
            }
            else
            {
                DicomServerGroupEditComponent editor = new DicomServerGroupEditComponent(serverTree, ServerUpdateType.Edit);
				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, SR.TitleEditServerGroup);
            }

			this.Context.UpdateType = (int)ServerUpdateType.None;
		}

        private void SetDoubleClickHandler()
        {
            this.Context.DefaultActionHandler = EditServer;
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
        	//enabled if it is a server or server group and is not the "My Servers" root node
			this.Enabled = (this.Context.ServerTree.CurrentNode.IsServer || this.Context.ServerTree.CurrentNode.IsServerGroup) &&
							this.Context.ServerTree.CurrentNode != this.Context.ServerTree.RootNode.ServerGroupNode;
        }
    }
}