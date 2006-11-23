using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Services
{
    public class PatientProfileDiff
    {
        private PatientProfile[] _profiles;
        private PatientProfileDiscrepancy _discrepancies;

        public PatientProfileDiff(PatientProfile[] profiles, PatientProfileDiscrepancy discrepancies)
        {
            _profiles = profiles;
            _discrepancies = discrepancies;
        }

        public PatientProfile[] Profiles
        {
            get { return _profiles; }
        }

        public PatientProfileDiscrepancy Discrepancies
        {
            get { return _discrepancies; }
        }
    }
}
