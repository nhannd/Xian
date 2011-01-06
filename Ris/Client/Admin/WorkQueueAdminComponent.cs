#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
	[ExtensionPoint]
	public class WorkQueueToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	/// <summary>
	/// Extension point for views onto <see cref="WorkQueueAdminComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class WorkQueueAdminComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// WorkQueueAdminComponent class.
	/// </summary>
	[AssociateView(typeof(WorkQueueAdminComponentViewExtensionPoint))]
	public class WorkQueueAdminComponent : QueueAdminComponentBase<WorkQueueItemSummary, WorkQueueItemDetail>
	{
		private List<string> _typeChoices;
		private List<string> _typeSelections;

		private List<EnumValueInfo> _statusChoices;
		private List<EnumValueInfo> _statusSelections;

		public enum TimeFilterOptions
		{
			Scheduled,
			Processed
		}

		private TimeFilterOptions _selectedTimeFilterOption;
		private DateTime? _startTime;
		private DateTime? _endTime;

		private ListWorkQueueItemsRequest _listRequest;

		/// <summary>
		/// Constructor.
		/// </summary>
		public WorkQueueAdminComponent()
			: base(new WorkQueueSummaryTable(), new WorkQueueToolExtensionPoint())
		{
		}

		#region QueueAdminComponentBase overrides

		public override string PreviewPageUrl
		{
			get { return WebResourcesSettings.Default.WorkQueuePreviewPageUrl; }
		}

		public override void InitialiseFormData()
		{
			Platform.GetService<IWorkQueueAdminService>(
				service =>
				{
					var formResponse = service.GetWorkQueueFormData(new GetWorkQueueFormDataRequest());

					_statusChoices = formResponse.Statuses;
					_typeChoices = formResponse.Types;
				});

			SetDefaultFilterOptions();
		}

		public override IList<WorkQueueItemSummary> GetSummaryItemsPage(int firstRow, int maxRows)
		{
			ListWorkQueueItemsResponse listResponse = null;

			Platform.GetService<IWorkQueueAdminService>(
				service =>
				{
					var listRequest = _listRequest;
					listRequest.Page.FirstRow = firstRow;
					listRequest.Page.MaxRows = maxRows;

					listResponse = service.ListWorkQueueItems(listRequest);
				});

			return listResponse.WorkQueueItems;
		}

		public override WorkQueueItemDetail GetQueueItemDetail(WorkQueueItemSummary workQueueItemSummary)
		{
			if (this.SelectedWorkQueueItem != null && workQueueItemSummary.WorkQueueItemRef == this.SelectedWorkQueueItem.WorkQueueItemRef)
				return this.SelectedWorkQueueItem;

			WorkQueueItemDetail detail = null;
			Platform.GetService<IWorkQueueAdminService>(
				service =>
				{
					var request = new LoadWorkQueueItemForEditRequest(workQueueItemSummary.WorkQueueItemRef);
					var response = service.LoadWorkQueueItemForEdit(request);

					if (response != null)
					{
						detail = response.WorkQueueItemDetail;

					}
				});
			return detail;
		}

		public override void Refresh()
		{
			ApplyFilter();
		}

		#endregion

		#region Presentation Model

		#region Type

		public IList<string> TypeChoices
		{
			get { return _typeChoices.ToArray(); }
		}

		public IList SelectedTypes
		{
			get { return _typeSelections; }
			set
			{
				IList<string> list = new TypeSafeListWrapper<string>(value);
				if (!CollectionUtils.Equal(list, _typeSelections, false))
				{
					_typeSelections = new List<string>(list);
				}
			}
		}

		#endregion

		#region Status

		public IList StatusChoices
		{
			get { return _statusChoices; }
		}

		public IList SelectedStatuses
		{
			get { return _statusSelections; }
			set
			{
				IList<EnumValueInfo> list = new TypeSafeListWrapper<EnumValueInfo>(value);
				if (!CollectionUtils.Equal(list, _statusSelections, false))
				{
					_statusSelections = new List<EnumValueInfo>(list);
				}
			}
		}

		#endregion

		#region Time

		public TimeFilterOptions SelectedTimeFilterOption
		{
			get { return _selectedTimeFilterOption; }
			set
			{
				if (_selectedTimeFilterOption == value)
					return;

				_selectedTimeFilterOption = value;
				NotifyPropertyChanged("SelectedTimeFilterOption");
			}
		}

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

		#endregion

		public void ApplyFilter()
		{
			var request = new ListWorkQueueItemsRequest
							{
								Types = _typeSelections,
								Statuses = _statusSelections
							};

			switch (_selectedTimeFilterOption)
			{
				case TimeFilterOptions.Scheduled:
					request.ScheduledStartTime = _startTime;
					request.ScheduledEndTime = _endTime;
					break;
				case TimeFilterOptions.Processed:
					request.ProcessedStartTime = _startTime;
					request.ProcessedEndTime = _endTime;
					break;
			}

			_listRequest = request;
			GetFirstPage();
		}

		public void ClearFilter()
		{
			SetDefaultFilterOptions();
			NotifyPropertyChanged("SelectedTypes");  // Will - intentionally - cause all data-bound UI elements to update
		}

		#endregion

		private void SetDefaultFilterOptions()
		{
			_statusSelections = new List<EnumValueInfo>();
			_typeSelections = new List<string>();

			_startTime = Platform.Time.Date;
			_endTime = Platform.Time.Date.AddDays(1).AddTicks(-1);

			_selectedTimeFilterOption = TimeFilterOptions.Scheduled;
		}
	}
}
