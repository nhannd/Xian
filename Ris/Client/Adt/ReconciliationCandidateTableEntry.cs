using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Adt
{
    class ReconciliationCandidateTableEntry
    {
        private PatientProfileMatch _profileMatch;
        private bool _checked;

        public ReconciliationCandidateTableEntry(PatientProfileMatch match)
        {
            _profileMatch = match;
            _checked = false;
        }

        public PatientProfileMatch ProfileMatch
        {
            get { return _profileMatch; }
        }

        public bool Checked
        {
            get { return _checked; }
            set { _checked = value; }
        }
    }
}
