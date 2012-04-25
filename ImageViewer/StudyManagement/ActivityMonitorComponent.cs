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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using Action = ClearCanvas.Desktop.Actions.Action;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	[ExtensionPoint]
	public sealed class ActivityMonitorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

	[AssociateView(typeof(ActivityMonitorComponentViewExtensionPoint))]
	public class ActivityMonitorComponent : ApplicationComponent
	{
		#region ConnectionState classes

		abstract class ConnectionState
		{
			protected ConnectionState(ActivityMonitorComponent component)
			{
				Component = component;
			}

			public abstract ConnectionState Update();

			protected ActivityMonitorComponent Component { get; private set; }
		}

		class ConnectedState : ConnectionState
		{
			public ConnectedState(ActivityMonitorComponent component)
				: base(component)
			{
			}

			public override ConnectionState Update()
			{
				if (this.Component.ActivityMonitor.IsConnected)
					return this;

				this.Component.ActivityMonitor.WorkItemsChanged -= this.Component.WorkItemsChanged;
				return new DisconnectedState(this.Component);
			}
		}

		class DisconnectedState : ConnectionState
		{
			public DisconnectedState(ActivityMonitorComponent component)
				: base(component)
			{
			}

			public override ConnectionState Update()
			{
				if (!this.Component.ActivityMonitor.IsConnected)
					return this;

				// whatever is on the screen is out-of-date and should be refreshed
				this.Component.ActivityMonitor.WorkItemsChanged += this.Component.WorkItemsChanged;
				this.Component.RefreshInternal();
				return new ConnectedState(this.Component);
			}
		}

		#endregion

		#region WorkItem class

		internal class WorkItem
		{
			private readonly WorkItemData _data;

			public WorkItem(WorkItemData data)
			{
				_data = data;
			}

		    public WorkItemData Data
		    {
                get { return _data; }
		    }

			public long Id
			{
				get { return _data.Identifier; }
			}

			public string PatientId
			{
				get { return _data.Patient != null ? _data.Patient.PatientId : null; }
			}

			public string PatientsName
			{
				get { return _data.Patient != null ? _data.Patient.PatientsName : null; }
			}

			public string PatientsBirthDate
			{
				get { return _data.Patient != null ? _data.Patient.PatientsBirthDate : null; }
			}

			public string PatientsSex
			{
				get { return _data.Patient != null ? _data.Patient.PatientsSex : null; }
			}

			public string PatientInfo
			{
				get
				{
					var p = _data.Patient;
					if (p == null)
						return null;
                    return string.Format("{0} \u00B7 {1}", new PersonName(p.PatientsName).FormattedName, p.PatientId);
				}
			}

			public string StudyDate
			{
				get { return _data.Study != null ? _data.Study.StudyDate : null; }
			}

            public string StudyTime
            {
                get { return _data.Study != null ? _data.Study.StudyTime : null; }
            }

			public string AccessionNumber
			{
				get { return _data.Study != null ? _data.Study.AccessionNumber : null; }
			}

			public string StudyDescription
			{
				get { return _data.Study != null ? _data.Study.StudyDescription : null; }
			}

			public string StudyInfo
			{
				get
				{
                    if (_data.Study == null) return string.Empty;

                    // TODO (Marmot) Code taken from DicomImageSetDescriptor.GetName()  Should be consolidated?  TBD
                    DateTime studyDate;
                    DateParser.Parse(StudyDate, out studyDate);
                    DateTime studyTime;
                    TimeParser.Parse(StudyTime, out studyTime);

                    string modalitiesInStudy = null;
                    if (_data.Study != null && _data.Study.ModalitiesInStudy != null)
                        modalitiesInStudy = StringUtilities.Combine(_data.Study.ModalitiesInStudy, ", ");

                    var nameBuilder = new StringBuilder();
                    nameBuilder.AppendFormat("{0} {1}", studyDate.ToString(Format.DateFormat),
                                                        studyTime.ToString(Format.TimeFormat));

                    if (!String.IsNullOrEmpty(AccessionNumber))
                        nameBuilder.AppendFormat(", A#: {0}", AccessionNumber);

                    nameBuilder.AppendFormat(", [{0}] {1}", modalitiesInStudy ?? string.Empty, StudyDescription);

                    return nameBuilder.ToString();
				}
			}


			public WorkItemTypeEnum Type
			{
				get { return _data.Type; }
			}

			public WorkItemPriorityEnum Priority
			{
				get { return _data.Priority; }
			}

			public WorkItemStatusEnum Status
			{
				get { return _data.Status; }
			}

			public ActivityTypeEnum ActivityType
			{
				get { return _data.Request != null ? _data.Request.ActivityType : ActivityTypeEnum.ReIndex; }
			}

			public string ActivityDescription
			{
				get { return _data.Request != null ? _data.Request.ActivityDescription : null; }
			}

			public string ProgressStatus
			{
				get { return _data.Progress != null ? _data.Progress.Status : null; }
			}

			public string ProgressStatusDescription
			{
				get { return _data.Progress != null ? _data.Progress.StatusDetails : null; }
			}

			public DateTime ScheduledTime
			{
				get { return _data.ScheduledTime; }
			}

			public IconSet ProgressIcon
			{
				get { return GetProgressIcon(_data.Progress, _data.Status); }
			}

			public bool ContainsText(string text)
			{
				return ContainsText(text,
									w => w.PatientInfo,
									w => w.StudyInfo,
									w => w.ActivityDescription,
									w => w.ProgressStatus,
									w => w.ProgressStatusDescription,
									w => w.Type.GetDescription(),
									w => w.Priority.GetDescription(),
									w => w.Status.GetDescription());
			}

			private bool ContainsText(string text, params Func<WorkItem, string>[] fields)
			{
			    text = text.ToLower();
				return fields.Any(f =>
									{
										var value = f(this);
										return !String.IsNullOrEmpty(value) && value.ToLower().Contains(text);
									});
			}
		}

		#endregion

		#region WorkItemUpdateManager class

		internal class WorkItemUpdateManager
		{
			private readonly ItemCollection<WorkItem> _items;
			private readonly Dictionary<long, WorkItem> _failures;
			private readonly Predicate<WorkItem> _filter;

			public WorkItemUpdateManager(ItemCollection<WorkItem> itemCollection, Predicate<WorkItem> filter)
			{
				_items = itemCollection;
				_filter = filter;
				_failures = new Dictionary<long, WorkItem>();
			}

			public int FailedItemCount
			{
				get { return _failures.Values.Count(item => item.Status == WorkItemStatusEnum.Failed); }
			}

			public void Clear()
			{
				_items.Clear();
				_failures.Clear();
			}

			public void Update(IEnumerable<WorkItem> items)
			{
				var adds = new List<WorkItem>();
				foreach (var item in items)
				{
					var index = _items.FindIndex(w => w.Id == item.Id);
					if (index > -1)
					{
						// the item is currently in the list
						// if the item is marked deleted, or if it no longer meets the filter criteria, remove it
						// otherwise update it
						if (item.Status == WorkItemStatusEnum.Deleted || item.Status == WorkItemStatusEnum.DeleteInProgress || !Include(item))
							_items.RemoveAt(index);
						else
							_items[index] = item;
					}
					else
					{
						// the item is not currently in the list
						// if not deleted and it meets the filter criteria, add it
						if (item.Status != WorkItemStatusEnum.Deleted && item.Status != WorkItemStatusEnum.DeleteInProgress && Include(item))
						{
							adds.Add(item);
						}
					}

					// track failures
					if (item.Status == WorkItemStatusEnum.Failed || _failures.ContainsKey(item.Id))
					{
						_failures[item.Id] = item;
					}
				}

				// more efficient to add everything new at once (so GUI only re-draws once)
				_items.AddRange(adds);
			}

			private bool Include(WorkItem item)
			{
				return _filter(item);
			}
		}

		#endregion

		#region StudyCountWatcher class

		class StudyCountWatcher : IDisposable
		{
			private int _studyCount = -1;
			private readonly Timer _throttleTimer;
			private readonly System.Action _onChanged;

			public StudyCountWatcher(System.Action onChanged)
			{
				_throttleTimer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(5));
				_onChanged = onChanged;
			}

			public void Invalidate()
			{
				_throttleTimer.Start();
			}

			public void Dispose()
			{
				_throttleTimer.Dispose();
			}

			public int StudyCount
			{
				get
				{
					if (_studyCount == -1)
					{
						try
						{
							Platform.GetService<IStudyStoreQuery>(s => _studyCount = s.GetStudyCount(new GetStudyCountRequest()).StudyCount);
						}
						catch (Exception e)
						{
							//TODO (Marmot): Show something to the user?
							Platform.Log(LogLevel.Error, e, "Error getting the count of studies in the local store.");
						}
					}
					return _studyCount;
				}
			}

			private void OnTimerElapsed(object state)
			{
				_throttleTimer.Stop();
				_studyCount = -1;	// invalidate
				_onChanged();
			}
		}

		#endregion

		#region DiskspaceWatcher class

		class DiskspaceWatcher : IDisposable
		{
			private Diskspace _diskspace;
			private readonly Timer _refreshTimer;
			private readonly System.Action _onChanged;
			private readonly Func<string> _fileStorePathProvider;

			public DiskspaceWatcher(Func<string> fileStorePathProvider, System.Action onChanged)
			{
				_fileStorePathProvider = fileStorePathProvider;
				_refreshTimer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(20));
				_onChanged = onChanged;
			}

			public void Start()
			{
				_refreshTimer.Start();
			}

			public void Dispose()
			{
				_refreshTimer.Dispose();
			}

			public Diskspace Diskspace
			{
				get
				{
					if (_diskspace == null)
					{
						_diskspace = new Diskspace(_fileStorePathProvider().Substring(0, 1));
					}
					return _diskspace;
				}
			}

			private void OnTimerElapsed(object state)
			{
				_diskspace = null;	// invalidate
				_onChanged();
			}
		}

		#endregion

		#region WorkItemActionModel

		class WorkItemActionModel : SimpleActionModel
		{
		    private const string _deleteKey = "delete";
            private const string _cancelKey = "cancel";
            private const string _restartKey = "restart";

		    private readonly IItemCollection<WorkItem> _workItems;
            private IList<long> _selectedWorkItemIDs;

            public WorkItemActionModel(IItemCollection<WorkItem> workItems)
				: base(new ApplicationThemeResourceResolver(typeof(ActivityMonitorComponent).Assembly, new ApplicationThemeResourceResolver(typeof(CrudActionModel).Assembly)))
			{
                AddAction(_cancelKey, SR.NameCancelWorkItem, "CancelToolSmall.png", SR.TooltipCancelWorkItem, CancelSelectedWorkItems);
                AddAction(_restartKey, SR.NameRestartWorkItem, "RestartToolSmall.png", SR.TooltipRestartWorkItem, RestartSelectedWorkItems);
                AddAction(_deleteKey, SR.NameDeleteWorkItem, "DeleteToolSmall.png", SR.TooltipDeleteWorkItem, DeleteSelectedWorkItems);

                _workItems = workItems;
            }

            public IList<long> SelectedWorkItemIDs
            {
                get { return _selectedWorkItemIDs ?? (_selectedWorkItemIDs = new List<long>()); }
                set
                {
                    _selectedWorkItemIDs = value;
                    UpdateActionEnablement();
                }
            }

		    private IEnumerable<WorkItem> SelectedWorkItems
		    {
                get { return _workItems.Where(w => SelectedWorkItemIDs.Contains(w.Id)); }
		    }

		    private Action DeleteAction { get { return this[_deleteKey]; } }
            private Action CancelAction { get { return this[_cancelKey]; } }
            private Action RestartAction { get { return this[_restartKey]; } }

            public void OnWorkItemsChanged(IEnumerable<WorkItem> items)
            {
                if (items.Any(item => SelectedWorkItemIDs.Contains(item.Id)))
                    UpdateActionEnablement();
            }

            private void UpdateActionEnablement()
            {
		        DeleteAction.Enabled = SelectedWorkItems.All(
                    w => w.Status == WorkItemStatusEnum.Complete
                         || w.Status == WorkItemStatusEnum.Failed
                         || w.Status == WorkItemStatusEnum.Canceled);

                CancelAction.Enabled = SelectedWorkItems.All(
                    w => w.Status == WorkItemStatusEnum.InProgress
                         || w.Status == WorkItemStatusEnum.Idle
                         || w.Status == WorkItemStatusEnum.Pending);

                RestartAction.Enabled = SelectedWorkItems.All(
                    w => w.Status == WorkItemStatusEnum.Canceled
                         || w.Status == WorkItemStatusEnum.Failed);
            }

		    private void RestartSelectedWorkItems()
		    {
                try
                {
                    var client = new WorkItemClient();
                    foreach (var workItem in SelectedWorkItems)
                    {
                        client.WorkItem = workItem.Data;
                        client.Reset();
                    }
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, Application.ActiveDesktopWindow);
                }
		    }

            private void CancelSelectedWorkItems()
            {
                try
                {
                    var client = new WorkItemClient();
                    foreach (var workItem in SelectedWorkItems)
                    {
                        client.WorkItem = workItem.Data;
                        client.Cancel();
                    }
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, Application.ActiveDesktopWindow);
                }
            }

		    private void DeleteSelectedWorkItems()
            {
                try
                {
                    var client = new WorkItemClient();
                    foreach (var workItem in SelectedWorkItems)
                    {
                        client.WorkItem = workItem.Data;
                        client.Delete();
                    }
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, Application.ActiveDesktopWindow);
                }
            }
		}

		#endregion

		private static readonly object NoFilter = new object();

		private readonly WorkItemActionModel _workItemActionModel;
		private readonly Table<WorkItem> _workItems = new Table<WorkItem>();
		private readonly WorkItemUpdateManager _workItemManager;

		private IDicomServerConfigurationProvider _dicomConfigProvider;
		private ConnectionState _connectionState;
		private WorkItemStatusEnum? _statusFilter;
        private ActivityTypeEnum? _activityFilter;

		private string _textFilter;
		private readonly Timer _textFilterTimer;

		private readonly DiskspaceWatcher _diskspaceWatcher;
		private readonly StudyCountWatcher _studyCountWatcher;

	    public ActivityMonitorComponent()
		{
			_connectionState = new DisconnectedState(this);
			_textFilterTimer = new Timer(OnTextFilterTimerElapsed, null, 1000);
			_diskspaceWatcher = new DiskspaceWatcher(() => this.FileStore, OnDiskspaceChanged);
			_studyCountWatcher = new StudyCountWatcher(OnStudyCountChanged);
			_workItemManager = new WorkItemUpdateManager(_workItems.Items, Include);
            _workItemActionModel = new WorkItemActionModel(_workItems.Items);
		}

		public override void Start()
		{
			base.Start();

			_dicomConfigProvider = DicomServerConfigurationHelper.GetConfigurationProvider();
			_dicomConfigProvider.Changed += DicomServerConfigurationChanged;

            _workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnPatient, w => w.PatientInfo));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnStudy, w => w.StudyInfo));
            _workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnActivityDescription, w => w.ActivityDescription) { WidthFactor = .8f });
            _workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnStatus, w => w.Status.GetDescription()) { WidthFactor = .4f });
			_workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnStatusDescription, w => w.ProgressStatus));
            _workItems.Columns.Add(new DateTimeTableColumn<WorkItem>(SR.ColumnScheduledTime, w => w.ScheduledTime) { WidthFactor = .6f });
            _workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnPriority, w => w.Priority.GetDescription()) { WidthFactor = .3f });
			_workItems.Columns.Add(new TableColumn<WorkItem, IconSet>(SR.ColumnProgress, w => w.ProgressIcon) { WidthFactor = .5f});

			this.ActivityMonitor = WorkItemActivityMonitor.Create(true);
			_connectionState = _connectionState.Update();

			this.ActivityMonitor.IsConnectedChanged += ActivityMonitorIsConnectedChanged;

			_diskspaceWatcher.Start();
		}

	    public override void Stop()
		{
			ActivityMonitor.WorkItemsChanged -= WorkItemsChanged;
			ActivityMonitor.IsConnectedChanged -= ActivityMonitorIsConnectedChanged;
			ActivityMonitor.Dispose();
			ActivityMonitor = null;

			_dicomConfigProvider.Changed -= DicomServerConfigurationChanged;
			_dicomConfigProvider = null;

			_textFilterTimer.Dispose();
			_diskspaceWatcher.Dispose();
			_studyCountWatcher.Dispose();

			base.Stop();
		}

		#region Presentation Model

		public bool IsConnected
		{
			get { return _connectionState is ConnectedState; }
		}

		public string AeTitle
		{
			get { return _dicomConfigProvider.AETitle; }
		}

		public string HostName
		{
			get { return _dicomConfigProvider.HostName; }
		}

		public int Port
		{
			get { return _dicomConfigProvider.Port; }
		}

		public string FileStore
		{
			get { return _dicomConfigProvider.FileStoreDirectory; }
		}

		public int DiskspaceUsedPercent
		{
			get { return _diskspaceWatcher.Diskspace.UsedSpacePercent; }
		}

		public string DiskspaceUsed
		{
			get
			{
				var ds = _diskspaceWatcher.Diskspace;
				return string.Format(SR.DiskspaceTemplate,
					Diskspace.FormatBytes(ds.UsedSpace),
					Diskspace.FormatBytes(ds.TotalSpace),
					ds.UsedSpacePercent);
			}
		}

	    public int TotalStudies
	    {
            get { return _studyCountWatcher.StudyCount; }
	    }

		public int Failures
		{
			get { return _workItemManager.FailedItemCount; }
		}

		public ActionModelNode WorkItemActions
		{
			get { return _workItemActionModel; }
		}

		public ITable WorkItemTable
		{
			get { return _workItems; }
		}

		public IList StatusFilterChoices
		{
            get { return new[] { NoFilter }.Concat(Enum.GetValues(typeof(WorkItemStatusEnum)).Cast<object>().OrderBy<object, string>(FormatStatusFilter)).ToList(); }
		}

		public string FormatStatusFilter(object value)
		{
			return value == NoFilter ? SR.NoFilterItem : ((WorkItemStatusEnum)value).GetDescription();
		}

		public object StatusFilter
		{
			get { return _statusFilter.HasValue ? _statusFilter.Value : NoFilter; }
			set
			{
				var v = (value == NoFilter) ? (WorkItemStatusEnum?)null : (WorkItemStatusEnum)value;
				if (_statusFilter != v)
				{
					_statusFilter = v;
					NotifyPropertyChanged("StatusFilter");
					RefreshInternal();
				}
			}
		}

		public IList ActivityTypeFilterChoices
		{
			get { return new[] { NoFilter }.Concat(Enum.GetValues(typeof(ActivityTypeEnum)).Cast<object>().OrderBy<object, string>(FormatActivityTypeFilter)).ToList(); }
		}

		public string FormatActivityTypeFilter(object value)
		{
			return value == NoFilter ? SR.NoFilterItem : ((ActivityTypeEnum)value).GetDescription();
		}

		public object ActivityTypeFilter
		{
			get { return _activityFilter.HasValue ? _activityFilter.Value : NoFilter; }
			set
			{
                var v = (value == NoFilter) ? (ActivityTypeEnum?)null : (ActivityTypeEnum)value;
				if (_activityFilter != v)
				{
					_activityFilter = v;
					NotifyPropertyChanged("ActivityTypeFilter");
					RefreshInternal();
				}
			}
		}

		public string TextFilter
		{
			get { return _textFilter; }
			set
			{
				if (value != _textFilter)
				{
					_textFilter = value;
					NotifyPropertyChanged("TextFilter");

					// we don't want to do a full refresh on every keystroke, so
					// rather than refreshing immediately, we just re-start the timer
					_textFilterTimer.Stop();
					_textFilterTimer.Start();
				}
			}
		}

        public void SetSelection(ISelection selection)
        {
            SelectedWorkItemIDs = _workItemActionModel.SelectedWorkItemIDs = selection.Items.Cast<WorkItem>().Select(w => w.Id).ToList();
        }

        public void StartReindex()
        {
            ReindexTool.StartReindex(Host.DesktopWindow);
        }

		public void OpenFileStore()
		{
			var path = this.FileStore;
			if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
				Process.Start(path);
		}

	    #endregion

        private IList<long> SelectedWorkItemIDs { get; set; }
        
        private IWorkItemActivityMonitor ActivityMonitor { get; set; }

	    private static IconSet GetProgressIcon(WorkItemProgress progress, WorkItemStatusEnum status)
		{
			if (progress == null)
				return null;

			return new ProgressBarIconSet("progress", new Size(80, 10), progress.PercentComplete * 100, GetProgressState(status));
		}

		private static ProgressBarState GetProgressState(WorkItemStatusEnum status)
		{
			// determine progress state based on workItem status
			switch (status)
			{
				case WorkItemStatusEnum.InProgress:
				case WorkItemStatusEnum.Complete:
				case WorkItemStatusEnum.Deleted:
				case WorkItemStatusEnum.Canceled:
					return ProgressBarState.Active;

				case WorkItemStatusEnum.Pending:
				case WorkItemStatusEnum.Idle:
                    return ProgressBarState.Paused;

                case WorkItemStatusEnum.Failed:
					return ProgressBarState.Error;
			}
			throw new NotImplementedException();
		}

		private void DicomServerConfigurationChanged(object sender, EventArgs e)
		{
			NotifyPropertyChanged("AeTitle");
			NotifyPropertyChanged("HostName");
			NotifyPropertyChanged("Port");
			NotifyPropertyChanged("FileStore");

			// if FileStore path changed, diskspace may have changed too
			NotifyPropertyChanged("DiskspaceUsedPercent");
			NotifyPropertyChanged("DiskspaceUsed");
		}

		private void ActivityMonitorIsConnectedChanged(object sender, EventArgs e)
		{
			_connectionState = _connectionState.Update();
			NotifyPropertyChanged("IsConnected");
		}

		private void WorkItemsChanged(object sender, WorkItemsChangedEventArgs e)
		{
			var workItems = e.ChangedItems;
			if (workItems.Any(item => item.Type != WorkItemTypeEnum.ReapplyRules && item.Type != WorkItemTypeEnum.DicomSend))
			{
				_studyCountWatcher.Invalidate();
			}

			var items = workItems.Select(item => new WorkItem(item));
			_workItemManager.Update(items);
			_workItemActionModel.OnWorkItemsChanged(items);

			// tell view to update this value
			NotifyPropertyChanged("Failures");
		}

	    private void RefreshInternal()
	    {
	    	_workItemManager.Clear();

			try
			{
				this.ActivityMonitor.Refresh();
			}
			catch (Exception e)
			{
				// don't show a message box here, since the user may not even be looking at this workspace
				Platform.Log(LogLevel.Error, e);
			}
		}

		private bool Include(WorkItem item)
		{
            if (_activityFilter.HasValue && item.ActivityType != _activityFilter.Value)
				return false;

			if (_statusFilter.HasValue && item.Status != _statusFilter.Value)
				return false;

			if (!string.IsNullOrEmpty(_textFilter) && !item.ContainsText(_textFilter))
				return false;

			return true;
		}

		private void OnTextFilterTimerElapsed(object state)
		{
			_textFilterTimer.Stop();
			RefreshInternal();
		}

		private void OnStudyCountChanged()
		{
			NotifyPropertyChanged("TotalStudies");
		}

		private void OnDiskspaceChanged()
		{
			NotifyPropertyChanged("DiskspaceUsed");
			NotifyPropertyChanged("DiskspaceUsedPercent");
		}
    }
}
