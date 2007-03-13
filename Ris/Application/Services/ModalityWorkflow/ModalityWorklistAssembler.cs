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
            item.MRNAssigningAuthority = result.Mrn.AssigningAuthority;
            item.MRNID = result.Mrn.Id;
            PersonNameAssembler assembler = new PersonNameAssembler();
            item.PersonNameDetail = assembler.CreatePersonNameDetail(result.PatientName);
            item.AccessionNumber = result.AccessionNumber;
            item.ModalityProcedureStepName = result.ModalityProcedureStepName;
            item.ModalityName = result.ModalityName;
            item.Priority = result.Priority;
            return item;
        }

        public ModalityWorklistPreview CreateWorklistPreview(ModalityProcedureStep mps, string patientProfileAuthority)
        {
            ModalityWorklistPreview preview = new ModalityWorklistPreview();

            //TODO: Technologist workflow hasn't been fully defined yet, only pass back the PatientProfile now
            List<PatientProfile> profileList = mps.RequestedProcedure.Order.Patient.Profiles;
            foreach (PatientProfile profile in profileList)
            {
                if (profile.Mrn.AssigningAuthority.Equals(patientProfileAuthority))
                {
                    preview.PatientProfile = profile.GetRef();
                    break;
                }
            }

            return preview;
        }
    
        public ModalityProcedureStepSearchCriteria CreateSearchCriteria(ModalityWorklistSearchCriteria criteria)
        {
            ModalityProcedureStepSearchCriteria mpsSearchCriteria = new ModalityProcedureStepSearchCriteria();

            // TODO Check Enum conversion
            ActivityStatus status = (ActivityStatus)Enum.Parse(typeof(ActivityStatus), criteria.ActivityStatus);
            mpsSearchCriteria.State.EqualTo(status);

            return mpsSearchCriteria;
        }
    }
}
