using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class ProtocollingComponentWorklistItemManager
	{
		#region Private fields

		private ReportingWorklistItem _worklistItem;
		private event EventHandler _worklistItemChanged;

		private readonly ProtocollingComponentMode _componentMode;
		private readonly string _folderName;
		private readonly EntityRef _worklistRef;
		private readonly string _worklistClassName;
		private int _completedItems = 0;
		private bool _isInitialItem = true;

		private readonly List<ReportingWorklistItem> _skippedItems;
		private readonly Stack<ReportingWorklistItem> _worklistCache;

		private bool _protocolNextItem;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>
		/// Only one of worklistRef or worklistClassName should be specified.  worklistRef will take precedence if both are provided.
		/// </remarks>
		/// <param name="worklistItem">The initial worklist item for the <see cref="ProtocollingComponent"/></param>
		/// <param name="folderName">Folder system name, displayed in status text</param>
		/// <param name="worklistRef">An <see cref="EntityRef"/> for the folder from which additional worklist items should be loaded.</param>
		/// <param name="worklistClassName">A name for the folder class from which additional worklist items should be loaded.</param>
		/// <param name="mode"></param>
		public ProtocollingComponentWorklistItemManager(ReportingWorklistItem worklistItem, ProtocollingComponentMode mode, string folderName, EntityRef worklistRef, string worklistClassName)
		{
			_worklistItem = worklistItem;
			_componentMode = mode;
			_folderName = folderName;
			_worklistRef = worklistRef;
			_worklistClassName = worklistClassName;

			_protocolNextItem = this.ProtocolNextItemEnabled;

			_skippedItems = new List<ReportingWorklistItem>();
			_worklistCache = new Stack<ReportingWorklistItem>();
		}

		#endregion

		/// <summary>
		/// The current <see cref="ReportingWorklistItem"/>
		/// </summary>
		public ReportingWorklistItem WorklistItem
		{
			get { return _worklistItem; }
		}

		/// <summary>
		/// Used to request the next <see cref="ReportingWorklistItem"/> to be loaded.
		/// </summary>
		/// <param name="result">Indicates whether previous item was completed or skipped.</param>
		public void ProceedToNextWorklistItem(WorklistItemCompletedResult result)
		{
			if (result == WorklistItemCompletedResult.Completed)
				_completedItems++;
			else if (result == WorklistItemCompletedResult.Skipped)
				_skippedItems.Add(_worklistItem);

			_isInitialItem = false;

			if (_protocolNextItem)
			{
				if (_worklistCache.Count == 0)
				{
					RefreshWorklistItemCache();
				}

				_worklistItem = _worklistCache.Count > 0 ? _worklistCache.Pop() : null;
			}
			else
			{
				_worklistItem = null;
			}

			EventsHelper.Fire(_worklistItemChanged, this, EventArgs.Empty);
		}

		/// <summary>
		/// Specify a list of <see cref="ReportingWorklistItem"/> that shouldn't be returned from <see cref="ProceedToNextWorklistItem"/>
		/// </summary>
		/// <param name="protocols"></param>
		public void IgnoreWorklistItems(List<ReportingWorklistItem> protocols)
		{
			_skippedItems.AddRange(protocols);
			RefreshWorklistItemCache();
		}

		/// <summary>
		/// Fired when the next worklist item is available.
		/// </summary>
		public event EventHandler WorklistItemChanged
		{
			add { _worklistItemChanged += value; }
			remove { _worklistItemChanged -= value; }
		}

		public bool CanSkipItem
		{
			get { return _protocolNextItem && this.ProtocolNextItemEnabled; }
		}

		public bool ShouldUnclaim
		{
			get { return _componentMode == ProtocollingComponentMode.Assign; }
		}

		#region Presentation Model

		public string StatusText
		{
			get
			{
				string status = string.Format(SR.FormatProtocolFolderName, _folderName);

				if (!_isInitialItem)
				{
					status = status + string.Format(SR.FormatProtocolStatusText, _worklistCache.Count, _completedItems, _skippedItems.Count);
				}

				return status;
			}
		}

		public bool ShowStatusText
		{
			get { return this.CanProtocolMultipleItems; }
		}

		/// <summary>
		/// Specifies if the next <see cref="ReportingWorklistItem"/> should be protocolled
		/// </summary>
		public bool ProtocolNextItem
		{
			get { return _protocolNextItem; }
			set { _protocolNextItem = value; }
		}

		public bool ProtocolNextItemEnabled
		{
			get { return this.CanProtocolMultipleItems; }
		}

		#endregion

		private bool CanProtocolMultipleItems
		{
			get { return _componentMode == ProtocollingComponentMode.Assign && (_worklistRef != null || _worklistClassName != null); }
		}

		private void RefreshWorklistItemCache()
		{
			_worklistCache.Clear();

			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					QueryWorklistRequest request;
					if (_worklistRef != null)
					{
						request = new QueryWorklistRequest(_worklistRef, true, true, DowntimeRecovery.InDowntimeRecoveryMode);
					}
					else
					{
						request = new QueryWorklistRequest(_worklistClassName, true, true, DowntimeRecovery.InDowntimeRecoveryMode);
					}

					QueryWorklistResponse<ReportingWorklistItem> response = service.QueryWorklist(request);

					foreach (ReportingWorklistItem item in response.WorklistItems)
					{
						if (WorklistItemWasPreviouslySkipped(item) == false)
						{
							_worklistCache.Push(item);
						}
					}
				});
		}

		private bool WorklistItemWasPreviouslySkipped(ReportingWorklistItem item)
		{
			return CollectionUtils.Contains(_skippedItems,
				delegate(ReportingWorklistItem skippedItem)
				{
					return skippedItem.AccessionNumber == item.AccessionNumber;
				});
		}
	}
}