using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Client
{
    public class ExternalPractitionerLookupHandler : LookupHandler<TextQueryRequest, ExternalPractitionerSummary>
    {
        private readonly DesktopWindow _desktopWindow;

        public ExternalPractitionerLookupHandler(DesktopWindow desktopWindow)
        {
            _desktopWindow = desktopWindow;
        }

        protected override TextQueryResponse<ExternalPractitionerSummary> DoQuery(TextQueryRequest request)
        {
            TextQueryResponse<ExternalPractitionerSummary> response = null;
            Platform.GetService<IExternalPractitionerAdminService>(
                delegate(IExternalPractitionerAdminService service)
                {
                    response = service.TextQuery(request);
                });
            return response;
        }
        
        public override bool ResolveNameInteractive(string query, out ExternalPractitionerSummary result)
        {
            result = null;

            ExternalPractitionerSummaryComponent component = new ExternalPractitionerSummaryComponent(true);
            if (!string.IsNullOrEmpty(query))
            {
                string[] names = query.Split(',');
                if (names.Length > 0)
                    component.LastName = names[0].Trim();
                if (names.Length > 1)
                    component.FirstName = names[1].Trim();
            }

            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                _desktopWindow, component, SR.TitleExternalPractitioner);

            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                result = (ExternalPractitionerSummary)component.SelectedPractitioner.Item;
            }

            return (result != null);
        }

        public override string FormatItem(ExternalPractitionerSummary item)
        {
            return string.Format("{0}, {1}", item.Name.FamilyName, item.Name.GivenName);
        }
    }
}
