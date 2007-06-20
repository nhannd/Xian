using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;
using ClearCanvas.Workflow;
using ClearCanvas.Workflow.Brokers;
using Iesi.Collections;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow.TechnologistDocumentation
{
    internal class TechnologistDocumentationAssembler
    {
        public ProcedureStepDetail CreateProcedureStepDetail(ProcedureStep ps, IPersistenceContext context)
        {
            ProcedureStepDetail detail = new ProcedureStepDetail();
            detail.EntityRef = ps.GetRef();
            detail.Name = ps.Name;

            ActivityStatusEnumTable activityStatusEnumTable = context.GetBroker<IActivityStatusEnumBroker>().Load();
            detail.Status = activityStatusEnumTable[ps.State].Value;

            detail.DocumentationPage = new DocumentationPageDetail("http://localhost/RIS/nuclearmedicine.htm");

            detail.PerformedProcedureStep = CreatePerformedProcedureStepDetail(ps.PerformedSteps);

            return detail;
        }

        private PerformedProcedureStepDetail CreatePerformedProcedureStepDetail(ISet steps)
        {
            if (steps == null || steps.IsEmpty) return null;

            PerformedProcedureStep pps = (PerformedProcedureStep)CollectionUtils.FirstElement(steps);
            PerformedProcedureStepDetail detail = 
                new PerformedProcedureStepDetail(pps.GetRef(), pps.CreationTime, pps.LastStateChangeTime, pps.StartTime, pps.EndTime, pps.Documentation);

            return detail;
        }

        public void UpdateProcedureStep(ProcedureStep procedureStep, ProcedureStepDetail detail, Staff staff, IDictionary<int, EntityRef> ppsDictionary, IPersistenceContext context)
        {
            ActivityStatusEnumTable activityStatusEnumTable = context.GetBroker<IActivityStatusEnumBroker>().Load();
            ActivityStatus detailStatus = activityStatusEnumTable[detail.Status].Code;

            PerformedProcedureStep pps = GetPerformedProcedureStep(detail.PerformedProcedureStep, ppsDictionary, context);

            if (procedureStep.State != detailStatus)
            {
                if (procedureStep.State == ActivityStatus.SC && detailStatus == ActivityStatus.IP)
                {
                    procedureStep.Start(staff);

                    if(procedureStep.RequestedProcedure.Status == ActivityStatus.SC) 
                        procedureStep.RequestedProcedure.Start(staff);
                    //procedureStep.StartTime = detail.PerformedProcedureStep.StartTime;
                }
                else if (procedureStep.State == ActivityStatus.IP && detailStatus == ActivityStatus.CM)
                {
                    procedureStep.Complete(staff);

                    if(pps.State == PerformedStepStatus.IP) pps.Complete();
                    //procedureStep.EndTime = detail.PerformedProcedureStep.EndTime;
                }
                else if (procedureStep.State == ActivityStatus.SC && detailStatus == ActivityStatus.CM)
                {
                    procedureStep.Start(staff);
                    if (procedureStep.RequestedProcedure.Status == ActivityStatus.SC)
                        procedureStep.RequestedProcedure.Start(staff);
                    //procedureStep.StartTime = detail.PerformedProcedureStep.StartTime;

                    procedureStep.Complete(staff);

                    if (pps.State == PerformedStepStatus.IP) pps.Complete();
                    //procedureStep.EndTime = detail.PerformedProcedureStep.EndTime;
                }
            }

            pps.Documentation = detail.PerformedProcedureStep.Blob;

            procedureStep.PerformedSteps.Clear();
            procedureStep.AddPerformedStep(pps);

            if (detail.PerformedProcedureStep.PpsRef == null)
            {
                context.Lock(pps, DirtyState.New);
                context.SynchState();

                detail.PerformedProcedureStep.PpsRef = pps.GetRef();
                ppsDictionary.Add(detail.PerformedProcedureStep.GetHashCode(), detail.PerformedProcedureStep.PpsRef);
            }
        }

        private PerformedProcedureStep GetPerformedProcedureStep(PerformedProcedureStepDetail ppsDetail, IDictionary<int, EntityRef> ppsDictionary, IPersistenceContext context)
        {
            if (ppsDetail.PpsRef == null)
            {
                try
                {
                    ppsDetail.PpsRef = ppsDictionary[ppsDetail.GetHashCode()];
                }
                catch(KeyNotFoundException)
                {
                
                }
            }
            PerformedProcedureStep pps;
            if (ppsDetail.PpsRef != null)
            {
                pps = (PerformedProcedureStep) context.Load(ppsDetail.PpsRef);
            }
            else
            {
                pps = new ModalityPerformedProcedureStep();                
            }
            return pps;
        }
    }
}