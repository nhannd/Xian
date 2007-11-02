using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
    public class DiagnosticServiceLookupHandler : LookupHandler<TextQueryRequest, DiagnosticServiceSummary>
    {
        private readonly DesktopWindow _desktopWindow;

        public DiagnosticServiceLookupHandler(DesktopWindow desktopWindow)
        {
            _desktopWindow = desktopWindow;
        }

        protected override TextQueryResponse<DiagnosticServiceSummary> DoQuery(TextQueryRequest request)
        {
            TextQueryResponse<DiagnosticServiceSummary> response = null;
            Platform.GetService<IDiagnosticServiceAdminService>(
                delegate(IDiagnosticServiceAdminService service)
                {
                    response = service.TextQuery(request);
                });
            return response;
        }

        public override bool ResolveNameInteractive(string query, out DiagnosticServiceSummary diagnosticService)
        {
            DiagnosticServiceDetail detail;
            bool success = ResolveNameInteractive(query, out detail);
            diagnosticService = success ? detail.GetSummary() : null;
            return success;
        }

        public bool ResolveNameInteractive(string query, out DiagnosticServiceDetail diagnosticService)
        {
            diagnosticService = null;

            // cannot make use of the query string in any meaningful way, since we don't download the entire tree
            // to the client

            DiagnosticServiceTreeComponent component = new DiagnosticServiceTreeComponent();
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                _desktopWindow, component, SR.TitleDiagnosticServices);

            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                diagnosticService = component.SelectedDiagnosticServiceDetail;
            }

            return (diagnosticService != null);
        }

        public override string FormatItem(DiagnosticServiceSummary item)
        {
            return item.Name;
        }
    }
}
