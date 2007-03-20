using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.PatientReconcilliation;

namespace ClearCanvas.Ris.Client.Adt
{
    class ReconciliationCandidateTableEntry
    {
        private PatientProfileMatch _profileMatch;
        private bool _checked;
        private event EventHandler _checkedChanged;

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
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    EventsHelper.Fire(_checkedChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler CheckedChanged
        {
            add { _checkedChanged += value; }
            remove { _checkedChanged -= value; }
        }
    }
}
