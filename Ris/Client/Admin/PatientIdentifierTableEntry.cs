using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Admin
{
    class PatientIdentifierTableEntry
    {
        private PatientIdentifier _patientIdentifier;
        private static IPatientAdminService _patientAdminService;

        static PatientIdentifierTableEntry()
        {
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
        }

        public PatientIdentifier PatientIdentifier
        {
            get { return _patientIdentifier; }
        }
        
        public PatientIdentifierTableEntry(PatientIdentifier patientIdentifier)
        {
            _patientIdentifier = patientIdentifier;
        }

        [TableColumn("ID")]
        public string ID
        {
            get { return _patientIdentifier.Id; }
        }

        [TableColumn("AssigningAuthority")]
        public string Name
        {
            get { return _patientIdentifier.AssigningAuthority; }
        }

        [TableColumn("Type")]
        public string Type
        {
            get { return _patientAdminService.PatientIdentifierTypeEnumTable[_patientIdentifier.Type].Value; }
        }
    }
}
