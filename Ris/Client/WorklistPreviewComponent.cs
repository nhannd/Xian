using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Client
{
	public class WorklistPreviewComponent : DHtmlComponent
	{
		private readonly WorklistAdminDetail _worklist;

		public WorklistPreviewComponent(WorklistAdminDetail worklist)
		{
			_worklist = worklist;
		}

		public override void Start()
		{
			this.SetUrl(WebResourcesSettings.Default.WorklistPreviewPageUrl);
			base.Start();
		}

		public void Refresh()
		{
			NotifyAllPropertiesChanged();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _worklist;
		}
	}
}
