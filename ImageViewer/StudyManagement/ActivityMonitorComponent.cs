using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.WorkItem;
using System.Net;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	[ExtensionPoint]
	public sealed class ActivityMonitorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

	[AssociateView(typeof(ActivityMonitorComponentViewExtensionPoint))]
	public class ActivityMonitorComponent : ApplicationComponent
	{
		class WorkItem
		{
			private WorkItemData _data;

			public WorkItem(WorkItemData data)
			{
				_data = data;
			}

			public string PatientId
			{
				get { throw new NotImplementedException(); }
			}

			public string PatientsName
			{
				get { throw new NotImplementedException(); }
			}

			public string PatientsBirthDate
			{
				get { throw new NotImplementedException(); }
			}

			public string PatientsSex
			{
				get { throw new NotImplementedException(); }
			}

			public string StudyDate
			{
				get { throw new NotImplementedException(); }
			}

			public string AccessionNumber
			{
				get { throw new NotImplementedException(); }
			}

			public string StudyDescription
			{
				get { throw new NotImplementedException(); }
			}

			public string Type
			{
				get { throw new NotImplementedException(); }
			}

			public string Status
			{
				get { throw new NotImplementedException(); }
			}

			public string StatusDescription
			{
				get { throw new NotImplementedException(); }
			}

			public DateTime ScheduledTime
			{
				get { throw new NotImplementedException(); }
			}

			public string Priority
			{
				get { throw new NotImplementedException(); }
			}

			public IconSet ProgressIcon
			{
				get { throw new NotImplementedException(); }
			}
		}

		private IWorkItemActivityMonitor _monitor;
		private readonly Table<WorkItemData> _workItems = new Table<WorkItemData>();
		private WorkItemData _data = new WorkItemData {Progress = new ImportFilesProgress()};

		private IDicomServerConfigurationProvider _dicomConfigProvider;
		private Diskspace _totalDiskSpace;
		private Diskspace _usedDiskSpace;

		public ActivityMonitorComponent()
		{
			
		}

		public override void Start()
		{
			base.Start();


			_dicomConfigProvider = DicomServerConfigurationHelper.GetConfigurationProvider();
			_dicomConfigProvider.Changed += DicomServerConfigurationChanged;

			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Patient ID", w => w.PatientId));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Name", w => w.PatientsName));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Birthdate", w => w.PatientsBirthDate));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Sex", w => w.PatientsSex));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Study date", w => w.StudyDate));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Accession", w => w.AccessionNumber));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Study Desc.", w => w.StudyDescription));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Activity", w => w.Type));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Activity Desc.", w => "TODO"));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Status", w => w.Status));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Status Desc.", w => w.StatusDescription));
			_workItems.Columns.Add(new TableColumn<WorkItem, DateTime>("Sched. Time", w => w.ScheduledTime));
			_workItems.Columns.Add(new TableColumn<WorkItem, string>("Priority", w => w.Priority));
			_workItems.Columns.Add(new TableColumn<WorkItem, IconSet>("Progress", w => w.ProgressIcon));

			_monitor = WorkItemActivityMonitor.Create(true);
			_monitor.WorkItemChanged += new EventHandler<WorkItemChangedEventArgs>(WorkItemChanged);

			_workItems.Items.Add(_data);
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

		public void Search()
		{
			_data.Progress.PercentComplete += 10;
			_workItems.Items.NotifyItemUpdated(_data);
		}

		#endregion

		private static IconSet GetProgressIcon(WorkItemProgress progress)
		{
			return new ProgressBarIconSet("progress", new Size(60, 10), progress != null ? progress.PercentComplete : 0);
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
		}


		private Diskspace QueryDiskspace()
		{
			return new Diskspace(this.FileStore.Substring(0, 1));
		}

	}
}
