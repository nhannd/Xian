using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Reporting.Folders
{
    [MenuAction("apply", "folderexplorer-folders-contextmenu/Refresh")]
    [ButtonAction("apply", "folderexplorer-folders-toolbar/Refresh")]
    [Tooltip("apply", "Refresh Folder")]
    [IconSet("apply", IconScheme.Colour, "Icons.RefreshToolSmall.png", "Icons.RefreshToolMedium.png", "Icons.RefreshToolLarge.png")]
    [ClickHandler("apply", "Refresh")]
    [ExtensionOf(typeof(ReportingWorkflowFolderToolExtensionPoint))]
    public class RefreshTool : Tool<IReportingWorkflowFolderToolContext>
    {
        public void Refresh()
        {
            this.Context.SelectedFolder.Refresh();
        }
    }
}
