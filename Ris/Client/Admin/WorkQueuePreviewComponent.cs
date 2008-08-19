using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
	public class WorkQueuePreviewComponent : DHtmlComponent
	{
		private WorkQueueItemSummary _workQueueItem;

		public WorkQueueItemSummary WorkQueueItem
		{
			get { return _workQueueItem; }
			set
			{
				_workQueueItem = value;
				this.SetUrl(WebResourcesSettings.Default.WorkQueuePreviewPageUrl);
			}
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _workQueueItem;
		}
	}
}
