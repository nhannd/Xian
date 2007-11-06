using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ProtocolProcedureStepAssembler
    {
        public ProtocolProcedureStepDetail CreateProtocolProcedureStepDetail(ProtocolProcedureStep step, IPersistenceContext context)
        {
            ProtocolProcedureStepDetail detail = new ProtocolProcedureStepDetail();

            detail.ProtocolProcedureStepRef = step.GetRef();
            detail.Status = EnumUtils.GetEnumValueInfo(step.State, context);
            detail.ProtocolRef = step.Protocol.GetRef();

            return detail;
        }
    }
}
