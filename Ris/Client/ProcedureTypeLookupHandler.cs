using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin;

namespace ClearCanvas.Ris.Client
{
    public class ProcedureTypeLookupHandler : LookupHandler<TextQueryRequest, ProcedureTypeSummary>
    {
        private readonly DesktopWindow _desktopWindow;

        public ProcedureTypeLookupHandler(DesktopWindow desktopWindow)
			: base(ProcedureTypeLookupSettings.Default.MinQueryStringLength, ProcedureTypeLookupSettings.Default.QuerySpecificityThreshold)
        {
            _desktopWindow = desktopWindow;
        }

        protected override TextQueryResponse<ProcedureTypeSummary> DoQuery(TextQueryRequest request)
        {
            TextQueryResponse<ProcedureTypeSummary> response = null;
            Platform.GetService<IProcedureTypeAdminService>(
                delegate(IProcedureTypeAdminService service)
                {
                    response = service.TextQuery(request);
                });
            return response;
        }

        public override bool ResolveNameInteractive(string query, out ProcedureTypeSummary ProcedureType)
        {
			ProcedureType = null;

			ProcedureTypeSummaryComponent summaryComponent = new ProcedureTypeSummaryComponent(true);
			if (!string.IsNullOrEmpty(query))
			{
				summaryComponent.Name = query;
			}

			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				_desktopWindow, summaryComponent, "Procedure Types");

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				ProcedureType = (ProcedureTypeSummary)summaryComponent.SummarySelection.Item;
			}

			return (ProcedureType != null);
		}


        public override string FormatItem(ProcedureTypeSummary item)
        {
			return string.Format("{0} ({1})", item.Name, item.Id);
        }
    }
}
