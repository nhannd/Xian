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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
	public class WorkQueueSummaryTable : Table<WorkQueueItemSummary>
	{
		public WorkQueueSummaryTable()
		{
			this.Columns.Add(new DateTimeTableColumn<WorkQueueItemSummary>(SR.ColumnCreationTime,
				delegate(WorkQueueItemSummary item) { return item.CreationTime; }));

			this.Columns.Add(new DateTimeTableColumn<WorkQueueItemSummary>(SR.ColumnScheduledTime,
				delegate(WorkQueueItemSummary item) { return item.ScheduledTime; }));

			this.Columns.Add(new DateTimeTableColumn<WorkQueueItemSummary>(SR.ColumnExpirationTime,
				delegate(WorkQueueItemSummary item) { return item.ExpirationTime; }));

			this.Columns.Add(new TableColumn<WorkQueueItemSummary, string>(SR.ColumnUser,
				delegate(WorkQueueItemSummary item) { return item.User; }));

			this.Columns.Add(new TableColumn<WorkQueueItemSummary, string>(SR.ColumnType,
				delegate(WorkQueueItemSummary item) { return item.Type; }));

			this.Columns.Add(new TableColumn<WorkQueueItemSummary, string>(SR.ColumnStatus,
				delegate(WorkQueueItemSummary item) { return item.Status.Value; }));

			this.Columns.Add(new DateTimeTableColumn<WorkQueueItemSummary>(SR.ColumnProcessedTime,
				delegate(WorkQueueItemSummary item) { return item.ProcessedTime; }));

			this.Columns.Add(new TableColumn<WorkQueueItemSummary, int>(SR.ColumnFailureCount,
				delegate(WorkQueueItemSummary item) { return item.FailureCount; }));

			this.Columns.Add(new TableColumn<WorkQueueItemSummary, string>(SR.ColumnFailureDescription,
				delegate(WorkQueueItemSummary item) { return item.FailureDescription; }));

		}
	}

	/// <summary>
	/// Extension point for views onto <see cref="WorkQueueSummaryComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class WorkQueueSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// WorkQueueSummaryComponent class.
	/// </summary>
	[AssociateView(typeof(WorkQueueSummaryComponentViewExtensionPoint))]
	public class WorkQueueSummaryComponent : SummaryComponentBase<WorkQueueItemSummary, WorkQueueSummaryTable, ListWorkQueueItemsRequest>
	{
		private readonly EnumValueInfo _any = new EnumValueInfo("__any", "(Any)");

		private DateTime? _startTime;
		private DateTime? _endTime;
		private string _user;

		private string _type;
		private readonly List<string> _typeChoices = new List<string>();

		private EnumValueInfo _status;
		private readonly List<EnumValueInfo> _statusChoices = new List<EnumValueInfo>();

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			Platform.GetService<IWorkQueueAdminService>(
					delegate(IWorkQueueAdminService service)
					{
						GetWorkQueueFormDataResponse response = service.GetWorkQueueFormData(new GetWorkQueueFormDataRequest());
						_typeChoices.AddRange(response.Types);
						_typeChoices.Insert(0, _any.Value);
						_type = CollectionUtils.FirstElement(_typeChoices);
						_statusChoices.AddRange(response.Statuses);
						_statusChoices.Insert(0, _any);
						_status = CollectionUtils.FirstElement(_statusChoices);
					});

			base.Start();
		}

		#region Presentation Model

		public DateTime? StartTime
		{
			get { return _startTime; }
			set { _startTime = value; }
		}

		public DateTime? EndTime
		{
			get { return _endTime; }
			set { _endTime = value; }
		}

		public string User
		{
			get { return _user; }
			set { _user = value; }
		}

		public string Type
		{
			get { return _type; }
			set { _type = value; }
		}

		public List<string> TypeChoices
		{
			get { return _typeChoices; }
		}

		public EnumValueInfo Status
		{
			get { return _status; }
			set { _status = value; }
		}

		public IList StatusChoices
		{
			get { return _statusChoices; }
		}

		public void Clear()
		{
			this.StartTime = null;
			this.EndTime = null;
			this.User = null;
			this.Type = _any.Value;
			this.Status = _any;
			NotifyPropertyChanged("Status");
		}

		#endregion

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.Visible = false;
			model.Edit.Visible = false;
			//model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.System.WorkQueue);
			//model.ToggleActivation.Visible = false;
		}

		protected override IList<WorkQueueItemSummary> ListItems(ListWorkQueueItemsRequest request)
		{
			ListWorkQueueItemsResponse listResponse = null;
			Platform.GetService<IWorkQueueAdminService>(
				delegate(IWorkQueueAdminService service)
				{
					request.StartTime = _startTime;
					request.EndTime = _endTime;
					request.User = _user;
					request.Type = Equals(_type, _any.Value) ? null : _type;
					request.Status = _status.Code == _any.Code ? null : _status;
					listResponse = service.ListWorkQueueItems(request);
				});
			return listResponse.WorkQueueItems;

		}

		protected override bool AddItems(out IList<WorkQueueItemSummary> addedItems)
		{
			addedItems = new List<WorkQueueItemSummary>();
			return true;
		}

		protected override bool EditItems(IList<WorkQueueItemSummary> items, out IList<WorkQueueItemSummary> editedItems)
		{
			editedItems = new List<WorkQueueItemSummary>();
			return true;
		}

		protected override bool DeleteItems(
			IList<WorkQueueItemSummary> items, out IList<WorkQueueItemSummary> deletedItems, out string failureMessage)
		{
			throw new NotImplementedException();
		}

		protected override bool IsSameItem(WorkQueueItemSummary x, WorkQueueItemSummary y)
		{
			return false;
		}
	}
}
