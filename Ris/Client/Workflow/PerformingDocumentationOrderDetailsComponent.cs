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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="PerformingDocumentationOrderDetailsComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class PerformingDocumentationOrderDetailsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PerformingDocumentationOrderDetailsComponent class
	/// </summary>
	[AssociateView(typeof(PerformingDocumentationOrderDetailsComponentViewExtensionPoint))]
	public class PerformingDocumentationOrderDetailsComponent : ApplicationComponent
	{
		#region ProtocolSummaryComponent class

		public class ProtocolSummaryComponent : OrderDetailViewComponent
		{
			public ProtocolSummaryComponent(IPerformingDocumentationContext context)
				: base(context.OrderRef)
			{
			}

			protected override string PageUrl
			{
				get { return WebResourcesSettings.Default.PerformingOrderDetailPageUrl; }
			}
		}

		#endregion

		private OrderNoteSummaryComponent _orderNoteComponent;
		private ChildComponentHost _orderNotesComponentHost;
		private ChildComponentHost _protocolSummaryComponentHost;
		private ChildComponentHost _additionalInfoComponentHost;
		private OrderAdditionalInfoComponent _orderAdditionalInfoComponent;

		private readonly IPerformingDocumentationContext _context;
		private readonly DataContractBase _worklistItem;

		/// <summary>
		/// Constructor
		/// </summary>
		public PerformingDocumentationOrderDetailsComponent(IPerformingDocumentationContext context, DataContractBase worklistItem)
		{
			_context = context;
			_worklistItem = worklistItem;
		}

		public override void Start()
		{
			_orderNoteComponent = new OrderNoteSummaryComponent(OrderNoteCategory.General);
			_orderNoteComponent.Notes = _context.OrderNotes;
			_orderNotesComponentHost = new ChildComponentHost(this.Host, _orderNoteComponent);
			_orderNotesComponentHost.StartComponent();

			_protocolSummaryComponentHost = new ChildComponentHost(this.Host, new ProtocolSummaryComponent(_context));
			_protocolSummaryComponentHost.StartComponent();

			_orderAdditionalInfoComponent = new OrderAdditionalInfoComponent();
			_orderAdditionalInfoComponent.OrderExtendedProperties = _context.OrderExtendedProperties;
			_orderAdditionalInfoComponent.HealthcareContext = _worklistItem;
			_additionalInfoComponentHost = new ChildComponentHost(this.Host, _orderAdditionalInfoComponent);
			_additionalInfoComponentHost.StartComponent();

			base.Start();
		}

        public override void Stop()
        {
            if (_orderNotesComponentHost != null)
            {
                _orderNotesComponentHost.StopComponent();
                _orderNotesComponentHost = null;
            }

            if (_protocolSummaryComponentHost != null)
            {
                _protocolSummaryComponentHost.StopComponent();
                _protocolSummaryComponentHost = null;
            }

            if (_additionalInfoComponentHost != null)
            {
                _additionalInfoComponentHost.StopComponent();
                _additionalInfoComponentHost = null;
            }

            base.Stop();
        }

		public ApplicationComponentHost AdditionalInfoHost
		{
			get { return _additionalInfoComponentHost; }
		}

		public ApplicationComponentHost ProtocolHost
		{
			get { return _protocolSummaryComponentHost; }
		}

		public ApplicationComponentHost NotesHost
		{
			get { return _orderNotesComponentHost; }
		}

		internal void SaveData()
		{
			_orderAdditionalInfoComponent.SaveData();
		}
	}
}
