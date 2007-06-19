using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow.TechnologistDocumentation
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(ITechnologistDocumentationService))]
    class TechnologistDocumentationService : ApplicationServiceBase, ITechnologistDocumentationService
    {
        [ReadOperation]
        public GetProcedureStepsForWorklistItemResponse GetProcedureStepsForWorklistItem(
            GetProcedureStepsForWorklistItemRequest request)
        {
            ProcedureStep mps = (ProcedureStep)PersistenceContext.Load(request.WorklistItem.ProcedureStepRef);
            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();

            IList<ProcedureStep> documentableProcedureSteps =
                CollectionUtils.Select<ProcedureStep, List<ProcedureStep>>(
                    mps.RequestedProcedure.ProcedureSteps,
                    delegate(ProcedureStep ps)
                    {
                        return ps is ModalityProcedureStep;
                    });

            return new GetProcedureStepsForWorklistItemResponse(
                CollectionUtils.Map<ProcedureStep, ProcedureStepDetail, List<ProcedureStepDetail>>(
                    documentableProcedureSteps,
                    delegate (ProcedureStep ps)
                    {
                        return assembler.CreateProcedureStepDetail(ps, PersistenceContext);
                    }));
        }
    }
}
