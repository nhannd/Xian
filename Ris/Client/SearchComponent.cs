#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class WorklistSearchParams : SearchParams<WorklistItemTextQueryRequest.AdvancedSearchFields>
	{
		public WorklistSearchParams(string textSearch)
			: base(textSearch)
		{
		}

		/// <summary>
		/// Constructor for advance search
		/// </summary>
		public WorklistSearchParams(WorklistItemTextQueryRequest.AdvancedSearchFields searchFields)
			: base(searchFields)
		{
		}
	}

	[ExtensionPoint]
	public class SearchComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(SearchComponentViewExtensionPoint))]
	public class SearchComponent : SearchComponentBase
	{
		private DiagnosticServiceSummary _selectedDiagnosticService;
		private DiagnosticServiceLookupHandler _diagnosticServiceLookupHandler;

		private ProcedureTypeSummary _selectedProcedureType;
		private ProcedureTypeLookupHandler _procedureTypeLookupHandler;
		private ExternalPractitionerSummary _selectedOrderingPractitioner;
		private ExternalPractitionerLookupHandler _orderingPractitionerLookupHandler;

		private static readonly SearchComponentManager<SearchComponent> _componentManager = new SearchComponentManager<SearchComponent>();

		public SearchComponent()
			: base(new WorklistSearchParams(new WorklistItemTextQueryRequest.AdvancedSearchFields()))
		{
		}

		public static Shelf Launch(IDesktopWindow desktopWindow)
		{
			return _componentManager.Launch(desktopWindow);
		}

		public override void Start()
		{
			_orderingPractitionerLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);
			_procedureTypeLookupHandler = new ProcedureTypeLookupHandler(this.Host.DesktopWindow);
			_diagnosticServiceLookupHandler = new DiagnosticServiceLookupHandler(this.Host.DesktopWindow);

			base.Start();
		}

		protected override ISearchDataHandler ActiveSearchHandler
		{
			get { return _componentManager.ActiveSearchHandler; }
		}

		#region Public Member

		public string AccessionNumber
		{
			get { return this.SearchParams.SearchFields.AccessionNumber; }
			set
			{
				this.SearchParams.SearchFields.AccessionNumber = value;
				NotifyPropertyChanged("AccessionNumber");
			}
		}

		public string Mrn
		{
			get { return this.SearchParams.SearchFields.Mrn; }
			set
			{
				this.SearchParams.SearchFields.Mrn = value;
				NotifyPropertyChanged("Mrn");
			}
		}

		public string HealthcardNumber
		{
			get { return this.SearchParams.SearchFields.HealthcardNumber; }
			set
			{
				this.SearchParams.SearchFields.HealthcardNumber = value;
				NotifyPropertyChanged("HealthcardNumber");
			}
		}

		public string FamilyName
		{
			get { return this.SearchParams.SearchFields.FamilyName; }
			set
			{
				this.SearchParams.SearchFields.FamilyName = value;
				NotifyPropertyChanged("FamilyName");
			}
		}

		public string GivenName
		{
			get { return this.SearchParams.SearchFields.GivenName; }
			set
			{
				this.SearchParams.SearchFields.GivenName = value;
				NotifyPropertyChanged("GivenName");
			}
		}
		public ExternalPractitionerSummary OrderingPractitioner
		{
			get { return _selectedOrderingPractitioner; }
			set
			{
				_selectedOrderingPractitioner = value;
				this.SearchParams.SearchFields.OrderingPractitionerRef = value == null ? null : value.PractitionerRef;
				NotifyPropertyChanged("OrderingPractitioner");
			}
		}

		public ILookupHandler OrderingPractitionerLookupHandler
		{
			get { return _orderingPractitionerLookupHandler; }
		}

		public DiagnosticServiceSummary DiagnosticService
		{
			get { return _selectedDiagnosticService; }
			set
			{
				_selectedDiagnosticService = value;
				this.SearchParams.SearchFields.DiagnosticServiceRef = value == null ? null : value.DiagnosticServiceRef;
				NotifyPropertyChanged("DiagnosticService");
			}
		}

		public ILookupHandler DiagnosticServiceLookupHandler
		{
			get { return _diagnosticServiceLookupHandler; }
		}

		public ProcedureTypeSummary ProcedureType
		{
			get { return _selectedProcedureType; }
			set
			{
				_selectedProcedureType = value;
				this.SearchParams.SearchFields.ProcedureTypeRef = value == null ? null : value.ProcedureTypeRef;
				NotifyPropertyChanged("ProcedureType");
			}
		}

		public ILookupHandler ProcedureTypeLookupHandler
		{
			get { return _procedureTypeLookupHandler; }
		}

		public DateTime? FromDate
		{
			get { return this.SearchParams.SearchFields.FromDate; }
			set
			{
				this.SearchParams.SearchFields.FromDate = value;
				NotifyPropertyChanged("FromDate");
			}
		}

		public DateTime? UntilDate
		{
			get { return this.SearchParams.SearchFields.UntilDate; }
			set
			{
				this.SearchParams.SearchFields.UntilDate = value;
				NotifyPropertyChanged("UntilDate");
			}
		}

		#endregion

		#region SearchComponentBase overrides

		public override bool HasNonEmptyFields
		{
			get
			{
				return !string.IsNullOrEmpty(this.SearchParams.SearchFields.AccessionNumber)
						|| !string.IsNullOrEmpty(this.SearchParams.SearchFields.Mrn)
						|| !string.IsNullOrEmpty(this.SearchParams.SearchFields.HealthcardNumber)
						|| !string.IsNullOrEmpty(this.SearchParams.SearchFields.FamilyName)
						|| !string.IsNullOrEmpty(this.SearchParams.SearchFields.GivenName)
						|| this.SearchParams.SearchFields.OrderingPractitionerRef != null
						|| this.SearchParams.SearchFields.DiagnosticServiceRef != null
						|| this.SearchParams.SearchFields.ProcedureTypeRef != null
						|| this.SearchParams.SearchFields.FromDate != null
						|| this.SearchParams.SearchFields.UntilDate != null;
			}
		}

		public override void Clear()
		{
			this.AccessionNumber = null;
			this.Mrn = null;
			this.HealthcardNumber = null;
			this.FamilyName = null;
			this.GivenName = null;
			this.OrderingPractitioner = null;
			this.DiagnosticService = null;
			this.ProcedureType = null;
			this.FromDate = null;
			this.UntilDate = null;
		}
		
		#endregion

		private WorklistSearchParams SearchParams
		{
			get { return (WorklistSearchParams) _searchParams; }
		}
	}
}
