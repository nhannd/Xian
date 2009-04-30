#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Collections.ObjectModel;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Dicom.Utilities.Anonymization;

namespace ClearCanvas.Utilities.DicomEditor
{
	/// <summary>
	/// Extension point for views onto <see cref="AnonymizeStudyComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class AnonymizeStudyComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// AnonymizeStudyComponent class.
	/// </summary>
	[AssociateView(typeof(AnonymizeStudyComponentViewExtensionPoint))]
	public class AnonymizeStudyComponent : ApplicationComponent
	{
		private class ValidateAnonymizationRule : IValidationRule
		{
			private readonly AnonymizeStudyComponent _parent;
			private readonly string _property;
			private readonly ValidationFailureReason _validationReason;

			public ValidateAnonymizationRule(AnonymizeStudyComponent parent, string property, ValidationFailureReason validationReason)
			{
				_parent = parent;
				_property = property;
				_validationReason = validationReason;
			}

			#region IValidationRule Members

			public string PropertyName
			{
				get { return _property; }
			}

			public ValidationResult GetResult(IApplicationComponent component)
			{
				ReadOnlyCollection<ValidationFailureDescription> failures = _parent.GetValidationFailures();
				foreach (ValidationFailureDescription failure in failures)
				{
					if (failure.PropertyName == _property && _validationReason == failure.Reason)
					{
						if (failure.Reason == ValidationFailureReason.EmptyValue)
							return new ValidationResult(false, "The value cannot be empty.");
						else
							return new ValidationResult(false, "The value conflicts with the original value.");
					}
				}

				return new ValidationResult(true, "");
			}

			#endregion
		}

		private readonly StudyData _original;
		private readonly StudyData _anonymized;
		private readonly DicomAnonymizer.ValidationStrategy _validator;

		private bool _preserveSeriesData;

		internal AnonymizeStudyComponent(StudyItem studyItem)
		{
			_original = new StudyData();
			_original.AccessionNumber = studyItem.AccessionNumber;
			_original.PatientsName = studyItem.PatientsName;
			_original.PatientId = studyItem.PatientId;
			_original.StudyDescription = studyItem.StudyDescription;
			_original.PatientsBirthDateRaw = studyItem.PatientsBirthDate;
			_original.StudyDateRaw = studyItem.StudyDate;

			_anonymized = _original.Clone();

			_validator = new DicomAnonymizer.ValidationStrategy();
		}

		internal StudyData OriginalData
		{
			get { return _original; }	
		}

		internal StudyData AnonymizedData
		{
			get { return _anonymized; }
		}

		public string PatientsName
		{
			get { return _anonymized.PatientsNameRaw; }
			set
			{
				if (_anonymized.PatientsNameRaw == value)
					return;

				_anonymized.PatientsNameRaw = value;
				NotifyPropertyChanged("PatientsName");
			}
		}

		public string PatientId
		{
			get { return _anonymized.PatientId; }
			set
			{
				if (_anonymized.PatientId == value)
					return;

				_anonymized.PatientId = value;
				NotifyPropertyChanged("PatientId");
			}
		}

		public string AccessionNumber
		{
			get { return _anonymized.AccessionNumber; }
			set
			{
				if (_anonymized.AccessionNumber == value)
					return;

				_anonymized.AccessionNumber = value;
				NotifyPropertyChanged("AccessionNumber");
			}
		}

		public string StudyDescription
		{
			get { return _anonymized.StudyDescription; }
			set
			{
				if (_anonymized.StudyDescription == value)
					return;

				_anonymized.StudyDescription = value;
				NotifyPropertyChanged("StudyDescription");
			}
		}

		public DateTime? StudyDate
		{
			get { return _anonymized.StudyDate; }
			set
			{
				if (_anonymized.StudyDate == value)
					return;

				_anonymized.StudyDate = value;
				NotifyPropertyChanged("StudyDate");
			}
		}

		public DateTime? PatientsBirthDate
		{
			get { return _anonymized.PatientsBirthDate; }
			set
			{
				if (_anonymized.PatientsBirthDate == value)
					return;

				_anonymized.PatientsBirthDate = value;
				NotifyPropertyChanged("PatientsBirthDate");
			}
		}

		public bool PreserveSeriesData
		{
			get { return _preserveSeriesData; }
			set
			{
				if (_preserveSeriesData == value)
					return;

				_preserveSeriesData = value;
				NotifyPropertyChanged("PreserveSeriesDescriptions");
			}
		}

		public override void Start()
		{
			_anonymized.PatientsNameRaw = "Patient^Anonymous";
			_anonymized.PatientId = "12345";
			_anonymized.StudyDate = Platform.Time;
			_anonymized.AccessionNumber = "A12345";
			_preserveSeriesData = true;

			if (_anonymized.PatientsBirthDate != null)
			{
				_anonymized.PatientsBirthDate = new DateTime(_anonymized.PatientsBirthDate.Value.Year, 1, 1, 0, 0, 0);
				_anonymized.PatientsBirthDate = _anonymized.PatientsBirthDate.Value.AddDays(TimeSpan.FromDays(new Random().Next(0, 364)).Days);
			}

			base.Start();

			base.Validation.Add(new ValidateAnonymizationRule(this, "PatientId", ValidationFailureReason.EmptyValue));
			base.Validation.Add(new ValidateAnonymizationRule(this, "PatientsName", ValidationFailureReason.EmptyValue));
			base.Validation.Add(new ValidateAnonymizationRule(this, "AccessionNumber", ValidationFailureReason.EmptyValue));
			base.Validation.Add(new ValidateAnonymizationRule(this, "StudyDescription", ValidationFailureReason.EmptyValue));
			base.Validation.Add(new ValidateAnonymizationRule(this, "StudyDate", ValidationFailureReason.EmptyValue));
			base.Validation.Add(new ValidateAnonymizationRule(this, "PatientsBirthDate", ValidationFailureReason.EmptyValue));

			base.Validation.Add(new ValidateAnonymizationRule(this, "PatientId", ValidationFailureReason.ConflictingValue));
			base.Validation.Add(new ValidateAnonymizationRule(this, "PatientsName", ValidationFailureReason.ConflictingValue));
			base.Validation.Add(new ValidateAnonymizationRule(this, "AccessionNumber", ValidationFailureReason.ConflictingValue));
			base.Validation.Add(new ValidateAnonymizationRule(this, "StudyDescription", ValidationFailureReason.ConflictingValue));
			base.Validation.Add(new ValidateAnonymizationRule(this, "StudyDate", ValidationFailureReason.ConflictingValue));
			base.Validation.Add(new ValidateAnonymizationRule(this, "PatientsBirthDate", ValidationFailureReason.ConflictingValue));
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				base.ShowValidation(true);
			}
			else
			{
				this.ExitCode = ApplicationComponentExitCode.Accepted;
				this.Host.Exit();
			}
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			this.Host.Exit();
		}

		private ReadOnlyCollection<ValidationFailureDescription> GetValidationFailures()
		{
			return _validator.GetValidationFailures(_original, _anonymized);
		}
	}	
}
