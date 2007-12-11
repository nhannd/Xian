using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ModalityProcedureStepAssembler
    {
        public ModalityProcedureStepSummary CreateModalityProcedureStepSummary(ModalityProcedureStep mp, IPersistenceContext context)
        {
            return new ModalityProcedureStepSummary(mp.GetRef(),
                mp.Name,
                EnumUtils.GetEnumValueInfo(mp.State, context),
                mp.StartTime,
                mp.EndTime,
                mp.Modality.Id,
                mp.Modality.Name
                );
        }

        public ModalityProcedureStepDetail CreateModalityProcedureStepDetail(ModalityProcedureStep mp, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();

            return new ModalityProcedureStepDetail(
                mp.GetRef(),
                mp.Name,
                EnumUtils.GetEnumValueInfo(mp.State, context),
                mp.Scheduling == null ? null : mp.Scheduling.StartTime,
                mp.StartTime,
                mp.EndTime,
                mp.AssignedStaff == null ? null : staffAssembler.CreateStaffSummary(mp.AssignedStaff, context),
                mp.PerformingStaff == null ? null : staffAssembler.CreateStaffSummary(mp.PerformingStaff, context),
                mp.Modality.Id,
                mp.Modality.Name);
        }
    }
}
