#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
    class ReconciliationCandidateTable : Table<ReconciliationCandidateTableEntry>
    {
        public ReconciliationCandidateTable()
        {
            this.Columns.Add(
               new TableColumn<ReconciliationCandidateTableEntry, bool>(SR.ColumnAbbreviationReconciliation,
                   delegate(ReconciliationCandidateTableEntry item) { return item.Checked; },
                   delegate(ReconciliationCandidateTableEntry item, bool value) { item.Checked = value; }, 0.5f));
            this.Columns.Add(
                new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnScore,
                    delegate(ReconciliationCandidateTableEntry item) { return item.ReconciliationCandidate.Score.ToString(); }, 1.0f));
            this.Columns.Add(
                new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnSite,
                    delegate(ReconciliationCandidateTableEntry item) { return item.ReconciliationCandidate.PatientProfile.Mrn.AssigningAuthority.Code; }, 0.5f));
            this.Columns.Add(
               new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnMRN,
                   delegate(ReconciliationCandidateTableEntry item) { return item.ReconciliationCandidate.PatientProfile.Mrn.Id; }, 1.0f));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnName,
                  delegate(ReconciliationCandidateTableEntry item) { return PersonNameFormat.Format(item.ReconciliationCandidate.PatientProfile.Name); }, 2.0f));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnHealthcardNumber,
                  delegate(ReconciliationCandidateTableEntry item) { return HealthcardFormat.Format(item.ReconciliationCandidate.PatientProfile.Healthcard); }, 1.0f));

            DateTimeTableColumn<ReconciliationCandidateTableEntry> dateOfBirthColumn =
              new DateTimeTableColumn<ReconciliationCandidateTableEntry>(SR.ColumnDateOfBirth,
                  delegate(ReconciliationCandidateTableEntry item) { return item.ReconciliationCandidate.PatientProfile.DateOfBirth; }, 1.0f);
            this.Columns.Add(dateOfBirthColumn);

            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnSex,
                  delegate(ReconciliationCandidateTableEntry item) { return item.ReconciliationCandidate.PatientProfile.Sex.Value; }, 0.5f));
        }
    }
}
