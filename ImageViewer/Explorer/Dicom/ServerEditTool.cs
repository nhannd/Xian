using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [ButtonAction("activate", "dicomaenavigator-toolbar/ToolbarEdit")]
    [MenuAction("activate", "dicomaenavigator-contextmenu/MenuEdit")]
    [ClickHandler("activate", "EditServer")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipEdit")]
    [IconSet("activate", IconScheme.Colour, "Icons.Edit.png", "Icons.Edit.png", "Icons.Edit.png")]
    [ExtensionOf(typeof(AENavigatorToolExtensionPoint))]
    public class ServerEditTool : AENavigatorTool
    {
        public ServerEditTool()
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
            return;
        }

        private void SetDoubleClickHandler()
        {
            this.Context.DefaultActionHandler = EditServer;
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
            this.Enabled = !this.Context.ServerTree.CurrentNode.Name.Equals(AENavigatorComponent.MyDatastoreTitle)
                            && !this.Context.ServerTree.CurrentNode.Name.Equals(AENavigatorComponent.MyServersTitle);
        }
    }
}