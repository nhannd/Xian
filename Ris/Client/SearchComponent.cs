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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class SearchParams
	{
		/// <summary>
		/// Constructor for text-based search.
		/// </summary>
		public SearchParams(string textSearch)
		{
			this.TextSearch = textSearch;
		}

		/// <summary>
		/// Constructor for advance search
		/// </summary>
		public SearchParams(WorklistItemTextQueryRequest.AdvancedSearchFields searchFields)
		{
			this.UseAdvancedSearch = true;
			this.SearchFields = searchFields;
		}

		/// <summary>
		/// Specifies the query text.
		/// </summary>
		public string TextSearch;

		/// <summary>
		/// Specifies that "advanced" mode should be used, in which case the text query is ignored
		/// and the search is based on the content of the <see cref="SearchFields"/> member.
		/// </summary>
		public bool UseAdvancedSearch;

		/// <summary>
		/// Data used in the advanced search mode.
		/// </summary>
		public WorklistItemTextQueryRequest.AdvancedSearchFields SearchFields;
	}

	/// <summary>
	/// Defines an interface for handling the Search Data
	/// </summary>
	public interface ISearchDataHandler
	{
		bool SearchEnabled { get; }
		SearchParams SearchParams { set; }
		event EventHandler SearchEnabledChanged;
	}

	[ExtensionPoint]
	public class SearchComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(SearchComponentViewExtensionPoint))]
	public class SearchComponent : ApplicationComponent
	{
		private readonly SearchParams _searchParams;

		private ProcedureTypeSummary _selectedProcedureType;
		private ProcedureTypeLookupHandler _procedureTypeLookupHandler;
		private ExternalPractitionerSummary _selectedOrderingPractitioner;
		private ExternalPractitionerLookupHandler _orderingPractitionerLookupHandler;

		private bool _keepOpen;

		private static SearchComponent _instance;
		private static Shelf _searchComponentShelf;
		private static ISearchDataHandler _activeSearchHandler = GetActiveWorkspaceSearchHandler();

		private SearchComponent()
		{
			_searchParams = new SearchParams(new WorklistItemTextQueryRequest.AdvancedSearchFields());
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
			_procedureTypeLookupHandler = new ProcedureTypeLookupHandler(this.Host.DesktopWindow);

			base.Start();
		}

		#region Public Member

		public string AccessionNumber
		{
			get { return _searchParams.SearchFields.AccessionNumber; }
			set
			{
				_searchParams.SearchFields.AccessionNumber = value;
				NotifyPropertyChanged("AccessionNumber");
			}
		}

		public string Mrn
		{
			get { return _searchParams.SearchFields.Mrn; }
			set
			{
				_searchParams.SearchFields.Mrn = value;
				NotifyPropertyChanged("Mrn");
			}
		}

		public string HealthcardNumber
		{
			get { return _searchParams.SearchFields.HealthcardNumber; }
			set
			{
				_searchParams.SearchFields.HealthcardNumber = value;
				NotifyPropertyChanged("HealthcardNumber");
			}
		}

		public string FamilyName
		{
			get { return _searchParams.SearchFields.FamilyName; }
			set
			{
				_searchParams.SearchFields.FamilyName = value;
				NotifyPropertyChanged("FamilyName");
			}
		}

		public string GivenName
		{
			get { return _searchParams.SearchFields.GivenName; }
			set
			{
				_searchParams.SearchFields.GivenName = value;
				NotifyPropertyChanged("GivenName");
			}
		}
		public ExternalPractitionerSummary OrderingPractitioner
		{
			get { return _selectedOrderingPractitioner; }
			set
			{
				_selectedOrderingPractitioner = value;
				_searchParams.SearchFields.OrderingPractitionerRef = value == null ? null : value.PractitionerRef;
				NotifyPropertyChanged("OrderingPractitioner");
			}
		}

		public ILookupHandler OrderingPractitionerLookupHandler
		{
			get { return _orderingPractitionerLookupHandler; }
		}

		public ProcedureTypeSummary ProcedureType
		{
			get { return _selectedProcedureType; }
			set
			{
				_selectedProcedureType = value;
				_searchParams.SearchFields.ProcedureTypeRef = value == null ? null : value.ProcedureTypeRef;
				NotifyPropertyChanged("ProcedureType");
			}
		}

		public ILookupHandler ProcedureTypeLookupHandler
		{
			get { return _procedureTypeLookupHandler; }
		}

		public DateTime? FromDate
		{
			get { return _searchParams.SearchFields.FromDate; }
			set
			{
				_searchParams.SearchFields.FromDate = value;
				NotifyPropertyChanged("FromDate");
			}
		}

		public DateTime? UntilDate
		{
			get { return _searchParams.SearchFields.UntilDate; }
			set
			{
				_searchParams.SearchFields.UntilDate = value;
				NotifyPropertyChanged("UntilDate");
			}
		}

		public bool KeepOpen
		{
			get { return _keepOpen; }
			set { _keepOpen = value; }
		}

		public bool SearchEnabled
		{
			get
			{
				return this.ComponentEnabled
					&& (!string.IsNullOrEmpty(_searchParams.SearchFields.AccessionNumber)
						|| !string.IsNullOrEmpty(_searchParams.SearchFields.Mrn)
						|| !string.IsNullOrEmpty(_searchParams.SearchFields.HealthcardNumber)
						|| !string.IsNullOrEmpty(_searchParams.SearchFields.FamilyName)
						|| !string.IsNullOrEmpty(_searchParams.SearchFields.GivenName)
						|| _searchParams.SearchFields.OrderingPractitionerRef != null
						|| _searchParams.SearchFields.ProcedureTypeRef != null
						|| _searchParams.SearchFields.FromDate != null
						|| _searchParams.SearchFields.UntilDate != null);
			}
		}

		public bool ComponentEnabled
		{
			get { return _activeSearchHandler == null ? false : _activeSearchHandler.SearchEnabled; }
		}

		public void Search()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			if (_activeSearchHandler != null)
				_activeSearchHandler.SearchParams = _searchParams; ;

			// always turn the validation errors off after a successful search
			this.ShowValidation(false);

			if (!_keepOpen)
			{
				this.Host.Exit();
			}
		}

		public void Clear()
		{
			this.AccessionNumber = null;
			this.Mrn = null;
			this.HealthcardNumber = null;
			this.FamilyName = null;
			this.GivenName = null;
			this.OrderingPractitioner = null;
			this.ProcedureType = null;
			this.FromDate = null;
			this.UntilDate = null;
		}
		
		#endregion

		private static void Workspaces_ItemActivationChanged(object sender, ItemEventArgs<Workspace> e)
		{
			UpdateDisplay();
		}

		private static ISearchDataHandler GetActiveWorkspaceSearchHandler()
		{
			if (_searchComponentShelf == null)
				return null;

			Workspace activeWorkspace = _searchComponentShelf.DesktopWindow.ActiveWorkspace;
			if (activeWorkspace != null && activeWorkspace.Component is ISearchDataHandler)
				return (ISearchDataHandler)(activeWorkspace.Component);

			return null;
		}

		private static void UpdateDisplay()
		{
			if (_activeSearchHandler != null)
				_activeSearchHandler.SearchEnabledChanged -= OnSearchEnabledChanged;

			_activeSearchHandler = GetActiveWorkspaceSearchHandler();
			if (_activeSearchHandler != null)
				_activeSearchHandler.SearchEnabledChanged += OnSearchEnabledChanged;

			Instance.NotifyPropertyChanged("ComponentEnabled");
			Instance.NotifyPropertyChanged("SearchEnabled");
		}

		private static void OnSearchEnabledChanged(object sender, EventArgs e)
		{
			Instance.NotifyPropertyChanged("ComponentEnabled");
			Instance.NotifyPropertyChanged("SearchEnabled");
		}
	}
}
