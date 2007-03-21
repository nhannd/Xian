using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.PatientReconciliation
{
    public class PatientProfileAssembler
    {
        public PatientProfileSummary CreatePatientProfileSummary(PatientProfile profile, IPersistenceContext context)
        {
            PatientProfileSummary summary = new PatientProfileSummary();

            summary.AssigningAuthority = profile.Mrn.AssigningAuthority;
            summary.DateOfBirth = profile.DateOfBirth;
            summary.Healthcard = profile.Healthcard.Id;
            summary.Mrn = profile.Mrn.Id;
            summary.Name = profile.Name.ToString();
            summary.PatientRef = profile.Patient.GetRef();
            summary.ProfileRef = profile.GetRef();
            summary.Sex = context.GetBroker<ISexEnumBroker>().Load()[profile.Sex].Value;

            return summary;
        }
    }
}
