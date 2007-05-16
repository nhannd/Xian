using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Services.Admin;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Workflow;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow
{
    public class ModalityWorklistAssembler
    {
        public ModalityWorklistItem CreateModalityWorklistItem(WorklistItem domainItem, IPersistenceContext context)
        {
            ModalityWorklistItem item = new ModalityWorklistItem();

            item.ProcedureStepRef = domainItem.ModalityProcedureStepRef;

            PersonNameAssembler assembler = new PersonNameAssembler();
            item.PersonNameDetail = assembler.CreatePersonNameDetail(domainItem.PatientName);
            item.MrnID = domainItem.Mrn.Id;
            item.MrnAssigningAuthority = domainItem.Mrn.AssigningAuthority;

            item.AccessionNumber = domainItem.AccessionNumber;
            item.ModalityProcedureStepName = domainItem.ModalityProcedureStepType.Name;
            item.RequestedProcedureStepName = domainItem.RequestedProcedureType.Name;
            item.ModalityName = domainItem.Modality.Name;

            OrderPriorityEnumTable orderPriorities = context.GetBroker<IOrderPriorityEnumBroker>().Load();
            item.Priority = new EnumValueInfo(domainItem.Priority.ToString(), orderPriorities[domainItem.Priority].Value);

            return item;
        }

        public ModalityWorklistPreview CreateWorklistPreview(ModalityProcedureStep mps, string patientProfileAuthority)
        {
            ModalityWorklistPreview preview = new ModalityWorklistPreview();

            preview.ProcedureStepRef = mps.GetRef();

            foreach (PatientProfile profile in mps.RequestedProcedure.Order.Patient.Profiles)
            {
                if (profile.Mrn.AssigningAuthority.Equals(patientProfileAuthority))
                {
                    preview.PatientProfileRef = profile.GetRef();

                    preview.Healthcard = new HealthcardAssembler().CreateHealthcardDetail(profile.Healthcard);
                    preview.Mrn = new MrnDetail(profile.Mrn.Id, profile.Mrn.AssigningAuthority);
                    preview.Name = new PersonNameAssembler().CreatePersonNameDetail(profile.Name);
                    preview.Sex = profile.Sex.ToString();
                    preview.DateOfBirth = profile.DateOfBirth;

                    break;
                }
            }

            //TODO: Technologist workflow hasn't been fully defined yet, only pass back the PatientProfile now
            preview.AlertNotifications = new List<AlertNotificationDetail>();
            preview.HasReconciliationCandidates = false;

            return preview;
        }
    
        public ModalityProcedureStepSearchCriteria CreateSearchCriteria(ModalityWorklistSearchData criteria)
        {
            // TODO: add validation and throw RequestValidationException if necessary

            ModalityProcedureStepSearchCriteria mpsSearchCriteria = new ModalityProcedureStepSearchCriteria();

            ActivityStatus status = (ActivityStatus)Enum.Parse(typeof(ActivityStatus), criteria.ActivityStatusCode);
            mpsSearchCriteria.State.EqualTo(status);

            return mpsSearchCriteria;
        }
    }
}
