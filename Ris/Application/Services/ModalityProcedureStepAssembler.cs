using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ModalityProcedureStepAssembler
    {
        public ModalityProcedureStepDetail CreateModalityProcedureStepDetail(ModalityProcedureStep mp, IPersistenceContext context)
        {
            ModalityProcedureStepDetail detail = new ModalityProcedureStepDetail();
            detail.Name = mp.Name;
            detail.ModalityProcedureStepRef = mp.GetRef();
            detail.StartDateTime = mp.StartTime;
            detail.EndDateTime = mp.EndTime;
            detail.Status = EnumUtils.GetEnumValueInfo(mp.State, context);
            detail.ModalityId = mp.Modality.Id;
            detail.ModalityName = mp.Modality.Name;
            return detail;
        }
    }
}
