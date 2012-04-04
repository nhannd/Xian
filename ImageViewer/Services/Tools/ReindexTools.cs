using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Services.Tools
{
    [MenuAction("reindex", "global-menus/MenuTools/MenuLocalServer/MenuReindex", "Reindex")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class ReindexTools : Tool<IDesktopToolContext>
    {
        /// TODO (CR Apr 2012): Move to ImageViewer
        public void Reindex()
        {
            if (!WorkItemActivityMonitor.IsRunning)
            {
                Context.DesktopWindow.ShowMessageBox(ImageViewer.SR.MessageReindexServiceNotRunning, MessageBoxActions.Ok);
                return;
            }

            var dialog = new TimedDialog(ShowActivityMonitor);
            dialog.LinkText = SR.LinkOpenActivityMonitor;

            try
            {
                var client = new ReindexClient();
                if (client.Reindex())
                {
                    if (client.Request.Status == WorkItemStatusEnum.InProgress)
                        dialog.Message = ImageViewer.SR.MessageReindexInProgress;
                    else if (client.Request.Status == WorkItemStatusEnum.Idle)
                        dialog.Message = ImageViewer.SR.MessageReindexInProgress;
                    else if (client.Request.Status == WorkItemStatusEnum.Pending)
                        dialog.Message = ImageViewer.SR.MessageReindexScheduled;
                }
                else
                {
                    dialog.Message = ImageViewer.SR.MessageFailedToStartReindex;
                }

                dialog.Show();
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, ImageViewer.SR.MessageFailedToStartReindex, Context.DesktopWindow);
            }
        }

        private void ShowActivityMonitor()
        {
            ActivityMonitorWorkspace.Show(Context.DesktopWindow);
        }
    }
}
