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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using System.Text;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public class SearchParams
    {
        public SearchParams(string textSearch, bool showActiveOnly)
        {
            this.TextSearch = textSearch;
            this.ShowActiveOnly = showActiveOnly;
        }

        public string TextSearch;
        public bool ShowActiveOnly;
    }

    /// <summary>
    /// Defines an interface for handling the Search Data
    /// </summary>
    public interface ISearchDataHandler
    {
        SearchParams SearchParams { set; }
    }

    [ExtensionPoint]
    public class SearchComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(SearchComponentViewExtensionPoint))]
    public class SearchComponent : ApplicationComponent
    {
    	private string _searchString;

		// Filters
    	private string _accessionNumber;
    	private string _patientIdMrn;
    	private string _healthcardNumber;
    	private string _patientName;
    	private DateTime? _startDate;
    	private DateTime? _stopDate;
		private ProcedureTypeSummary _procedureType;
		private ExternalPractitionerSummary _orderingPractitioner;
		private ExternalPractitionerLookupHandler _orderingPractitionerLookupHandler;

		private bool _showActiveOnly;
		private bool _keepOpen;

		private static SearchComponent _instance;
        private static Shelf _searchComponentShelf;

        private SearchComponent()
        {
            // True by default
            _showActiveOnly = true;
        }

        public static Shelf Launch(IDesktopWindow desktopWindow)
        {
            try
            {
                if (_searchComponentShelf != null)
                {
                    _searchComponentShelf.Activate();
                }
                else
                {
                    desktopWindow.Workspaces.ItemActivationChanged += Workspaces_ItemActivationChanged;

                    _searchComponentShelf = LaunchAsShelf(
                        desktopWindow,
                        Instance,
                        SR.TitleSearch,
                        ShelfDisplayHint.DockFloat);

                    _searchComponentShelf.Closed += delegate
                            {
                                desktopWindow.Workspaces.ItemActivationChanged -= Workspaces_ItemActivationChanged;
                                _searchComponentShelf = null;
                            };

                    UpdateDisplay();
                }
            }
            catch (Exception e)
            {
                // cannot start component
                ExceptionHandler.Report(e, desktopWindow);
            }

            return _searchComponentShelf;
        }

        public static SearchComponent Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SearchComponent();

                return _instance;
            }
        }

		public override void Start()
		{
			_orderingPractitionerLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);

			base.Start();
		}

        #region Public Member

		public string SearchString
		{
			get { return _searchString; }
			private set
			{
				_searchString = value;

				NotifyPropertyChanged("SearchString");
				NotifyPropertyChanged("SearchEnabled");
				UpdateDisplay();
			}
		}

        public string AccessionNumber
        {
			get { return _accessionNumber; }
            set
            {
            	_accessionNumber = value;
            	UpdateSearchString();
            }
        }

		public string PatientIdMrn
		{
			get { return _patientIdMrn; }
			set
			{
				_patientIdMrn = value;
				UpdateSearchString();
			}
		}

		public string HealthcardNumber
		{
			get { return _healthcardNumber; }
			set
			{
				_healthcardNumber = value;
				UpdateSearchString();
			}
		}

		public string PatientName
		{
			get { return _patientName; }
			set
			{
				_patientName = value;
				UpdateSearchString();
			}
		}

		public ExternalPractitionerSummary OrderingPractitioner
		{
			get { return _orderingPractitioner; }
			set
			{
				_orderingPractitioner = value;
				UpdateSearchString();
			}
		}

		public ILookupHandler OrderingPractitionerLookupHandler
		{
			get { return _orderingPractitionerLookupHandler; }
		}

    	public ProcedureTypeSummary ProcedureType
    	{
			get { return _procedureType; }
			set
			{
				_procedureType = value;
				UpdateSearchString();
			}
    	}

		public DateTime? StartDate
    	{
			get { return _startDate; }
			set
			{
				_startDate = value;
				UpdateSearchString();
			}
		}

		public DateTime? StopDate
		{
			get { return _stopDate; }
			set
			{
				_stopDate = value;
				UpdateSearchString();
			}
		}

		public bool ShowActiveOnly
        {
            get { return _showActiveOnly; }
            set { _showActiveOnly = value; }
        }

        public bool KeepOpen
        {
            get { return _keepOpen; }
            set { _keepOpen = value; }
        }

        public bool SearchEnabled
        {
            get { return !string.IsNullOrEmpty(_searchString); }
        }

        public void Search()
        {
            if (this.HasValidationErrors)
            {
				this.ShowValidation(true);
            	return;
            }

            ISearchDataHandler searchHandler = GetActiveWorkspaceSearchHandler();
            if (searchHandler != null)
				searchHandler.SearchParams = new SearchParams(_searchString, _showActiveOnly);

            // always turn the validation errors off after a successful search
            this.ShowValidation(false);

            if (!_keepOpen)
            {
                this.Host.Exit();
            }
        }

		public string FormatProcedureType(object item)
		{
			ProcedureTypeSummary rpt = (ProcedureTypeSummary)item;
			return string.Format("{0} ({1})", rpt.Name, rpt.Id);
		}
		
        #endregion

		private void UpdateSearchString()
		{
			StringBuilder builder = new StringBuilder();
			
			if (!string.IsNullOrEmpty(_accessionNumber))
			{
				builder.Append(" a:");
				builder.Append(_accessionNumber);
			}

			if (!string.IsNullOrEmpty(_patientIdMrn))
			{
				builder.Append(" pid/mrn:");
				builder.Append(_patientIdMrn);
			}

			if (!string.IsNullOrEmpty(_healthcardNumber))
			{
				builder.Append(" h:");
				builder.Append(_healthcardNumber);
			}

			if (!string.IsNullOrEmpty(_patientName))
			{
				builder.Append(" pn:");
				builder.Append(_patientName);
			}

			if (_orderingPractitioner != null)
			{
				builder.Append(" op:");
				builder.Append(PersonNameFormat.Format(_orderingPractitioner.Name));
			}

			if (_procedureType != null)
			{
				builder.Append(" proc:");
				builder.Append(_procedureType.Name);
			}

			if (_startDate != null)
			{
				builder.Append(" start:");
				builder.Append(Format.Date(_startDate));
			}

			if (_stopDate != null)
			{
				builder.Append(" end:");
				builder.Append(Format.Date(_stopDate));
			}

			this.SearchString = builder.ToString();
		}

        private static void Workspaces_ItemActivationChanged(object sender, ItemEventArgs<Workspace> e)
        {
            UpdateDisplay();
        }

        private static ISearchDataHandler GetActiveWorkspaceSearchHandler()
        {
            Workspace activeWorkspace = _searchComponentShelf.DesktopWindow.ActiveWorkspace;
            if (activeWorkspace != null && activeWorkspace.Component is ISearchDataHandler)
                return (ISearchDataHandler) (activeWorkspace.Component);

            return null;
        }

        private static void UpdateDisplay()
        {
            ISearchDataHandler searchHandler = GetActiveWorkspaceSearchHandler();
            if (searchHandler != null)
            {
                // Use this to update the SearchComponentControl when a workspace that implement ISearchDataHandler is active
                // Do something here
            }
        }
    }
}
