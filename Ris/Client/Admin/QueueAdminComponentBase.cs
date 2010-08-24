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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Admin
{
	public interface IWorkQueueToolContext<TQueueItemDetail> : IToolContext
	{
		IDesktopWindow DesktopWindow { get; }
		ClickHandlerDelegate DefaultAction { get; set; }

		TQueueItemDetail SelectedWorkQueueItem { get; }
		event EventHandler SelectedWorkQueueItemChanged;

		void Refresh();
	}

	public abstract class QueueAdminComponentBase<TQueueItemSummary, TQueueItemDetail> : ApplicationComponent
		where TQueueItemSummary : DataContractBase
		where TQueueItemDetail : DataContractBase
	{
		protected class WorkQueueItemPreviewComponent : DHtmlComponent
		{
			private readonly QueueAdminComponentBase<TQueueItemSummary, TQueueItemDetail> _owner;

			public WorkQueueItemPreviewComponent(QueueAdminComponentBase<TQueueItemSummary, TQueueItemDetail> owner)
			{
				_owner = owner;
			}

			public override void Start()
			{
				_owner.SelectedHL7QueueItemChanged += Refresh;
				SetUrl(_owner.PreviewPageUrl);
				base.Start();
			}

			public override void Stop()
			{
				_owner.SelectedHL7QueueItemChanged -= Refresh;
				base.Stop();
			}

			private void Refresh(object sender, EventArgs e)
			{
				NotifyAllPropertiesChanged();
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _owner.SelectedWorkQueueItem;
			}
		}

		protected class WorkQueueToolContext : ToolContext, IWorkQueueToolContext<TQueueItemDetail>
		{
			private readonly QueueAdminComponentBase<TQueueItemSummary, TQueueItemDetail> _component;

			public WorkQueueToolContext(QueueAdminComponentBase<TQueueItemSummary, TQueueItemDetail> component)
			{
				_component = component;
			}

			#region IHL7QueueToolContext Members

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public ClickHandlerDelegate DefaultAction
			{
				get { return _component._defaultAction; }
				set { _component._defaultAction = value; }
			}

			public TQueueItemDetail SelectedWorkQueueItem
			{
				get { return _component.SelectedWorkQueueItem; }
			}

			public event EventHandler SelectedWorkQueueItemChanged
			{
				add { _component.SelectedHL7QueueItemChanged += value; }
				remove { _component.SelectedHL7QueueItemChanged -= value; }
			}

			public void Refresh()
			{
				_component.Refresh();
			}

			#endregion
		}

		class DummyItem
		{
			private readonly string _displayString;

			public DummyItem(string displayString)
			{
				_displayString = displayString;
			}

			public override string ToString()
			{
				return _displayString;
			}
		}

		private IPagingController<TQueueItemSummary> _pagingController;
		private PagingActionModel<TQueueItemSummary> _pagingActionHandler;

		private TQueueItemDetail _selectedWorkQueueItem;
		private event EventHandler _selectedWorkQueueItemChanged;

		private readonly Table<TQueueItemSummary> _queue;
		private readonly ToolSet _toolSet;
		private ClickHandlerDelegate _defaultAction;

		private DHtmlComponent _previewComponent;
		private ApplicationComponentHost _previewComponentHost;

		private static readonly object _nullFilterItem = new DummyItem(SR.DummyItemNone);
		private static readonly int _pageSize = 50;

		protected QueueAdminComponentBase(Table<TQueueItemSummary> queue, IExtensionPoint extensionPoint)
		{
			_queue = queue;
			_toolSet = new ToolSet(extensionPoint, new WorkQueueToolContext(this));
		}

		public abstract string PreviewPageUrl { get; }
		public abstract void InitialiseFormData();
		public abstract void Refresh();
		public abstract IList<TQueueItemSummary> GetSummaryItemsPage(int firstRow, int maxRows);
		public abstract TQueueItemDetail GetQueueItemDetail(TQueueItemSummary queueItemSummary);

		public override void Start()
		{
			InitialisePaging();
			InitialiseFormData();

			_previewComponent = new WorkQueueItemPreviewComponent(this);
			_previewComponentHost = new ChildComponentHost(this.Host, _previewComponent);
			_previewComponentHost.StartComponent();

			base.Start();
		}

		public override void Stop()
		{
			if (_previewComponentHost != null)
			{
				_previewComponentHost.StopComponent();
				_previewComponentHost = null;
			}

			base.Stop();
		}

		private void InitialisePaging()
		{
			_pagingController = new PagingController<TQueueItemSummary>(
				_pageSize, 
				delegate(int firstrow, int maxrows, Action<IList<TQueueItemSummary>> resulthandler)
				{
					_queue.Items.Clear();

					IList<TQueueItemSummary> hl7QueueItems = null;
					try
					{
						hl7QueueItems = GetSummaryItemsPage(firstrow, maxrows);
					}
					catch (Exception e)
					{
						ExceptionHandler.Report(e, Host.DesktopWindow);
					}

					resulthandler(hl7QueueItems);
				});
			_pagingController.PageChanged += PagingControllerPageChangedEventHandler;

			_pagingActionHandler = new PagingActionModel<TQueueItemSummary>(_pagingController, Host.DesktopWindow);
		}


		private void PagingControllerPageChangedEventHandler(object sender, PageChangedEventArgs<TQueueItemSummary> e)
		{
			_queue.Items.AddRange(e.Items);
		}

		protected void GetFirstPage()
		{
			_pagingController.GetFirst();
		}

		public object NullFilterItem
		{
			get { return _nullFilterItem; }
		}

		public Table<TQueueItemSummary> Queue
		{
			get { return _queue; }
		}

		public ApplicationComponentHost PreviewComponentHost
		{
			get { return _previewComponentHost; }
		}

		public ActionModelNode MenuModel
		{
			get
			{
				return ActionModelRoot.CreateModel(this.GetType().FullName, "workqueue-contextmenu", _toolSet.Actions);
			}
		}

		public ActionModelNode ToolbarModel
		{
			get
			{
				ActionModelNode node = ActionModelRoot.CreateModel(this.GetType().FullName, "workqueue-toolbar", _toolSet.Actions);
				node.Merge(_pagingActionHandler);
				return node;
			}
		}

		public void SetSelectedItem(ISelection selection)
		{
			var selectedSummaryTableItem = selection.Item as TQueueItemSummary;

			_selectedWorkQueueItem = selectedSummaryTableItem == null 
				? null 
				: GetQueueItemDetail(selectedSummaryTableItem);

			EventsHelper.Fire(_selectedWorkQueueItemChanged, this, EventArgs.Empty);
		}

		#region WorkQueueToolContext Helpers

		public TQueueItemDetail SelectedWorkQueueItem
		{
			get { return _selectedWorkQueueItem; }
		}

		public event EventHandler SelectedHL7QueueItemChanged
		{
			add { _selectedWorkQueueItemChanged += value; }
			remove { _selectedWorkQueueItemChanged -= value; }
		}

		public override IActionSet ExportedActions
		{
			get { return _toolSet.Actions; }
		}

		#endregion
	}
}