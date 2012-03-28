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

			public string Type
			{
				get { return _data.Type.ToString(); }
			}

			public string Status
			{
				get { return _data.Status.ToString(); }
			}

			public string StatusDescription
			{
				get { return _data.Progress != null ? _data.Progress.StatusDetails : null; }
			}

			public DateTime ScheduledTime
			{
				get { return _data.ScheduledTime; }
			}

			public string Priority
			{
				get { return _data.Priority.ToString(); }
			}

			public IconSet ProgressIcon
			{
				get { return GetProgressIcon(_data.Progress); }
			}
		}

		private IWorkItemActivityMonitor _monitor;
		private readonly Table<WorkItem> _workItems = new Table<WorkItem>();

		private IDicomServerConfigurationProvider _dicomConfigProvider;

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
			_monitor.WorkItemChanged += WorkItemChanged;
		}

		public override void Stop()
		{
			_monitor.WorkItemChanged -= WorkItemChanged;
			_monitor.Dispose();
			_monitor = null;

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

		#endregion

		private static IconSet GetProgressIcon(WorkItemProgress progress)
		{
			if(progress == null)
				return null;

			return new ProgressBarIconSet("progress", new Size(60, 10), progress.PercentComplete);
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
			if(index > -1)
			{
				_workItems.Items[index] = newItem;
			}
			else
			{
				_workItems.Items.Add(newItem);
			}
		}


		private Diskspace QueryDiskspace()
		{
			return new Diskspace(this.FileStore.Substring(0, 1));
		}

	}
}
