using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class RequestedProcedureAssembler
    {
        public RequestedProcedureDetail CreateRequestedProcedureDetail(RequestedProcedure rp, IPersistenceContext context)
        {
            RequestedProcedureDetail detail = new RequestedProcedureDetail();

            ModalityProcedureStepAssembler modalityProcedureStepAssembler = new ModalityProcedureStepAssembler();
            ProtocolProcedureStepAssembler protocolProcedureStepAssembler = new ProtocolProcedureStepAssembler();

            detail.RequestedProcedureRef = rp.GetRef();
            detail.Name = rp.Type.Name;
            detail.Status = EnumUtils.GetEnumValueInfo(rp.Status, context);
            detail.ModalityProcedureSteps = CollectionUtils.Map<ModalityProcedureStep, ModalityProcedureStepDetail>(
                rp.ModalityProcedureSteps,
                delegate(ModalityProcedureStep mp) { return modalityProcedureStepAssembler.CreateModalityProcedureStepDetail(mp, context); });
            detail.ProtocolProcedureStepDetail = rp.ProtocolProcedureStep != null
                ? protocolProcedureStepAssembler.CreateProtocolProcedureStepDetail(rp.ProtocolProcedureStep, context)
                : null;

            return detail;
        }

    }
}
