using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerOverviewComponent : DHtmlComponent
	{
		private ExternalPractitionerDetail _externalPractitionerDetail;
		private readonly EntityRef _externalPractitionerRef;

		public ExternalPractitionerOverviewComponent(EntityRef externalPractitionerRef)
		{
			_externalPractitionerRef = externalPractitionerRef;
		}

		public override void Start()
		{
			Platform.GetService<IExternalPractitionerAdminService>(service =>
				{
					var response = service.LoadExternalPractitionerForEdit(new LoadExternalPractitionerForEditRequest(_externalPractitionerRef));
					_externalPractitionerDetail = response.PractitionerDetail;
				});

			SetUrl(WebResourcesSettings.Default.ExternalPractitionerOverviewPageUrl);
			base.Start();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _externalPractitionerDetail;
		}
	}
}
