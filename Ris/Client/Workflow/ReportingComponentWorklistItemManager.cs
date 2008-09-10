using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public enum WorklistItemCompletedResult
	{
		Completed,
		Skipped,
		Invalid
	}

	/// <summary>
	/// Manages retrieval of worklist items for the <see cref="ReportingComponent"/>
	/// </summary>
	public class ReportingComponentWorklistItemManager
	{
		/// <summary>
		/// Dictates the mode the mode of operation <see cref="ReportingComponent"/>.  The mode impacts availability of "Report Next Order" checkbox
		/// and indicates if the worklist item needs to be "unclaimed"
		/// </summary>
		private enum ReportingComponentMode
		{
			/// <summary>
			/// "Report Next Order" checkbox enabled.  Worklist item attempted to be claimed.
			/// </summary>
			Create,

			/// <summary>
			/// "Report Next Order" checkbox disabled.  Worklist item attempted to be claimed.
			/// </summary>
			CreateAddendum,

			/// <summary>
			/// "Report Next Order" checkbox disabled.  Worklist item not claimed.
			/// </summary>
			Edit,

			/// <summary>
			/// Read-only: "Report Next Order" checkbox disabled.  Worklist item not claimed.
			/// </summary>
			Review,

			/// <summary>
			/// "Report Next Order" checkbox enabled.  Worklist item is not unclaimed.
			/// </summary>
			Verify
		}

		#region Private fields

		private ReportingWorklistItem _worklistItem;
		private event EventHandler _worklistItemChanged;

		private readonly ReportingComponentMode _componentMode;
		private readonly string _folderName;
		private readonly EntityRef _worklistRef;
		private readonly string _worklistClassName;
		private int _completedItems = 0;
		private bool _isInitialItem = true;

		private readonly List<ReportingWorklistItem> _skippedItems;
		private readonly Stack<ReportingWorklistItem> _worklistCache;

		private bool _reportNextItem;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>
		/// Only one of worklistRef or worklistClassName should be specified.  worklistRef will take precedence if both are provided.
		/// </remarks>
		/// <param name="worklistItem">The initial worklist item for the <see cref="ReportingComponent"/></param>
		/// <param name="folderName">Folder system name, displayed in status text</param>
		/// <param name="worklistRef">An <see cref="EntityRef"/> for the folder from which additional worklist items should be loaded.</param>
		/// <param name="worklistClassName">A name for the folder class from which additional worklist items should be loaded.</param>
		public ReportingComponentWorklistItemManager(ReportingWorklistItem worklistItem, string folderName, EntityRef worklistRef, string worklistClassName)
		{
			_worklistItem = worklistItem;
			_componentMode = GetMode(_worklistItem);
			_folderName = folderName;
			_worklistRef = worklistRef;
			_worklistClassName = worklistClassName;

			_reportNextItem = this.ReportNextItemEnabled;

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

		/// <summary>
		/// Specify a list of <see cref="ReportingWorklistItem"/> that shouldn't be returned from <see cref="ProceedToNextWorklistItem"/>
		/// </summary>
		/// <param name="interpretations"></param>
		public void IgnoreWorklistItems(List<ReportingWorklistItem> interpretations)
		{
			_skippedItems.AddRange(interpretations);
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
			get { return _reportNextItem && this.ReportNextItemEnabled; }
		}

		public bool ShouldUnclaim
		{
			get { return _componentMode == ReportingComponentMode.Create || _componentMode == ReportingComponentMode.CreateAddendum; }
		}

		#region Presentation Model

		/// <summary>
		/// A string indicating the name of the source folder system and counts of available, completed and skipped items.
		/// </summary>
		public string StatusText
		{
			get
			{
				string status = string.Format(SR.FormatReportingFolderName, _folderName);

				if (!_isInitialItem)
				{
					status = status + string.Format(SR.FormatReportingStatusText, _worklistCache.Count, _completedItems, _skippedItems.Count);
				}

				return status;
			}
		}

		public bool StatusTextVisible
		{
			get { return _componentMode == ReportingComponentMode.Create; }
		}

		/// <summary>
		/// Specifies if the next <see cref="ReportingWorklistItem"/> should be reported
		/// </summary>
		public bool ReportNextItem
		{
			get { return _reportNextItem; }
			set { _reportNextItem = value; }
		}

		public bool ReportNextItemEnabled
		{
			get
			{
				return (_worklistRef != null 
						|| String.Equals(_worklistClassName, WorklistClassNames.ReportingToBeReportedWorklist)
						|| String.Equals(_worklistClassName, WorklistClassNames.ReportingToBeReviewedReportWorklist)) 
					&& (_componentMode == ReportingComponentMode.Create || _componentMode == ReportingComponentMode.Verify);
			}
		}

		#endregion

		#region Private methods

		// Determine the ReportingComponentMode from the ReportingWorklistItem's ProcedureStepName and/or ActivityStatus
		private static ReportingComponentMode GetMode(ReportingWorklistItem item)
		{
			if (item == null)
				return ReportingComponentMode.Review;

			if (item.ProcedureStepName == StepType.Publication)
				return ReportingComponentMode.Review;

			if (item.ProcedureStepName == StepType.Verification)
				return ReportingComponentMode.Verify;

			switch (item.ActivityStatus.Code)
			{
				case StepState.Scheduled:
					return item.IsAddendumStep ? ReportingComponentMode.CreateAddendum : ReportingComponentMode.Create;
				case StepState.InProgress:
					return ReportingComponentMode.Edit;
				default:
					return ReportingComponentMode.Review;
			}
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

		#endregion
	}
}