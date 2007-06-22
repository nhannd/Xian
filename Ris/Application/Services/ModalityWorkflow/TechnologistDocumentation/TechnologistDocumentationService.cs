using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
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
            ProcedureStep mps = PersistenceContext.Load<ProcedureStep>(request.WorklistItem.ProcedureStepRef);
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

        [UpdateOperation]
        public DocumentProceduresResponse DocumentProcedures(DocumentProceduresRequest request)
        {
            IList<ProcedureStepDetail> dirtyProcedureStepDetails =
                CollectionUtils.Select<ProcedureStepDetail, List<ProcedureStepDetail>>(
                    request.ProcedureSteps,
                    delegate(ProcedureStepDetail detail) { return detail.Dirty; });

            IDictionary<int, EntityRef> ppsDictionary = new Dictionary<int, EntityRef>(); 

            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();
            foreach (ProcedureStepDetail detail in dirtyProcedureStepDetails)
            {
                ProcedureStep procedureStep = PersistenceContext.Load<ProcedureStep>(detail.EntityRef);
                assembler.UpdateProcedureStep(procedureStep, detail, this.CurrentUserStaff, ppsDictionary, PersistenceContext);
            }

            return new DocumentProceduresResponse();
        }
    }
}
