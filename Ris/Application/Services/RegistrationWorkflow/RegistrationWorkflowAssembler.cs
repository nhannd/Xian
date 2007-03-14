using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    public class RegistrationWorkflowAssembler
    {
        public RegistrationWorklistItem CreateWorklistItem(WorklistQueryResult result, IPersistenceContext context)
        {
            RegistrationWorklistItem item = new RegistrationWorklistItem();

            item.PatientProfileRef = result.PatientProfile;
            item.MRNAssigningAuthority = result.Mrn.AssigningAuthority;
            item.MRNID = result.Mrn.Id;

            PersonNameAssembler personNameAssembler = new PersonNameAssembler();
            item.Name = personNameAssembler.CreatePersonNameDetail(result.PatientName);

            HealthcardAssembler healthcardAssembler = new HealthcardAssembler();
            item.Healthcard = healthcardAssembler.CreateHealthcardDetail(result.HealthcardNumber);

            item.DateOfBirth = result.DateOfBirth;

            SexEnumTable sexEnumTable = context.GetBroker<ISexEnumBroker>().Load();
            item.Sex.Code = result.Sex.ToString();
            item.Sex.Value = sexEnumTable[result.Sex].Values;

            return item;
        }

    }
}
