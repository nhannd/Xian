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
				this.Component.ActivityMonitor.WorkItemChanged += this.Component.WorkItemChanged;
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
				get { return _data.Progress != null ? _data.Request.ActivityType : ActivityTypeEnum.ReIndex; }
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
			private readonly Predicate<WorkItem> _filter;

			public WorkItemUpdateManager(ItemCollection<WorkItem> itemCollection, Predicate<WorkItem> filter)
			{
				_items = itemCollection;
				_filter = filter;
			}

			public void Update(WorkItem newItem)
			{
				var index = _items.FindIndex(w => w.Id == newItem.Id);
				if (index > -1)
				{
					// the item is currently in the list
					// if the item is marked deleted, or if it no longer meets the filter criteria, remove it
					// otherwise update it
					if (newItem.Status == WorkItemStatusEnum.Deleted || !Include(newItem))
						_items.RemoveAt(index);
					else
						_items[index] = newItem;
				}
				else
				{
					// the item is not currently in the list
					// if not deleted and it meets the filter criteria, add it
					if (newItem.Status != WorkItemStatusEnum.Deleted && Include(newItem))
					{
						_items.Add(newItem);
					}
				}

			}

			private bool Include(WorkItem item)
			{
				return _filter(item);
			}
		}

		#endregion

		class WorkItemActionModel : SimpleActionModel
		{
			public WorkItemActionModel()
				: base(new ApplicationThemeResourceResolver(typeof(ActivityMonitorComponent).Assembly, new ApplicationThemeResourceResolver(typeof(CrudActionModel).Assembly)))
			{
			}
		}


		private static readonly object NoFilter = new object();

		private readonly WorkItemActionModel _workItemActionModel = new WorkItemActionModel();
		private readonly Table<WorkItem> _workItems = new Table<WorkItem>();
		private readonly WorkItemUpdateManager _workItemManager;

		private IDicomServerConfigurationProvider _dicomConfigProvider;
		private ConnectionState _connectionState;
		private WorkItemStatusEnum? _statusFilter;
        private ActivityTypeEnum? _activityFilter;

		private string _textFilter;
		private readonly Timer _textFilterTimer;

		private Diskspace _diskspace;
		private readonly Timer _diskspaceTimer;

		private int _totalStudies;

	    public ActivityMonitorComponent()
		{
			_connectionState = new DisconnectedState(this);
			_textFilterTimer = new Timer(OnTextFilterTimerElapsed, null, 1000);
			_diskspaceTimer = new Timer(OnDiskspaceTimerElapsed, null, TimeSpan.FromSeconds(60));

	    	_workItemManager = new WorkItemUpdateManager(_workItems.Items, Include);
		}

		public override void Start()
		{
			base.Start();

		    UpdateStudyCount();		

			_dicomConfigProvider = DicomServerConfigurationHelper.GetConfigurationProvider();
			_dicomConfigProvider.Changed += DicomServerConfigurationChanged;

            _workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnPatient, w => w.PatientInfo));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnStudy, w => w.StudyInfo));
            _workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnActivityDescription, w => w.ActivityDescription) { WidthFactor = .8f });
            _workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnStatus, w => w.Status.GetDescription()) { WidthFactor = .4f });
			_workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnStatusDescription, w => w.ProgressStatus));
            _workItems.Columns.Add(new TableColumn<WorkItem, DateTime>(SR.ColumnScheduledTime, w => w.ScheduledTime) { WidthFactor = .6f });
            _workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnPriority, w => w.Priority.GetDescription()) { WidthFactor = .3f });
			_workItems.Columns.Add(new TableColumn<WorkItem, IconSet>(SR.ColumnProgress, w => w.ProgressIcon) { WidthFactor = .5f});

			this.ActivityMonitor = WorkItemActivityMonitor.Create(true);
			_connectionState = _connectionState.Update();

			this.ActivityMonitor.IsConnectedChanged += ActivityMonitorIsConnectedChanged;

			_diskspaceTimer.Start();
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
			_diskspaceTimer.Dispose();

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
			get { return this.Diskspace.UsedSpacePercent; }
		}

		public string DiskspaceUsed
		{
			get
			{
				var ds = this.Diskspace;
				return string.Format(SR.DiskspaceTemplate,
					Diskspace.FormatBytes(ds.UsedSpace),
					Diskspace.FormatBytes(ds.TotalSpace),
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

		private void WorkItemChanged(object sender, WorkItemChangedEventArgs e)
		{
		    UpdateStudyCount(e.ItemData);

			_workItemManager.Update(new WorkItem(e.ItemData));
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
			_workItems.Items.Clear();

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

		private void OnDiskspaceTimerElapsed(object state)
		{
			_diskspace = null;	// invalidate

			NotifyPropertyChanged("DiskspaceUsed");
			NotifyPropertyChanged("DiskspaceUsedPercent");
		}

		private Diskspace Diskspace
		{
			get
			{
				if (_diskspace == null)
				{
					_diskspace = new Diskspace(this.FileStore.Substring(0, 1));
				}
				return _diskspace;
			}
		}

	}
}
