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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using System.Text.RegularExpressions;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

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

		private string _title;

		private string _name = "";
		private string _referringPhysiciansName = "";
		private string _patientID = "";
		private string _accessionNumber = "";
		private string _studyDescription = "";

		private DateTime? _studyDateFrom = null;
		private DateTime? _studyDateTo = null;

		private readonly Regex _openNameSearchRegex;
		private readonly Regex _lastNameFirstNameRegex;

		private readonly List<string> _searchModalities;
		private readonly ICollection<string> _availableModalities;

		private event EventHandler<SearchRequestEventArgs> _searchRequestEvent;

		/// <summary>
		/// Constructor
		/// </summary>
		public SearchPanelComponent()
		{
			_searchModalities = new List<string>();
			_availableModalities = StandardModalities.Modalities;

			InternalClearDates();

			char separator = DicomExplorerConfigurationSettings.Default.NameSeparator;
			string allowedExceptSeparator = String.Format("[^{0}{1}]", _disallowedCharacters, separator);

			// Last Name, First Name search
			// \A\s*[^\r\n\e\f\\,]+\s*[^\r\n\e\f\\,]*\s*,[^\r\n\e\f\\,]*\Z
			//
			// Examples of matches:
			// Doe, John
			// Doe,
			//
			// Examples of non-matches:
			// ,
			// Doe, John,

			_lastNameFirstNameRegex = new Regex(String.Format(@"\A\s*{0}+\s*{0}*\s*{1}{0}*\Z", allowedExceptSeparator, separator));

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

			_openNameSearchRegex = new Regex(String.Format(@"\A\s*{0}+\s*\Z", allowedExceptSeparator));
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

		public string ReferringPhysiciansName
		{
			get { return _referringPhysiciansName; }
			set
			{
				_referringPhysiciansName = value ?? "";
				NotifyPropertyChanged("ReferringPhysiciansName");
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
			this.ReferringPhysiciansName = "";
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

			var queryParamsList = GetQueryParametersPermutation();
			var eventArgs = new SearchRequestEventArgs(queryParamsList);
			EventsHelper.Fire(_searchRequestEvent, this, eventArgs);
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

		public event EventHandler<SearchRequestEventArgs> SearchRequestEvent
		{
			add { _searchRequestEvent += value; }
			remove { _searchRequestEvent -= value; }
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
			return ValidateName(PatientsName);
		}
		
		[ValidationMethodFor("ReferringPhysiciansName")]
		private ValidationResult ValidateReferringPhysiciansName()
		{
			return ValidateName(ReferringPhysiciansName);
		}

		private ValidationResult ValidateName(string name)
		{
			if (String.IsNullOrEmpty(name))
			{
				return new ValidationResult(true, "");
			}
			else if (_openNameSearchRegex.IsMatch(name) || _lastNameFirstNameRegex.IsMatch(name))
			{
				return new ValidationResult(true, "");
			}
			else if (name.Contains(DicomExplorerConfigurationSettings.Default.NameSeparator.ToString()))
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

		private List<QueryParameters> GetQueryParametersPermutation()
		{
			var baseQueryParams = PrepareBaseQueryParameters();

			if (string.IsNullOrEmpty(this.Species) && string.IsNullOrEmpty(this.Breed))
				return new List<QueryParameters> { baseQueryParams };

			var queryParametersSets = new List<QueryParameters>();
			if (string.IsNullOrEmpty(this.Species))
			{
				var parametersSets = GetBreedQueryParametersSets(baseQueryParams, this.Breed);
				queryParametersSets.AddRange(parametersSets);
			}
			else if (string.IsNullOrEmpty(this.Breed))
			{
				var parametersSets = GetSpeciesQueryParametersSets(baseQueryParams, this.Species);
				queryParametersSets.AddRange(parametersSets);
			}
			else
			{
				// If both species and breed are specified, 3x3 = 9 queries will need to be performed in order to get all permutations of 
				// the search parameters.
				var speciesParametersSets = GetSpeciesQueryParametersSets(baseQueryParams, this.Species);
				foreach (var q in speciesParametersSets)
				{
					var speciesAndBreedParametersSets = GetBreedQueryParametersSets(q, this.Breed);
					queryParametersSets.AddRange(speciesAndBreedParametersSets);
				}
			}

			return queryParametersSets;
		}

		private QueryParameters PrepareBaseQueryParameters()
		{
			var patientsName = ConvertNameToSearchCriteria(this.PatientsName);
			var referringPhysiciansName = ConvertNameToSearchCriteria(this.ReferringPhysiciansName);

			var patientId = "";
			if (!String.IsNullOrEmpty(this.PatientID))
				patientId = this.PatientID + "*";

			var accessionNumber = "";
			if (!String.IsNullOrEmpty(this.AccessionNumber))
				accessionNumber = this.AccessionNumber + "*";

			var studyDescription = "";
			if (!String.IsNullOrEmpty(this.StudyDescription))
				studyDescription = this.StudyDescription + "*";

			var dateRangeQuery = DateRangeHelper.GetDicomDateRangeQueryString(this.StudyDateFrom, this.StudyDateTo);

			//At the application level, ClearCanvas defines the 'ModalitiesInStudy' filter as a multi-valued
			//Key Attribute.  This goes against the Dicom standard for C-FIND SCU behaviour, so the
			//underlying IStudyFinder(s) must handle this special case, either by ignoring the filter
			//or by running multiple queries, one per modality specified (for example).
			var modalityFilter = DicomStringHelper.GetDicomStringArray(this.SearchModalities);

			var responsiblePerson = ConvertNameToSearchCriteria(this.ResponsiblePerson);

			var responsibleOrganization = "";
			if (!String.IsNullOrEmpty(this.ResponsibleOrganization))
				responsibleOrganization = this.ResponsibleOrganization + "*";

			var queryParams = new QueryParameters();
			queryParams.Add("PatientsName", patientsName);
			queryParams.Add("ReferringPhysiciansName", referringPhysiciansName);
			queryParams.Add("PatientId", patientId);
			queryParams.Add("AccessionNumber", accessionNumber);
			queryParams.Add("StudyDescription", studyDescription);
			queryParams.Add("ModalitiesInStudy", modalityFilter);
			queryParams.Add("StudyDate", dateRangeQuery);
			queryParams.Add("StudyInstanceUid", "");

			queryParams.Add("PatientSpeciesDescription", "");
			queryParams.Add("PatientSpeciesCodeSequenceCodeValue", "");
			queryParams.Add("PatientSpeciesCodeSequenceCodeMeaning", "");
			queryParams.Add("PatientBreedDescription", "");
			queryParams.Add("PatientBreedCodeSequenceCodeValue", "");
			queryParams.Add("PatientBreedCodeSequenceCodeMeaning", "");
			queryParams.Add("ResponsiblePerson", responsiblePerson);
			queryParams.Add("ResponsibleOrganization", responsibleOrganization);

			return queryParams;
		}

		private static List<QueryParameters> GetSpeciesQueryParametersSets(QueryParameters baseQueryParams, string species)
		{
			var queryParametersSet = new List<QueryParameters>();

			var queryParams = new QueryParameters(baseQueryParams);
			queryParams["PatientSpeciesDescription"] = species;
			queryParametersSet.Add(queryParams);

			queryParams = new QueryParameters(baseQueryParams);
			queryParams["PatientSpeciesCodeSequenceCodeValue"] = species;
			queryParametersSet.Add(queryParams);

			queryParams = new QueryParameters(baseQueryParams);
			queryParams["PatientSpeciesCodeSequenceCodeMeaning"] = species;
			queryParametersSet.Add(queryParams);

			return queryParametersSet;
		}

		private static List<QueryParameters> GetBreedQueryParametersSets(QueryParameters baseQueryParams, string breed)
		{
			var queryParametersSet = new List<QueryParameters>();

			var queryParams = new QueryParameters(baseQueryParams);
			queryParams["PatientBreedDescription"] = breed;
			queryParametersSet.Add(queryParams);

			queryParams = new QueryParameters(baseQueryParams);
			queryParams["PatientBreedCodeSequenceCodeValue"] = breed;
			queryParametersSet.Add(queryParams);

			queryParams = new QueryParameters(baseQueryParams);
			queryParams["PatientBreedCodeSequenceCodeMeaning"] = breed;
			queryParametersSet.Add(queryParams);

			return queryParametersSet;
		}

		private static string ConvertNameToSearchCriteria(string name)
		{
			string[] nameComponents = GetNameComponents(name);
			if (nameComponents.Length == 1)
			{
				//Open name search
				return String.Format("*{0}*", nameComponents[0].Trim());
			}
			else if (nameComponents.Length > 1)
			{
				if (String.IsNullOrEmpty(nameComponents[0]))
				{
					//Open name search - should never get here
					return String.Format("*{0}*", nameComponents[1].Trim());
				}
				else if (String.IsNullOrEmpty(nameComponents[1]))
				{
					//Pure Last Name search
					return String.Format("{0}*", nameComponents[0].Trim());
				}
				else
				{
					//Last Name, First Name search
					return String.Format("{0}*{1}*", nameComponents[0].Trim(), nameComponents[1].Trim());
				}
			}

			return "";
		}

		private static string[] GetNameComponents(string unparsedName)
		{
			unparsedName = unparsedName ?? "";
			char separator = DicomExplorerConfigurationSettings.Default.NameSeparator;
			string name = unparsedName.Trim();
			if (String.IsNullOrEmpty(name))
				return new string[0];

			return name.Split(new char[] { separator }, StringSplitOptions.None);
		}
	}
}
