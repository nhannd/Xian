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

		//public static void EnsureCorrectSearchComponent()
		//{
		//    _componentManager.EnsureProperSearchComponent();
		//}

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
