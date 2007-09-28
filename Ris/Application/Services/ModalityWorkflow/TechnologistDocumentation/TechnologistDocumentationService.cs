using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;
using ClearCanvas.Workflow;
using Iesi.Collections;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow.TechnologistDocumentation
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(ITechnologistDocumentationService))]
    class TechnologistDocumentationService : ApplicationServiceBase, ITechnologistDocumentationService
    {
        #region Old Tech Documentation page services

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
                            return ps.Is<ModalityProcedureStep>();
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

        #endregion

        #region ITechnologistDocumentationService Members

        [ReadOperation]
        public GetProcedurePlanForWorklistItemResponse GetProcedurePlanForWorklistItem(GetProcedurePlanForWorklistItemRequest request)
        {
            ModalityProcedureStep mps = this.PersistenceContext.Load<ModalityProcedureStep>(request.ProcedureStepRef);

            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();

            GetProcedurePlanForWorklistItemResponse response = new GetProcedurePlanForWorklistItemResponse();

            Order order = mps.RequestedProcedure.Order;
            response.OrderRef = order.GetRef();
            
            response.RequestedProcedures = CollectionUtils.Map<RequestedProcedure, RequestedProcedureDetail>(
                order.RequestedProcedures,
                delegate(RequestedProcedure rp) { return assembler.CreateRequestedProcedureDetail(rp, this.PersistenceContext);});

            return response;
        }

        [ReadOperation]
        public ListPerformedProcedureStepsResponse ListPerformedProcedureSteps(ListPerformedProcedureStepsRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();

            ISet mppsSet = new HybridSet();
            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                foreach (ModalityProcedureStep mps in rp.ModalityProcedureSteps)
                {
                    mppsSet.AddAll(mps.PerformedSteps);
                }
            }

            ListPerformedProcedureStepsResponse response = new ListPerformedProcedureStepsResponse();
            response.PerformedProcedureSteps = CollectionUtils.Map<ModalityPerformedProcedureStep, ModalityPerformedProcedureStepSummary>(
                mppsSet,
                delegate (ModalityPerformedProcedureStep mpps) { return assembler.CreateModalityPerformedProcedureStepSummary(mpps, this.PersistenceContext);}); 

            return response;
        }

        [UpdateOperation]
        public StartModalityProcedureStepResponse StartModalityProcedureStep(StartModalityProcedureStepRequest request)
        {
            ModalityPerformedProcedureStep mpps = new ModalityPerformedProcedureStep();
            this.PersistenceContext.Lock(mpps, DirtyState.New);

            Order order = null;

            foreach (ModalityProcedureStepDetail mpsDetail in request.ModalityProcedureSteps)
            {
                ModalityProcedureStep mps = this.PersistenceContext.Load<ModalityProcedureStep>(mpsDetail.ModalityProcedureStepRef);
                mps.Start(this.CurrentUserStaff);
                mps.AddPerformedStep(mpps);

                if (order == null) order = mps.RequestedProcedure.Order;
            }

            this.PersistenceContext.SynchState();

            StartModalityProcedureStepResponse response = new StartModalityProcedureStepResponse();
            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();

            response.RequestedProcedures = CollectionUtils.Map<RequestedProcedure, RequestedProcedureDetail>(
                order.RequestedProcedures,
                delegate(RequestedProcedure rp) { return assembler.CreateRequestedProcedureDetail(rp, this.PersistenceContext); });

            response.ModalityPerformedProcedureStep = assembler.CreateModalityPerformedProcedureStepSummary(mpps, this.PersistenceContext);

            return response;
        }

        [UpdateOperation]
        public StopModalityPerformedProcedureStepResponse StopModalityPerformedProcedureStep(StopModalityPerformedProcedureStepRequest request)
        {
            ModalityPerformedProcedureStep mpps = this.PersistenceContext.Load<ModalityPerformedProcedureStep>(request.MppsRef);

            mpps.Complete();

            // Drill back to order so we can refresh procedure plan
            ModalityProcedureStep oneMps = CollectionUtils.FirstElement<ModalityProcedureStep>(mpps.Activities);
            Order order = oneMps.RequestedProcedure.Order;
            this.PersistenceContext.SynchState();

            StopModalityPerformedProcedureStepResponse response = new StopModalityPerformedProcedureStepResponse();
            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();

            response.StoppedMpps = assembler.CreateModalityPerformedProcedureStepSummary(mpps, this.PersistenceContext);
            response.RequestedProcedures = CollectionUtils.Map<RequestedProcedure, RequestedProcedureDetail>(
                order.RequestedProcedures,
                delegate(RequestedProcedure rp) { return assembler.CreateRequestedProcedureDetail(rp, this.PersistenceContext); });

            return response;
        }

        [UpdateOperation]
        public DiscontinueModalityPerformedProcedureStepResponse DiscontinueModalityPerformedProcedureStep(DiscontinueModalityPerformedProcedureStepRequest request)
        {
            ModalityPerformedProcedureStep mpps = this.PersistenceContext.Load<ModalityPerformedProcedureStep>(request.MppsRef);

            mpps.Discontinue();

            // Drill back to order so we can refresh procedure plan
            ModalityProcedureStep oneMps = CollectionUtils.FirstElement<ModalityProcedureStep>(mpps.Activities);
            Order order = oneMps.RequestedProcedure.Order;
            this.PersistenceContext.SynchState();

            DiscontinueModalityPerformedProcedureStepResponse response = new DiscontinueModalityPerformedProcedureStepResponse();
            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();

            response.DiscontinuedMpps = assembler.CreateModalityPerformedProcedureStepSummary(mpps, this.PersistenceContext);
            response.RequestedProcedures = CollectionUtils.Map<RequestedProcedure, RequestedProcedureDetail>(
                order.RequestedProcedures,
                delegate(RequestedProcedure rp) { return assembler.CreateRequestedProcedureDetail(rp, this.PersistenceContext); });

            return response;
        }

        [UpdateOperation]
        public CompleteModalityProcedureStepsResponse CompleteModalityProcedureSteps(CompleteModalityProcedureStepsRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                bool allMpsComplete = true;

                foreach (ModalityProcedureStep mps in rp.ModalityProcedureSteps)
                {
                    // TODO: "Completer" can be different?
                    mps.TryCompleteFromPerformedProcedureSteps();
                    allMpsComplete &= mps.State == ActivityStatus.CM;
                }

                bool hasInterpretationStep = CollectionUtils.Contains<ProcedureStep>(
                    rp.ProcedureSteps,
                    delegate(ProcedureStep ps) { return ps.Is<InterpretationStep>(); });

                if(allMpsComplete && !hasInterpretationStep)
                {
                    InterpretationStep ip = new InterpretationStep(rp);
                    this.PersistenceContext.Lock(ip, DirtyState.New);
                }
            }

            this.PersistenceContext.SynchState();

            CompleteModalityProcedureStepsResponse response = new CompleteModalityProcedureStepsResponse();
            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();

            response.RequestedProcedures = CollectionUtils.Map<RequestedProcedure, RequestedProcedureDetail>(
                order.RequestedProcedures,
                delegate(RequestedProcedure rp) { return assembler.CreateRequestedProcedureDetail(rp, this.PersistenceContext); });

            return response;
        }

        [UpdateOperation]
        public DiscontinueRequestedProcedureOrModalityProcedureStepResponse DiscontinueRequestedProcedureOrModalityProcedureStep(DiscontinueRequestedProcedureOrModalityProcedureStepRequest request)
        {
            Order order = null;

            foreach (ModalityProcedureStepDetail mpsDetail in request.ModalityProcedureSteps)
            {
                ModalityProcedureStep mps = this.PersistenceContext.Load<ModalityProcedureStep>(mpsDetail.ModalityProcedureStepRef);
                mps.Discontinue();

                if (order == null) order = mps.RequestedProcedure.Order;
            }

            foreach (RequestedProcedureDetail rpDetail in request.RequestedProcedures)
            {
                RequestedProcedure rp = this.PersistenceContext.Load<RequestedProcedure>(rpDetail.RequestedProcedureRef);
                
                //This logic should not be here -> need CancelOrDiscontinueOperation
                if (rp.Status == RequestedProcedureStatus.SC) 
                    rp.Cancel();
                else 
                    rp.Discontinue();

                if (order == null) order = rp.Order;
            }

            this.PersistenceContext.SynchState();

            DiscontinueRequestedProcedureOrModalityProcedureStepResponse response = new DiscontinueRequestedProcedureOrModalityProcedureStepResponse();
            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();

            response.RequestedProcedures = CollectionUtils.Map<RequestedProcedure, RequestedProcedureDetail>(
                order.RequestedProcedures,
                delegate(RequestedProcedure rp) { return assembler.CreateRequestedProcedureDetail(rp, this.PersistenceContext); });

            return response;
        }

        #endregion
    }
}
