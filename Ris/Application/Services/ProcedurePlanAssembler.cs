using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ProcedurePlanAssembler
    {
        public ProcedurePlanSummary CreateProcedurePlanSummary(Order order, IPersistenceContext context)
        {
            ProcedurePlanSummary summary = new ProcedurePlanSummary();

            RequestedProcedureAssembler assembler = new RequestedProcedureAssembler();

            summary.OrderRef = order.GetRef();
            summary.RequestedProcedures = CollectionUtils.Map<RequestedProcedure, RequestedProcedureDetail>(
                order.RequestedProcedures,
                delegate(RequestedProcedure rp) { return assembler.CreateRequestedProcedureDetail(rp, context); });
            summary.DiagnosticServiceSummary =
                new DiagnosticServiceSummary(order.DiagnosticService.GetRef(), order.DiagnosticService.Id, order.DiagnosticService.Name);

            return summary;
        }
    }
}
