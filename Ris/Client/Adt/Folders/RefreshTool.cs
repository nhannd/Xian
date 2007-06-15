using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt.Folders
{
    [MenuAction("apply", "folderexplorer-folders-contextmenu/Refresh")]
    [ButtonAction("apply", "folderexplorer-folders-toolbar/Refresh")]
    [Tooltip("apply", "Refresh Folder")]
    [IconSet("apply", IconScheme.Colour, "Icons.RefreshToolSmall.png", "Icons.RefreshToolMedium.png", "Icons.RefreshToolLarge.png")]
    [ClickHandler("apply", "Refresh")]
    [ExtensionOf(typeof(RegistrationWorkflowFolderToolExtensionPoint))]
    [ExtensionOf(typeof(TechnologistWorkflowFolderToolExtensionPoint))]
    public class RefreshTool : Tool<IToolContext>
    {
        public void Refresh()
        {
            if (this.ContextBase is IRegistrationWorkflowFolderToolContext)
                ((IRegistrationWorkflowFolderToolContext)this.Context).SelectedFolder.Refresh();
            else if (this.ContextBase is ITechnologistWorkflowFolderToolContext)
                ((ITechnologistWorkflowFolderToolContext)this.Context).SelectedFolder.Refresh();
        }
    }
}
