using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.RequestedProcedureTypeGroupAdmin
{
    internal class RequestedProcedureTypeAssembler
    {
        internal RequestedProcedureTypeSummary GetRequestedProcedureTypeSummary(RequestedProcedureType rpt)
        {
            return new RequestedProcedureTypeSummary(rpt.GetRef(), rpt.Name, rpt.Id);
        }
    }
}
