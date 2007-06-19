using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;
using Iesi.Collections;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow.TechnologistDocumentation
{
    internal class TechnologistDocumentationAssembler
    {
        public ProcedureStepDetail CreateProcedureStepDetail(ProcedureStep ps, IPersistenceContext context)
        {
            ProcedureStepDetail detail = new ProcedureStepDetail();
            detail.Name = ps.Name;
            //detail.Status = ps.State;
            //detail.DocumentationPageUrl = "about:blank";
            detail.PerformedProcedureStep = CreatePerformedProcedureStepDetail(ps.PerformedSteps);
            return detail;
        }

        private PerformedProcedureStepDetail CreatePerformedProcedureStepDetail(ISet steps)
        {
            if (steps == null || steps.IsEmpty) return null;

            PerformedProcedureStep pps = (PerformedProcedureStep)CollectionUtils.FirstElement(steps);
            PerformedProcedureStepDetail detail = new PerformedProcedureStepDetail();
            detail.PpsRef = pps.GetRef();
            detail.StartTime = pps.StartTime;
            detail.EndTime = pps.EndTime;

            return detail;
        }
    }
}