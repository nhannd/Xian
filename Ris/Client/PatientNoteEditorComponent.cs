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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Validation;
using System.Collections;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class PatientNoteEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(PatientNoteEditorComponentViewExtensionPoint))]
    public class PatientNoteEditorComponent : ApplicationComponent
    {
        private readonly PatientNoteDetail _note;
        private readonly List<PatientNoteCategorySummary> _noteCategoryChoices;

        public PatientNoteEditorComponent(PatientNoteDetail noteDetail, List<PatientNoteCategorySummary> noteCategoryChoices)
        {
            _note = noteDetail;
            _noteCategoryChoices = noteCategoryChoices;

            this.Validation.Add(new ValidationRule("ExpiryDate",
                delegate
                {
                    bool valid = _note.ValidRangeUntil == null || _note.ValidRangeUntil >= Platform.Time.Date;
                    return new ValidationResult(valid, "Expiry date cannot be in the past.");
                }));
        }

        public override void Start()
        {
            base.Start();
        }

        #region Presentation Model

        public string Comment
        {
            get { return _note.Comment; }
            set 
            { 
                _note.Comment = value;
                this.Modified = true;
            }
        }

        public DateTime? ExpiryDate
        {
            get { return _note.ValidRangeUntil == null ? null : (DateTime?)_note.ValidRangeUntil.Value.Date; }
            set
            {
                _note.ValidRangeUntil = (value == null) ? null : (DateTime?)value.Value.Date;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public PatientNoteCategorySummary Category
        {
            get 
            {
                return _note.Category;
            }
            set
            {
                if (_note.Category != value)
                {
                    _note.Category = value;

                    NotifyPropertyChanged("Category");
                    NotifyPropertyChanged("CategoryDescription");
                    this.Modified = true;
                }
            }
        }

        public string FormatNoteCategory(object item)
        {
            PatientNoteCategorySummary noteCategory = (PatientNoteCategorySummary) item;
            return String.Format(SR.FormatNoteCategory, noteCategory.Name, noteCategory.Severity.Value);
        }

        public string CategoryDescription
        {
            get { return _note.Category == null ? "" : _note.Category.Description; }
        }

        public IList CategoryChoices
        {
            get 
            {
                return _noteCategoryChoices;
            }
        }

        public bool IsNewItem
        {
            get { return _note.CreationTime == null; }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                this.ExitCode = ApplicationComponentExitCode.Accepted;
                Host.Exit();
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        #endregion

    }
}
