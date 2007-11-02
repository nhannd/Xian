using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Application.Services
{
    public class DiagnosticServiceAssembler
    {
        public DiagnosticServiceSummary CreateDiagnosticServiceSummary(DiagnosticService diagnosticService)
        {
            return new DiagnosticServiceSummary(
                diagnosticService.GetRef(),
                diagnosticService.Id,
                diagnosticService.Name);
        }

        public DiagnosticServiceDetail CreateDiagnosticServiceDetail(DiagnosticService diagnosticService)
        {
            RequestedProcedureTypeAssembler rptAssembler = new RequestedProcedureTypeAssembler();
            return new DiagnosticServiceDetail(
                diagnosticService.GetRef(),
                diagnosticService.Id,
                diagnosticService.Name,
                CollectionUtils.Map<RequestedProcedureType, RequestedProcedureTypeDetail, List<RequestedProcedureTypeDetail>>(
                    diagnosticService.RequestedProcedureTypes,
                    delegate(RequestedProcedureType rpType)
                    {
                        return rptAssembler.CreateRequestedProcedureTypeDetail(rpType);
                    }));
        }
    }
}
