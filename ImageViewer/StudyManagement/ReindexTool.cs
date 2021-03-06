using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    [MenuAction("reindex", "global-menus/MenuTools/MenuReindex", "Reindex")]
    [ViewerActionPermissionAttribute("reindex", AuthorityTokens.Administration.ReIndex)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class ReindexTool : Tool<IDesktopToolContext>
    {
        public override IActionSet Actions
        {
            get
            {
                if (!WorkItemActivityMonitor.IsSupported)
                    return new ActionSet();

                return base.Actions;
            }
        }

        public void Reindex()
        {
            StartReindex(this.Context.DesktopWindow);
        }

        internal static void StartReindex(IDesktopWindow desktopWindow)
        {
            if (!WorkItemActivityMonitor.IsRunning)
            {
                desktopWindow.ShowMessageBox(SR.MessageReindexServiceNotRunning, MessageBoxActions.Ok);
                return;
            }

            if (!PermissionsHelper.IsInRole(AuthorityTokens.Administration.ReIndex))
            {
                desktopWindow.ShowMessageBox(SR.WarningReindexPermission, MessageBoxActions.Ok);
                return;
            }

            string linkText = SR.LinkOpenActivityMonitor;

            try
            {
                string message;
                var client = new ReindexFilestoreBridge();
                client.Reindex();

                if (client.WorkItem.Status == WorkItemStatusEnum.InProgress)
                    message = SR.MessageReindexInProgress;
                else if (client.WorkItem.Status == WorkItemStatusEnum.Idle)
                    message = SR.MessageReindexInProgress;
                else if (client.WorkItem.Status == WorkItemStatusEnum.Pending)
                    message = SR.MessageReindexScheduled;
                else
                    message = SR.MessageFailedToStartReindex;

				desktopWindow.ShowAlert(AlertLevel.Info, message, linkText, ActivityMonitorManager.Show, true);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.MessageFailedToStartReindex, desktopWindow);
            }
        }
    }
}
