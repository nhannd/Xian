#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
using Iesi.Collections.Generic;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow.TechnologistDocumentation
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(ITechnologistDocumentationService))]
    class TechnologistDocumentationService : ApplicationServiceBase, ITechnologistDocumentationService
    {
        #region ITechnologistDocumentationService Members

        [ReadOperation]
        public GetProcedurePlanForWorklistItemResponse GetProcedurePlanForWorklistItem(GetProcedurePlanForWorklistItemRequest request)
        {
            ProcedureStep mps = this.PersistenceContext.Load<ProcedureStep>(request.ProcedureStepRef);
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

            ISet<PerformedStep> mppsSet = new HashedSet<PerformedStep>();
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

            // Create the Documentation Steps
            if (modalitySteps[0].RequestedProcedure.DocumentationProcedureStep == null)
            {
                foreach (RequestedProcedure orderRp in modalitySteps[0].RequestedProcedure.Order.RequestedProcedures)
                {
                    ProcedureStep docStep = new DocumentationProcedureStep(orderRp);
                    docStep.Start(this.CurrentUserStaff);
                }
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

            foreach (ModalityProcedureStep mps in mpps.Activities)
            {
                // Any MPS can have multiple MPPS's, so discontinue the MPS only if all MPPS's are discontinued
                if(mps.PerformedSteps.Count > 1)
                {
                    bool allMppsDiscontinued = CollectionUtils.TrueForAll<PerformedProcedureStep>(
                        mps.PerformedSteps,
                        delegate(PerformedProcedureStep pps)
                        {
                            return pps.State == PerformedStepStatus.DC;
                        });

                    if (allMppsDiscontinued) mps.Discontinue();
                }
                // In practice, most MPS only have a single MPPS
                else if(mps.PerformedSteps.Count == 1)
                {
                    mps.Discontinue();
                }
            }

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

            this.PersistenceContext.SynchState();

            SaveDataResponse response = new SaveDataResponse();
            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();
            response.ProcedurePlanSummary = assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);

            return response;
        }

        [UpdateOperation]
        public CompleteOrderDocumentationResponse CompleteOrderDocumentation(CompleteOrderDocumentationRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            foreach (RequestedProcedure requestedProcedure in order.RequestedProcedures)
            {
                if(requestedProcedure.DocumentationProcedureStep != null && requestedProcedure.DocumentationProcedureStep.State != ActivityStatus.CM)
                {
                    requestedProcedure.DocumentationProcedureStep.Complete();
                }
            }

            this.PersistenceContext.SynchState();

            CompleteOrderDocumentationResponse response = new CompleteOrderDocumentationResponse();
            TechnologistDocumentationAssembler assembler = new TechnologistDocumentationAssembler();
            response.ProcedurePlanSummary = assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);

            return response;
        }

        #endregion
    }
}
