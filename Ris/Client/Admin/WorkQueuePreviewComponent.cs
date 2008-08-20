using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
	public class WorkQueuePreviewComponent : DHtmlComponent
	{
		private WorkQueueItemDetail _workQueueItem;

		public WorkQueueItemSummary WorkQueueItem
		{
			set
			{
				if (value == null)
				{
					_workQueueItem = null;
				}
				else
				{
					try
					{
						Platform.GetService<IWorkQueueAdminService>(
							delegate(IWorkQueueAdminService service)
								{
									LoadWorkQueueItemForEditResponse response = service.LoadWorkQueueItemForEdit(new LoadWorkQueueItemForEditRequest(value.WorkQueueItemRef));
									_workQueueItem = response.WorkQueueItemDetail;
								});
					}
					catch (Exception e)
					{
						_workQueueItem = null;
						ExceptionHandler.Report(e, this.Host.DesktopWindow);
					}
				}

				this.SetUrl(WebResourcesSettings.Default.WorkQueuePreviewPageUrl);
			}
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _workQueueItem;
		}
	}
}
