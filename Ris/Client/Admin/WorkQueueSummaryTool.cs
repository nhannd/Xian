using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
	[MenuAction("launch", "global-menus/Admin/Work Queue", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Management.WorkQueue)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	class WorkQueueSummaryTool : Tool<IDesktopToolContext>
	{
		private IWorkspace _workspace;

		public void Launch()
		{
			if (_workspace == null)
			{
				try
				{
					WorkQueueSummaryComponent summaryComponent = new WorkQueueSummaryComponent();
					WorkQueuePreviewComponent previewComponent = new WorkQueuePreviewComponent();

					SplitComponentContainer splitComponent = new SplitComponentContainer(SplitOrientation.Vertical);
					splitComponent.Pane1 = new SplitPane("Summary", summaryComponent, 1.0f);
					splitComponent.Pane2 = new SplitPane("Preview", previewComponent, 1.0f);

					summaryComponent.SummarySelectionChanged += delegate
						{
							previewComponent.WorkQueueItem = (WorkQueueItemSummary) summaryComponent.SummarySelection.Item;
						};

					_workspace = ApplicationComponent.LaunchAsWorkspace(
						this.Context.DesktopWindow,
						splitComponent,
						SR.TitleWorkQueue);
					_workspace.Closed += delegate { _workspace = null; };
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, this.Context.DesktopWindow);
				}
			}
			else
			{
				_workspace.Activate();
			}
		}
	}
}
