using System;
using System.Drawing;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using System.Collections;
using ClearCanvas.Common.Utilities;

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

				this.Component.ActivityMonitor.WorkItemChanged -= this.Component.WorkItemChanged;
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
				this.Component.RefreshInternal();
				return new ConnectedState(this.Component);
			}
		}

		#endregion

		#region WorkItem class

		class WorkItem
		{
			private readonly WorkItemData _data;

			public WorkItem(WorkItemData data)
			{
				_data = data;
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
					// todo: format this like elsewhere in the viewer
					var p = _data.Patient;
					if (p == null)
						return null;

					return string.Format("{0} {1} {2} {3}", p.PatientId, p.PatientsName, p.PatientsBirthDate, p.PatientsSex);
				}
			}

			public string StudyDate
			{
				get { return _data.Study != null ? _data.Study.StudyDate : null; }
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
					// todo: format this like elsewhere in the viewer
					var s = _data.Study;
					if (s == null)
						return null;

					return string.Format("{0} {1} {2}", s.AccessionNumber, s.StudyDescription, s.StudyDate);
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

			public string ActivityType
			{
				get { return _data.Request != null ? _data.Request.ActivityType : null; }
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
				// todo: not sure if we need to include all of these fields or not
				// would it be better to be a little more selective?
				return ContainsText(text,
									w => w.PatientInfo,
									w => w.StudyInfo,
									w => w.ActivityType,
									w => w.ActivityDescription,
									w => w.ProgressStatus,
									w => w.ProgressStatusDescription,
									w => w.Type.GetDescription(),
									w => w.Priority.GetDescription(),
									w => w.Status.GetDescription());
			}

			private bool ContainsText(string text, params Func<WorkItem, string>[] fields)
			{
				return fields.Any(f =>
									{
										var value = f(this);
										return value != null && value.Contains(text);
									});
			}
		}

		#endregion

		class WorkItemActionModel : SimpleActionModel
		{
			internal ClickAction Refresh { get; private set; }

			public WorkItemActionModel()
				: base(new ApplicationThemeResourceResolver(typeof(ActivityMonitorComponent).Assembly, new ApplicationThemeResourceResolver(typeof(CrudActionModel).Assembly)))
			{
				//todo loc
				Refresh = AddAction("refresh", "Refresh", "RefreshSmall.png");
			}
		}


		private static readonly object NoFilter = new object();

		private readonly WorkItemActionModel _workItemActionModel = new WorkItemActionModel();
		private readonly Table<WorkItem> _workItems = new Table<WorkItem>();

		private IDicomServerConfigurationProvider _dicomConfigProvider;
		private ConnectionState _connectionState;
		private WorkItemStatusEnum? _statusFilter;
		private WorkItemTypeEnum? _activityFilter;
		private string _textFilter;
		private readonly Timer _textFilterTimer;
	    private int _totalStudies;

	    public ActivityMonitorComponent()
		{
			_connectionState = new DisconnectedState(this);
			_textFilterTimer = new Timer(OnTextFilterTimerElapsed, null, 1000);
		}

		public override void Start()
		{
			base.Start();

		    UpdateStudyCount();
			_workItemActionModel.Refresh.SetClickHandler(Refresh);


			_dicomConfigProvider = DicomServerConfigurationHelper.GetConfigurationProvider();
			_dicomConfigProvider.Changed += DicomServerConfigurationChanged;

			// todo loc
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Patient", w => w.PatientInfo));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Study", w => w.StudyInfo));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Activity", w => w.ActivityType));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Activity Desc.", w => w.ActivityDescription));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Status", w => w.Status.GetDescription()));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Status Desc.", w => w.ProgressStatus));
			_workItems.Columns.Add(new TableColumn<WorkItem, DateTime>("Sched. Time", w => w.ScheduledTime));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Priority", w => w.Priority.GetDescription()));
			_workItems.Columns.Add(new TableColumn<WorkItem, IconSet>("Progress", w => w.ProgressIcon));

			this.ActivityMonitor = WorkItemActivityMonitor.Create(true);
			_connectionState = _connectionState.Update();

			this.ActivityMonitor.IsConnectedChanged += ActivityMonitorIsConnectedChanged;
		}

		public override void Stop()
		{
			ActivityMonitor.WorkItemChanged -= WorkItemChanged;
			ActivityMonitor.IsConnectedChanged -= ActivityMonitorIsConnectedChanged;
			ActivityMonitor.Dispose();
			ActivityMonitor = null;

			_dicomConfigProvider.Changed -= DicomServerConfigurationChanged;
			_dicomConfigProvider = null;

			_textFilterTimer.Dispose();

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
			//todo: put this on a timer
			get { return QueryDiskspace().UsedSpacePercent; }
		}

		public string DiskspaceUsed
		{
			//todo: put this on a timer
			get
			{
				var ds = QueryDiskspace();
				return string.Format("{0} of {1} ({2}%)",
					Diskspace.FormatBytes(ds.UsedSpace, false),
					Diskspace.FormatBytes(ds.TotalSpace, true),
					ds.UsedSpacePercent);
			}
		}

	    public int TotalStudies
	    {
            get { return _totalStudies; }
            private set
            {
                if (value == _totalStudies)
                    return;

                _totalStudies = value;
                NotifyPropertyChanged("TotalStudies");
            }
	    }

		public int Failures { get; private set; }

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
			get { return new[] { NoFilter }.Concat(Enum.GetValues(typeof(WorkItemStatusEnum)).Cast<object>()).ToList(); }
		}

		public string FormatStatusFilter(object value)
		{
			// todo loc
			if (value == NoFilter)
				return "(all)";
			return ((WorkItemStatusEnum)value).GetDescription();
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
			get { return new[] { NoFilter }.Concat(Enum.GetValues(typeof(WorkItemTypeEnum)).Cast<object>()).ToList(); }
		}

		public string FormatActivityTypeFilter(object value)
		{
			// todo loc
			if (value == NoFilter)
				return "(all)";
			return ((WorkItemTypeEnum)value).GetDescription();
		}

		public object ActivityTypeFilter
		{
			get { return _activityFilter.HasValue ? _activityFilter.Value : NoFilter; }
			set
			{
				var v = (value == NoFilter) ? (WorkItemTypeEnum?)null : (WorkItemTypeEnum)value;
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

		public void Refresh()
		{
			if (_connectionState is DisconnectedState)
			{
				this.Host.ShowMessageBox("not connected", MessageBoxActions.Ok); // todo loc
				return;
			}
			RefreshInternal();
		}

        public void StartReindex()
        {
            ReindexTool.StartReindex(Host.DesktopWindow);
        }

	    #endregion

		private IWorkItemActivityMonitor ActivityMonitor { get; set; }

		private static IconSet GetProgressIcon(WorkItemProgress progress, WorkItemStatusEnum status)
		{
			if (progress == null)
				return null;

			return new ProgressBarIconSet("progress", new Size(80, 10), progress.PercentComplete * 100, GetProgressState(status));
		}

		private static ProgressBarState GetProgressState(WorkItemStatusEnum status)
		{
			// TODO: not sure exactly how these should map to colors,
			// but I think keeping the progress bar limited to 3 colors (red, yellow, green) is a good idea
			switch (status)
			{
				case WorkItemStatusEnum.Pending:
				case WorkItemStatusEnum.InProgress:
				case WorkItemStatusEnum.Complete:
				case WorkItemStatusEnum.Deleted:
				case WorkItemStatusEnum.Canceled:
					return ProgressBarState.Active;
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
			NotifyPropertyChanged("IpAddress");
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

		private void WorkItemChanged(object sender, WorkItemChangedEventArgs e)
		{
		    UpdateStudyCount(e.ItemData);

			var newItem = new WorkItem(e.ItemData);
			var index = _workItems.Items.FindIndex(w => w.Id == newItem.Id);
			if (index > -1)
			{
				// the item is currently in the list
				// if the item is marked deleted, or if it no longer meets the filter criteria, remove it
				// otherwise update it
				if (newItem.Status == WorkItemStatusEnum.Deleted || !Include(newItem))
					_workItems.Items.RemoveAt(index);
				else
					_workItems.Items[index] = newItem;
			}
			else
			{
				// the item is not currently in the list
				// if it meets the filter criteria, add it
				if (Include(newItem))
				{
					_workItems.Items.Add(newItem);
				}
			}
		}

        private void UpdateStudyCount(WorkItemData workItem)
        {
            if (workItem.Type == WorkItemTypeEnum.ReapplyRules || workItem.Type == WorkItemTypeEnum.DicomSend)
                return;

            UpdateStudyCount();
        }

        private void UpdateStudyCount()
        {
            try
            {
                Platform.GetService<IStudyStore>(s => TotalStudies = s.GetStudyCount(new GetStudyCountRequest()).StudyCount);
            }
            catch (Exception e)
            {
                //TODO (Marmot): Show something to the user?
                Platform.Log(LogLevel.Error, e, "Error getting the count of studies in the local store.");
            }
        }

	    private void RefreshInternal()
		{
			this.ActivityMonitor.WorkItemChanged -= WorkItemChanged;
			_workItems.Items.Clear();

			try
			{
				Platform.GetService<IWorkItemService>(
					service =>
					{
						var response = service.Query(new WorkItemQueryRequest());
						var items = from wd in response.Items
									let w = new WorkItem(wd)
									where Include(w)
									select w;

						_workItems.Items.AddRange(items);
					});
			}
			catch (Exception e)
			{
				// don't show a message box here, since the user may not even be looking at this workspace
				Platform.Log(LogLevel.Error, e);
			}

			this.ActivityMonitor.WorkItemChanged += WorkItemChanged;
		}

		private bool Include(WorkItem item)
		{
			if (_activityFilter.HasValue && item.Type != _activityFilter.Value)
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

		private Diskspace QueryDiskspace()
		{
			return new Diskspace(this.FileStore.Substring(0, 1));
		}


	}
}
