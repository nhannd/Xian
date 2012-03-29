using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.WorkItem;
using System.Collections;

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
				if (this.Component.Monitor.IsConnected)
					return this;

				this.Component.Monitor.WorkItemChanged -= this.Component.WorkItemChanged;
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
				if (!this.Component.Monitor.IsConnected)
					return this;

				this.Component.Refresh();
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
				get { return GetProgressIcon(_data.Progress); }
			}
		}

		#endregion

		private static readonly object NoFilter = new object();

		private readonly Table<WorkItem> _workItems = new Table<WorkItem>();

		private IDicomServerConfigurationProvider _dicomConfigProvider;
		private ConnectionState _connectionState;
		private WorkItemStatusEnum? _statusFilter;
		private WorkItemTypeEnum? _activityFilter;

		public ActivityMonitorComponent()
		{
			_connectionState = new DisconnectedState(this);
		}

		public override void Start()
		{
			base.Start();


			_dicomConfigProvider = DicomServerConfigurationHelper.GetConfigurationProvider();
			_dicomConfigProvider.Changed += DicomServerConfigurationChanged;

			// todo loc
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Patient ID", w => w.PatientId));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Name", w => w.PatientsName));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Birthdate", w => w.PatientsBirthDate));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Sex", w => w.PatientsSex));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Study date", w => w.StudyDate));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Accession", w => w.AccessionNumber));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Study Desc.", w => w.StudyDescription));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Activity", w => w.ActivityType));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Activity Desc.", w => w.ActivityDescription));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Status", w => w.Status.ToString()));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Status Desc.", w => w.ProgressStatus));
			_workItems.Columns.Add(new TableColumn<WorkItem, DateTime>("Sched. Time", w => w.ScheduledTime));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Priority", w => w.Priority.ToString()));
			_workItems.Columns.Add(new TableColumn<WorkItem, IconSet>("Progress", w => w.ProgressIcon));

			this.Monitor = WorkItemActivityMonitor.Create(true);
			_connectionState = _connectionState.Update();

			this.Monitor.IsConnectedChanged += (sender, e) => _connectionState = _connectionState.Update();
		}

		public override void Stop()
		{
			Monitor.WorkItemChanged -= WorkItemChanged;
			Monitor.Dispose();
			Monitor = null;

			_dicomConfigProvider.Changed -= DicomServerConfigurationChanged;
			_dicomConfigProvider = null;

			base.Stop();
		}

		#region Presentation Model

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
			get { return (int)QueryDiskspace().UsedSpacePercent; }
		}

		public string DiskspaceUsed
		{
			get { return Diskspace.FormatBytes(QueryDiskspace().UsedSpace); }
		}

		public int TotalStudies { get; private set; }

		public int Failures { get; private set; }


		public ITable WorkItemTable
		{
			get { return _workItems; }
		}

		public IList StatusFilterChoices
		{
			get { return new [] { NoFilter }.Concat(Enum.GetValues(typeof(WorkItemStatusEnum)).Cast<object>()).ToList(); }
		}

		public string FormatStatusFilter(object value)
		{
			// todo loc
			if (value == NoFilter)
				return "(all)";
			return value.ToString();
		}

		public object StatusFilter
		{
			get { return _statusFilter.HasValue ? _statusFilter.Value : NoFilter; }
			set
			{
				var v = (value == NoFilter) ? (WorkItemStatusEnum?)null : (WorkItemStatusEnum)value;
				if(_statusFilter != v)
				{
					_statusFilter = v;
					NotifyPropertyChanged("StatusFilter");
					Refresh();
				}
			}
		}

		public IList ActivityTypeFilterChoices
		{
			get { return new [] { NoFilter }.Concat(Enum.GetValues(typeof(WorkItemTypeEnum)).Cast<object>()).ToList(); }
		}

		public string FormatActivityTypeFilter(object value)
		{
			// todo loc
			if (value == NoFilter)
				return "(all)";
			return value.ToString();
		}

		public object ActivityTypeFilter
		{
			get { return _activityFilter.HasValue ? _activityFilter.Value : NoFilter; }
			set
			{
				var v = (value == NoFilter) ? (WorkItemTypeEnum?)null : (WorkItemTypeEnum)value;
				if(_activityFilter != v)
				{
					_activityFilter = v;
					NotifyPropertyChanged("ActivityTypeFilter");
					Refresh();
				}
			}
		}

		#endregion

		private IWorkItemActivityMonitor Monitor { get; set; }

		private static IconSet GetProgressIcon(WorkItemProgress progress)
		{
			if (progress == null)
				return null;

			return new ProgressBarIconSet("progress", new Size(80, 10), progress.PercentComplete * 100);
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

		private void WorkItemChanged(object sender, WorkItemChangedEventArgs e)
		{
			var newItem = new WorkItem(e.ItemData);
			var index = _workItems.Items.FindIndex(w => w.Id == newItem.Id);
			if (index > -1)
			{
				if (newItem.Status == WorkItemStatusEnum.Deleted || !Include(newItem))
					_workItems.Items.RemoveAt(index);
				else
					_workItems.Items[index] = newItem;
			}
			else
			{
				if (Include(newItem))
				{
					_workItems.Items.Add(newItem);
				}
			}
		}

		private void Refresh()
		{
			Monitor.WorkItemChanged -= WorkItemChanged;
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
				// do not show a message box here, since refreshes can happen spontaneously
				// and the user might not even be on this page
				Platform.Log(LogLevel.Error, e);
			}

			Monitor.WorkItemChanged += WorkItemChanged;
		}

		private Diskspace QueryDiskspace()
		{
			return new Diskspace(this.FileStore.Substring(0, 1));
		}

		private bool Include(WorkItem item)
		{
			if(_activityFilter.HasValue && item.Type != _activityFilter.Value)
				return false;

			if(_statusFilter.HasValue && item.Status != _statusFilter.Value)
				return false;

			return true;
		}
	}
}
