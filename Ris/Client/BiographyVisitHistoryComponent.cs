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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="BiographyVisitHistoryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class BiographyVisitHistoryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PatientVisitHistoryComponent class
	/// </summary>
	[AssociateView(typeof(BiographyVisitHistoryComponentViewExtensionPoint))]
	public class BiographyVisitHistoryComponent : ApplicationComponent
	{
		private readonly EntityRef _patientRef;
		private readonly VisitListTable _visitList;
		private VisitListItem _selectedVisit;

		private VisitDetailViewComponent _visitDetailComponent;
		private ChildComponentHost _visitDetailComponentHost;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyVisitHistoryComponent(EntityRef patientRef)
		{
			_patientRef = patientRef;
			_visitList = new VisitListTable();
		}

		public override void Start()
		{
			_visitDetailComponent = new BiographyVisitDetailViewComponent();
			_visitDetailComponentHost = new ChildComponentHost(this.Host, _visitDetailComponent);
			_visitDetailComponentHost.StartComponent();

			LoadVisits();

			base.Start();
		}

		public override void Stop()
		{
			if (_visitDetailComponentHost != null)
			{
				_visitDetailComponentHost.StopComponent();
				_visitDetailComponentHost = null;
			}

			base.Stop();
		}

		#region Presentation Model

		public ITable Visits
		{
			get { return _visitList; }
		}

		public ISelection SelectedVisit
		{
			get { return new Selection(_selectedVisit); }
			set
			{
				var newSelection = (VisitListItem)value.Item;
				if (_selectedVisit == newSelection)
					return;

				_selectedVisit = newSelection;

				UpdatePages();
				NotifyAllPropertiesChanged();
			}
		}

		public ApplicationComponentHost VisitDetailComponentHost
		{
			get { return _visitDetailComponentHost; }
		}

		#endregion

		private void UpdatePages()
		{
			_visitDetailComponent.Context = _selectedVisit == null 
				? null 
				: new VisitDetailViewComponent.VisitContext(_selectedVisit.VisitRef);
		}

		private void LoadVisits()
		{
			Async.CancelPending(this);

			if (_patientRef == null)
				return;

			Async.Request(
				this,
				(IBrowsePatientDataService service) =>
				{
					var request = new GetDataRequest
					{
						ListVisitsRequest = new ListVisitsRequest(_patientRef)
					};

					return service.GetData(request);
				},
				response =>
				{
					_visitList.Items.Clear();
					_visitList.Items.AddRange(response.ListVisitsResponse.Visits);
				});
		}
	}
}