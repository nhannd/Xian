using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public interface IContinuousWorkflowComponentMode
	{
		bool ShouldUnclaim { get; }
		bool ShowStatusText { get; }
		bool CanContinue { get; }
	}

	public abstract class ContinuousWorkflowComponentMode : IContinuousWorkflowComponentMode
	{
		private readonly bool _shouldUnclaim;
		private readonly bool _showStatusText;
		private readonly bool _canContinue;

		protected ContinuousWorkflowComponentMode(bool shouldUnclaim, bool showStatusText, bool canContinue)
		{
			_shouldUnclaim = shouldUnclaim;
			_showStatusText = showStatusText;
			_canContinue = canContinue;
		}

		#region IContinuousWorkflowComponentMode Members

		public bool ShouldUnclaim
		{
			get { return _shouldUnclaim; }
		}

		public bool ShowStatusText
		{
			get { return _showStatusText; }
		}

		public bool CanContinue
		{
			get { return _canContinue; }
		}

		#endregion
	}

	public interface IWorklistItemManager<TWorklistItem>
	{
		/// <summary>
		/// The current <see cref="TWorklistItem"/>
		/// </summary>
		TWorklistItem WorklistItem { get; }

		/// <summary>
		/// Used to request the next <see cref="TWorklistItem"/> to be loaded.
		/// </summary>
		/// <param name="result">Indicates whether previous item was completed or skipped.</param>
		void ProceedToNextWorklistItem(WorklistItemCompletedResult result);

		/// <summary>
		/// Specify a list of <see cref="TWorklistItem"/> that should be excluded from <see cref="ProceedToNextWorklistItem"/>
		/// </summary>
		/// <param name="worklistItems"></param>
		void IgnoreWorklistItems(List<TWorklistItem> worklistItems);

		/// <summary>
		/// Fired when the next worklist item is available.
		/// </summary>
		event EventHandler WorklistItemChanged;

		bool ShouldUnclaim { get; }

		/// <summary>
		/// A string indicating the name of the source folder system and counts of available, completed and skipped items.
		/// </summary>
		string StatusText { get; }

		bool StatusTextVisible { get; }

		/// <summary>
		/// Specifies if the next <see cref="TWorklistItem"/> should be reported
		/// </summary>
		bool ReportNextItem { get; set; }

		/// <summary>
		/// 
		/// </summary>
		bool ReportNextItemEnabled { get; }

		/// <summary>
		/// Specifies if a "Skip" button should be enabled based on mode and value of <see cref="ReportNextItem"/>
		/// </summary>
		bool CanSkipItem { get; }
	}

	public abstract class WorklistItemManager<TWorklistItem, TWorkflowService> : IWorklistItemManager<TWorklistItem>
		where TWorklistItem : WorklistItemSummaryBase
		where TWorkflowService : IWorklistService<TWorklistItem>
	{
		#region Private fields

		private TWorklistItem _worklistItem;
		private event EventHandler _worklistItemChanged;

		private IContinuousWorkflowComponentMode _componentMode;
		private readonly string _folderName;
		private readonly EntityRef _worklistRef;
		private readonly string _worklistClassName;
		private int _completedItems = 0;
		private bool _isInitialItem = true;

		private readonly List<TWorklistItem> _skippedItems;
		private readonly Stack<TWorklistItem> _worklistCache;

		private bool _reportNextItem;

		private bool _isInitialized = false;

		#endregion

		protected abstract IContinuousWorkflowComponentMode GetMode<TWorklistITem>(TWorklistItem worklistItem);
		protected abstract string TaskName { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>
		/// Only one of worklistRef or worklistClassName should be specified.  worklistRef will take precedence if both are provided.
		/// </remarks>
		/// <param name="folderName">Folder system name, displayed in status text</param>
		/// <param name="worklistRef">An <see cref="EntityRef"/> for the folder from which additional worklist items should be loaded.</param>
		/// <param name="worklistClassName">A name for the folder class from which additional worklist items should be loaded.</param>
		public WorklistItemManager(string folderName, EntityRef worklistRef, string worklistClassName)
		{
			_folderName = folderName;
			_worklistRef = worklistRef;
			_worklistClassName = worklistClassName;

			_skippedItems = new List<TWorklistItem>();
			_worklistCache = new Stack<TWorklistItem>();
		}

		public void Initialize(TWorklistItem worklistItem)
		{
			this.Initialize(worklistItem, GetMode<TWorklistItem>(worklistItem));
		}

		public void Initialize(TWorklistItem worklistItem, IContinuousWorkflowComponentMode mode)
		{
			_worklistItem = worklistItem;
			_componentMode = mode;
			_reportNextItem = this.ReportNextItemEnabled;

			_isInitialized = true;
		}

		public TWorklistItem WorklistItem
		{
			get
			{
				if(!_isInitialized)
					throw new Exception("Not initialized.");

				return _worklistItem;
			}
		}

		public void ProceedToNextWorklistItem(WorklistItemCompletedResult result)
		{
			if (result == WorklistItemCompletedResult.Completed)
				_completedItems++;
			else if(result == WorklistItemCompletedResult.Skipped)
				_skippedItems.Add(_worklistItem);

			_isInitialItem = false;

			if (_reportNextItem)
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

		public void IgnoreWorklistItems(List<TWorklistItem> interpretations)
		{
			_skippedItems.AddRange(interpretations);
			RefreshWorklistItemCache();
		}

		public event EventHandler WorklistItemChanged
		{
			add { _worklistItemChanged += value; }
			remove { _worklistItemChanged -= value; }
		}

		public bool ShouldUnclaim
		{
			get { return _componentMode.ShouldUnclaim; }
		}

		#region Presentation Model

		public string StatusText
		{
			get
			{
				string status = string.Format(SR.FormatContinuousWorkflowDescription, this.TaskName, _folderName);

				if (!_isInitialItem)
				{
					status = status + string.Format(SR.FormatReportingStatusText, _worklistCache.Count, _completedItems, _skippedItems.Count);
				}

				return status;
			}
		}

		public bool StatusTextVisible
		{
			get { return _componentMode.ShowStatusText; }
		}

		public bool ReportNextItem
		{
			get { return _reportNextItem; }
			set { _reportNextItem = value; }
		}

		public bool ReportNextItemEnabled
		{
			get { return _componentMode.CanContinue; }
		}

		public bool CanSkipItem
		{
			get { return _reportNextItem && this.ReportNextItemEnabled; }
		}

		#endregion

		#region Private methods

		private void RefreshWorklistItemCache()
		{
			_worklistCache.Clear();

			Platform.GetService<TWorkflowService>(
				delegate(TWorkflowService service)
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

					QueryWorklistResponse<TWorklistItem> response = service.QueryWorklist(request);

					foreach (TWorklistItem item in response.WorklistItems)
					{
						if (WorklistItemWasPreviouslySkipped(item) == false)
						{
							_worklistCache.Push(item);
						}
					}
				});
		}

		private bool WorklistItemWasPreviouslySkipped(TWorklistItem item)
		{
			return CollectionUtils.Contains(_skippedItems,
			                                delegate(TWorklistItem skippedItem)
			                                {
			                                	return skippedItem.AccessionNumber == item.AccessionNumber;
			                                });
		}

		#endregion
	}
}