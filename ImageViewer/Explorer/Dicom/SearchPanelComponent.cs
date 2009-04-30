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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using System.Text.RegularExpressions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	/// <summary>
	/// Extension point for views onto <see cref="SearchPanelComponent"/>
	/// </summary>
	[ExtensionPoint]
	public sealed class SearchPanelComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}
	
	/// <summary>
	/// SearchPanelComponent class
	/// </summary>
	[AssociateView(typeof(SearchPanelComponentViewExtensionPoint))]
	public class SearchPanelComponent : ApplicationComponent
	{
		private const string _disallowedCharacters = @"\r\n\e\f\\";
		private const string _disallowedCharactersPattern = @"[" + _disallowedCharacters + @"]+";

		private readonly StudyBrowserComponent _studyBrowserComponent;

		private string _title;

		private string _name = "";
		private string _patientID = "";
		private string _accessionNumber = "";
		private string _studyDescription = "";

		private DateTime? _studyDateFrom = null;
		private DateTime? _studyDateTo = null;

		private readonly Regex _openNameSearchRegex;
		private readonly Regex _lastNameFirstNameRegex;

		private readonly List<string> _searchModalities;
		private readonly ICollection<string> _availableModalities;
		
		/// <summary>
		/// Constructor
		/// </summary>
		public SearchPanelComponent(StudyBrowserComponent studyBrowserComponent)
		{
			Platform.CheckForNullReference(studyBrowserComponent, "studyBrowserComponent");
			_studyBrowserComponent = studyBrowserComponent;
			_studyBrowserComponent.SearchPanelComponent = this;

			_searchModalities = new List<string>();
			_availableModalities = StandardModalities.Modalities;

			InternalClearDates();

			char separator = DicomExplorerConfigurationSettings.Default.NameSeparator;
			string allowedExceptSeparator = @"[^" + _disallowedCharacters + separator + @"]";
			string allowedExceptSeparatorAndWhitespace = @"[^" + _disallowedCharacters + separator + @"\s]";

			// Last Name, First Name search
			// \A\s*[^\r\n\e\f\\,\s]+\s*,{1}[^\r\n\e\f\\,]*\Z
			//
			// Examples of matches:
			// Doe, John
			// Doe,
			//
			// Examples of non-matches:
			// ,
			// Doe, John,

			_lastNameFirstNameRegex = new Regex(@"\A\s*" + allowedExceptSeparatorAndWhitespace + @"+\s*"
				+ separator + @"{1}" + allowedExceptSeparator + @"*\Z");

			// Open search
			// \A\s*[^\r\n\e\f\\,]+\s*\Z
			//
			// John
			// Doe
			//
			// Examples of non-matches:
			// ,
			// Doe,
			// John

			_openNameSearchRegex = new Regex(@"\A\s*" + allowedExceptSeparator + @"+\s*\Z");
		}

		public IDesktopWindow DesktopWindow
		{
			get { return this.Host.DesktopWindow; }
		}

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				NotifyPropertyChanged("Title");
			}
		}

		[ValidateRegex(_disallowedCharactersPattern, SuccessOnMatch = false, Message = "ValidationInvalidCharacters")]
		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set
			{
				_accessionNumber = value ?? "";
				NotifyPropertyChanged("AccessionNumber");
			}
		}

		[ValidateRegex(_disallowedCharactersPattern, SuccessOnMatch = false, Message = "ValidationInvalidCharacters")]
		public string PatientID
		{
			get { return _patientID; }
			set
			{
				_patientID = value ?? "";
				NotifyPropertyChanged("PatientID");
			}
		}

		public string PatientsName
		{
			get { return _name; }
			set
			{
				_name = value ?? "";
				NotifyPropertyChanged("PatientsName");
			}
		}

		[ValidateRegex(_disallowedCharactersPattern, SuccessOnMatch = false, Message = "ValidationInvalidCharacters")]
		public string StudyDescription
		{
			get { return _studyDescription; }
			set
			{
				_studyDescription = value ?? "";
				NotifyPropertyChanged("StudyDescription");
			}
		}

		public DateTime? StudyDateFrom
		{
			get
			{
				return _studyDateFrom;
			}
			set
			{
				_studyDateFrom = (value == null) ? value : MinimumDate(((DateTime)value).Date, this.MaximumStudyDateFrom);
				NotifyPropertyChanged("StudyDateFrom");
			}
		}

		public DateTime? StudyDateTo
		{
			get
			{
				return _studyDateTo;
			}
			set
			{
				_studyDateTo = (value == null) ? value : MinimumDate(((DateTime)value).Date, this.MaximumStudyDateTo);
				NotifyPropertyChanged("StudyDateTo");
			}
		}

		public DateTime MaximumStudyDateFrom
		{
			get { return Platform.Time.Date; }
		}

		public DateTime MaximumStudyDateTo
		{
			get { return Platform.Time.Date; }
		}

		public ICollection<string> AvailableSearchModalities
		{
			get { return _availableModalities; }
		}

		public IList<string> SearchModalities
		{
			get { return _searchModalities; }
			set
			{
				_searchModalities.Clear();

				if (value != null)
					_searchModalities.AddRange(value);

				NotifyPropertyChanged("SearchModalities");
			}
		}

		public override void Start()
		{
			this.Title = SR.TitleSearch;

			base.Start();
		}

		public void Clear()
		{
			this.PatientID = "";
			this.PatientsName = "";
			this.AccessionNumber = "";
			this.StudyDescription = "";
			this.SearchModalities = new List<string>(); //clear the checked modalities.

			InternalClearDates();
		}

		public void Search()
		{
			if (base.HasValidationErrors)
			{
				base.ShowValidation(true);
				return;
			}
			else
			{
				base.ShowValidation(false);
			}

			try
			{
				BlockingOperation.Run(_studyBrowserComponent.Search);
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void SearchToday()
        {
			InternalSearchLastXDays(0);
        }

		public void SearchLastWeek()
		{
			InternalSearchLastXDays(7);
		}

		public void SearchLastXDays(int numberOfDays)
		{
			InternalSearchLastXDays(numberOfDays);
		}

		private void InternalSearchLastXDays(int numberOfDays)
		{
			this.StudyDateTo = Platform.Time.Date;
			this.StudyDateFrom = Platform.Time.Date - TimeSpan.FromDays((double)numberOfDays);

			Search();
		}

		private void InternalClearDates()
		{
			this.StudyDateTo = Platform.Time.Date;
			this.StudyDateFrom = Platform.Time.Date;
			this.StudyDateTo = null;
			this.StudyDateFrom = null;
		}

		private DateTime MinimumDate(params DateTime[] dates)
		{
			DateTime minimumDate = dates[0];
			for (int i = 1; i < dates.Length; ++i)
			{
				if (dates[i] < minimumDate)
					minimumDate = dates[i];
			}

			return minimumDate;
		}

		internal string[] GetPatientsNameComponents()
		{
			char separator = DicomExplorerConfigurationSettings.Default.NameSeparator;
			string patientsName = PatientsName.Trim();
			if (String.IsNullOrEmpty(patientsName))
				return new string[0];

			return patientsName.Split(new char[] { separator }, StringSplitOptions.None);
		}

		[ValidationMethodFor("StudyDateFrom")]
		private ValidationResult ValidateStudyDateFrom()
		{
			return ValidateDateRange();
		}

		[ValidationMethodFor("StudyDateTo")]
		private ValidationResult ValidateStudyDateTo()
		{
			return ValidateDateRange();
		}

		[ValidationMethodFor("PatientsName")]
		private ValidationResult ValidatePatientsName()
		{
			if (String.IsNullOrEmpty(PatientsName))
			{
				return new ValidationResult(true, "");
			}
			else if (_openNameSearchRegex.IsMatch(PatientsName) || _lastNameFirstNameRegex.IsMatch(PatientsName))
			{
				return new ValidationResult(true, "");
			}
			else if (PatientsName.Contains(DicomExplorerConfigurationSettings.Default.NameSeparator.ToString()))
			{
				return new ValidationResult(false, SR.ValidationInvalidLastNameSearch);
			}
			else
			{
				return new ValidationResult(false, SR.ValidationInvalidCharacters);
			}
		}

		private ValidationResult ValidateDateRange()
		{
			if (this.StudyDateFrom.HasValue && this.StudyDateTo.HasValue)
			{
				if (this.StudyDateFrom.Value <= this.StudyDateTo.Value)
					return new ValidationResult(true, "");

				return new ValidationResult(false, SR.MessageFromDateCannotBeAfterToDate);
			}

			return new ValidationResult(true, "");
		}
    }
}
