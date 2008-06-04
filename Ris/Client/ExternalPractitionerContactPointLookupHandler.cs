using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
    public class ExternalPractitionerContactPointLookupHandler : LookupHandler<TextQueryRequest, ExternalPractitionerContactPointSummary>
    {
    	private readonly EntityRef _practitionerRef;
        private readonly DesktopWindow _desktopWindow;

		public ExternalPractitionerContactPointLookupHandler(EntityRef practitionerRef, DesktopWindow desktopWindow)
        {
			_practitionerRef = practitionerRef;
            _desktopWindow = desktopWindow;
        }

		protected override TextQueryResponse<ExternalPractitionerContactPointSummary> DoQuery(TextQueryRequest request)
        {
			TextQueryResponse<ExternalPractitionerContactPointSummary> response = null;
            Platform.GetService<IExternalPractitionerAdminService>(
                delegate(IExternalPractitionerAdminService service)
                {
                    response = service.ContactPointTextQuery(new ContactPointTextQueryRequest(_practitionerRef, request));
                });
            return response;
        }

		public override bool ResolveNameInteractive(string query, out ExternalPractitionerContactPointSummary result)
        {
            result = null;

			ExternalPractitionerDetail practitionerDetail = null;
			Platform.GetService<IExternalPractitionerAdminService>(
				delegate(IExternalPractitionerAdminService service)
				{
					LoadExternalPractitionerForEditResponse response = service.LoadExternalPractitionerForEdit(new LoadExternalPractitionerForEditRequest(_practitionerRef));
					practitionerDetail = response.PractitionerDetail;
				});

			ExternalPractitionerContactPointSummaryComponent component = new ExternalPractitionerContactPointSummaryComponent(_practitionerRef);

			practitionerDetail.ContactPoints.ForEach(delegate(ExternalPractitionerContactPointDetail p)
													  {
														  component.Subject.Add(p);
													  });
			
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				_desktopWindow, component, SR.TitleContactPoints);

            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
				ExternalPractitionerContactPointDetail detail = (ExternalPractitionerContactPointDetail) component.SelectedContactPoint.Item;
				result = new ExternalPractitionerContactPointSummary(detail.ContactPointRef, detail.Name, detail.Description, detail.IsDefaultContactPoint);
            }

            return (result != null);
        }

		public override string FormatItem(ExternalPractitionerContactPointSummary item)
        {
            return string.Format("{0}, {1}", item.Name, item.Description);
        }
    }
}
