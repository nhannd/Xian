using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;
using ClearCanvas.Workflow;
using Iesi.Collections;
using ClearCanvas.Ris.Application.Common;

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
            Order order = mps.RequestedProcedure.Order;
            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();

            GetProcedurePlanForWorklistItemResponse response = new GetProcedurePlanForWorklistItemResponse();
            response.ProcedurePlanSummary = assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);

            response.OrderExtendedProperties = new Dictionary<string, string>();
            foreach (string key in order.ExtendedProperties.Keys)
            {
                response.OrderExtendedProperties[key] = (string)order.ExtendedProperties[key];
            }

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
        public StartModalityProcedureStepsResponse StartModalityProcedureSteps(StartModalityProcedureStepsRequest request)
        {
            // load the set of mps
            List<ModalityProcedureStep> modalitySteps = CollectionUtils.Map<EntityRef, ModalityProcedureStep>(request.ModalityProcedureSteps,
                delegate(EntityRef mpsRef)
                {
                    return this.PersistenceContext.Load<ModalityProcedureStep>(mpsRef);
                });

            if (modalitySteps.Count == 0)
                throw new RequestValidationException("At least one procedure step is required.");

            // validate that each mps being started is being performed on the same modality
            if (!CollectionUtils.TrueForAll<ModalityProcedureStep>(modalitySteps,
                delegate(ModalityProcedureStep step) { return step.Modality.Equals(modalitySteps[0].Modality); }))
            {
                // TODO -better error message
                throw new RequestValidationException("Procedure steps cannot be started together because they are not on the same modality.");
            }

            // create an mpps
            ModalityPerformedProcedureStep mpps = new ModalityPerformedProcedureStep();
            this.PersistenceContext.Lock(mpps, DirtyState.New);

            foreach (ModalityProcedureStep mps in modalitySteps)
            {
                mps.Start(this.CurrentUserStaff);
                mps.AddPerformedStep(mpps);
            }

            this.PersistenceContext.SynchState();

            StartModalityProcedureStepsResponse response = new StartModalityProcedureStepsResponse();
            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();

            response.ProcedurePlanSummary = assembler.CreateProcedurePlanSummary(modalitySteps[0].RequestedProcedure.Order, this.PersistenceContext);
            response.StartedMpps = assembler.CreateModalityPerformedProcedureStepSummary(mpps, this.PersistenceContext);

            return response;
        }

        [UpdateOperation]
        public DiscontinueModalityProcedureStepsResponse DiscontinueModalityProcedureSteps(DiscontinueModalityProcedureStepsRequest request)
        {
            Order order = null;

            foreach (EntityRef mpsRef in request.ModalityProcedureSteps)
            {
                ModalityProcedureStep mps = this.PersistenceContext.Load<ModalityProcedureStep>(mpsRef);
                mps.Discontinue();

                if (order == null) order = mps.RequestedProcedure.Order;
            }

            //foreach (RequestedProcedureDetail rpDetail in request.RequestedProcedures)
            //{
            //    RequestedProcedure rp = this.PersistenceContext.Load<RequestedProcedure>(rpDetail.RequestedProcedureRef);
                
            //    //This logic should not be here -> need CancelOrDiscontinueOperation
            //    if (rp.Status == RequestedProcedureStatus.SC) 
            //        rp.Cancel();
            //    else 
            //        rp.Discontinue();

            //    if (order == null) order = rp.Order;
            //}

            this.PersistenceContext.SynchState();

            DiscontinueModalityProcedureStepsResponse response = new DiscontinueModalityProcedureStepsResponse();
            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();

            response.ProcedurePlanSummary = assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);

            return response;
        }

        [UpdateOperation]
        public StopModalityPerformedProcedureStepResponse StopModalityPerformedProcedureStep(StopModalityPerformedProcedureStepRequest request)
        {
            ModalityPerformedProcedureStep mpps = this.PersistenceContext.Load<ModalityPerformedProcedureStep>(request.MppsRef);

            // copy extended properties (should this be in an assembler?)
            foreach (KeyValuePair<string, string> pair in request.ExtendedProperties)
            {
                mpps.ExtendedProperties[pair.Key] = pair.Value;
            }

            mpps.Complete();

            // Drill back to order so we can refresh procedure plan
            ModalityProcedureStep oneMps = CollectionUtils.FirstElement<ModalityProcedureStep>(mpps.Activities);
            Order order = oneMps.RequestedProcedure.Order;

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

                if (allMpsComplete && !hasInterpretationStep)
                {
                    InterpretationStep ip = new InterpretationStep(rp);
                    this.PersistenceContext.Lock(ip, DirtyState.New);
                }
            }

            this.PersistenceContext.SynchState();

            StopModalityPerformedProcedureStepResponse response = new StopModalityPerformedProcedureStepResponse();
            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();

            response.ProcedurePlanSummary = assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);
            response.StoppedMpps = assembler.CreateModalityPerformedProcedureStepSummary(mpps, this.PersistenceContext);

            return response;
        }

        [UpdateOperation]
        public DiscontinueModalityPerformedProcedureStepResponse DiscontinueModalityPerformedProcedureStep(DiscontinueModalityPerformedProcedureStepRequest request)
        {
            ModalityPerformedProcedureStep mpps = this.PersistenceContext.Load<ModalityPerformedProcedureStep>(request.MppsRef);

            mpps.Discontinue();

            // Drill back to order so we can refresh procedure plan
            ModalityProcedureStep oneMps = CollectionUtils.FirstElement<ModalityProcedureStep>(mpps.Activities);
            this.PersistenceContext.SynchState();

            DiscontinueModalityPerformedProcedureStepResponse response = new DiscontinueModalityPerformedProcedureStepResponse();
            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();

            response.ProcedurePlanSummary = assembler.CreateProcedurePlanSummary(oneMps.RequestedProcedure.Order, this.PersistenceContext);
            response.DiscontinuedMpps = assembler.CreateModalityPerformedProcedureStepSummary(mpps, this.PersistenceContext);

            return response;
        }

        [UpdateOperation]
        public SaveDataResponse SaveData(SaveDataRequest request)
        {
            Order order = PersistenceContext.Load<Order>(request.OrderRef);
            foreach (KeyValuePair<string, string> pair in request.OrderExtendedProperties)
            {
                order.ExtendedProperties[pair.Key] = pair.Value;
            }

            return new SaveDataResponse();
        }

        #endregion
    }
}
