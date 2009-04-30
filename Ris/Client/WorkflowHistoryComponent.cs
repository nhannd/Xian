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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="WorkflowHistoryComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class WorkflowHistoryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// WorkflowHistoryComponent class.
	/// </summary>
	[AssociateView(typeof(WorkflowHistoryComponentViewExtensionPoint))]
	public class WorkflowHistoryComponent : ApplicationComponent
	{
		class ProcedureViewComponent : DHtmlComponent
		{
			private readonly WorkflowHistoryComponent _owner;

			public ProcedureViewComponent(WorkflowHistoryComponent owner)
			{
				_owner = owner;
			}

			public override void Start()
			{
				SetUrl(WebResourcesSettings.Default.WorkflowHistoryPageUrl);
				base.Start();
			}

			public void Refresh()
			{
				NotifyAllPropertiesChanged();
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _owner._selectedProcedure;
			}
		}

		private readonly EntityRef _orderRef;
		private Table<ProcedureDetail> _procedureTable;
		private ProcedureDetail _selectedProcedure;

		private ChildComponentHost _procedureViewComponentHost;

		/// <summary>
		/// Constructor.
		/// </summary>
		public WorkflowHistoryComponent(EntityRef orderRef)
		{
			_orderRef = orderRef;
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_procedureTable = new Table<ProcedureDetail>();
			_procedureTable.Columns.Add(new TableColumn<ProcedureDetail, string>("Procedure",
				delegate(ProcedureDetail item) { return Formatting.ProcedureFormat.Format(item); }));

			_procedureViewComponentHost = new ChildComponentHost(this.Host, new ProcedureViewComponent(this));
			_procedureViewComponentHost.StartComponent();

			Platform.GetService<IBrowsePatientDataService>(
				delegate(IBrowsePatientDataService service)
				{
					GetDataRequest request = new GetDataRequest();
					request.GetOrderDetailRequest = new GetOrderDetailRequest(_orderRef, false, true, false, false, false, false);
					GetDataResponse response = service.GetData(request);
					_procedureTable.Items.AddRange(response.GetOrderDetailResponse.Order.Procedures);
				});

			base.Start();
		}

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		#region Presentation Model

		public ITable ProcedureTable
		{
			get { return _procedureTable; }
		}

		public ISelection SelectedProcedure
		{
			get { return new Selection(_selectedProcedure); }
			set
			{
				if(!Equals(value.Item, _selectedProcedure))
				{
					_selectedProcedure = (ProcedureDetail) value.Item;
					NotifyPropertyChanged("SelectedProcedure");
					((ProcedureViewComponent)_procedureViewComponentHost.Component).Refresh();
				}
			}
		}

		public ApplicationComponentHost ProcedureViewComponentHost
		{
			get { return _procedureViewComponentHost; }
		}

		#endregion
	}
}
