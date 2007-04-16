using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Services.Admin;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow
{
    public class ModalityWorklistAssembler
    {
        public ModalityWorklistItem CreateWorklistItem(WorklistQueryResult result)
        {
            ModalityWorklistItem item = new ModalityWorklistItem();

            item.ProcedureStepRef = result.ProcedureStep;
            item.MrnAssigningAuthority = result.Mrn.AssigningAuthority;
            item.MrnID = result.Mrn.Id;
            PersonNameAssembler assembler = new PersonNameAssembler();
            item.PersonNameDetail = assembler.CreatePersonNameDetail(result.PatientName);
            item.AccessionNumber = result.AccessionNumber;
            item.ModalityProcedureStepName = result.ModalityProcedureStepName;
            item.ModalityName = result.ModalityName;
            item.Priority = result.Priority.ToString();
            return item;
        }

        public ModalityWorklistPreview CreateWorklistPreview(ModalityProcedureStep mps, string patientProfileAuthority)
        {
            ModalityWorklistPreview preview = new ModalityWorklistPreview();

            //TODO: Technologist workflow hasn't been fully defined yet, only pass back the PatientProfile now
            foreach (PatientProfile profile in mps.RequestedProcedure.Order.Patient.Profiles)
            {
                if (profile.Mrn.AssigningAuthority.Equals(patientProfileAuthority))
                {
                    preview.PatientProfile = profile.GetRef();
                    break;
                }
            }

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
