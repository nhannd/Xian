using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;

namespace ClearCanvas.Ris.Client.Adt
{
    class ReconciliationCandidateTableEntry
    {
        private ReconciliationCandidate _profileMatch;
        private bool _checked;
        private event EventHandler _checkedChanged;

        public ReconciliationCandidateTableEntry(ReconciliationCandidate match)
        {
            _profileMatch = match;
            _checked = false;
        }

        public ReconciliationCandidate ReconciliationCandidate
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
