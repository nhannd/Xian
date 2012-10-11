#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class PatientNoteEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(PatientNoteEditorComponentViewExtensionPoint))]
	public class PatientNoteEditorComponent : ApplicationComponent
	{
		private readonly bool _readOnlyMode;
		private readonly PatientNoteDetail _note;
		private readonly List<PatientNoteCategorySummary> _noteCategoryChoices;

		public PatientNoteEditorComponent(PatientNoteDetail noteDetail, List<PatientNoteCategorySummary> noteCategoryChoices)
			: this(noteDetail, noteCategoryChoices, false)
		{
		}

		public PatientNoteEditorComponent(PatientNoteDetail noteDetail, List<PatientNoteCategorySummary> noteCategoryChoices, bool readOnlyMode)
		{
			_readOnlyMode = readOnlyMode;
			_note = noteDetail;
			_noteCategoryChoices = noteCategoryChoices;

			this.Validation.Add(new ValidationRule("ExpiryDate",
				delegate
				{
					var valid = _note.ValidRangeUntil == null || _note.ValidRangeUntil > Platform.Time.Date;
					return new ValidationResult(valid, SR.MessageInvalidExpiryDate);
				}));
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

		public bool IsCommentEditable
		{
			get { return !_readOnlyMode && IsNewItem; }
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

		public bool IsExpiryDateEditable
		{
			get { return !_readOnlyMode && !IsExpired; }
		}

		public IList CategoryChoices
		{
			get
			{
				return _noteCategoryChoices;
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
				if (_note.Category == value)
					return;

				_note.Category = value;

				NotifyPropertyChanged("Category");
				NotifyPropertyChanged("CategoryDescription");
				this.Modified = true;
			}
		}

		public string CategoryDescription
		{
			get { return _note.Category == null ? "" : _note.Category.Description; }
		}

		public string FormatNoteCategory(object item)
		{
			var noteCategory = (PatientNoteCategorySummary) item;
			return String.Format(SR.FormatNoteCategory, noteCategory.Name, noteCategory.Severity.Value);
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

		#endregion

		private bool IsNewItem
		{
			get { return _note.CreationTime == null; }
		}

		private bool IsExpired
		{
			get { return this.ExpiryDate.HasValue && this.ExpiryDate < Platform.Time; }
		}
	}
}
