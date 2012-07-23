#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;

namespace ClearCanvas.Ris.Client.Workflow.Extended
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
