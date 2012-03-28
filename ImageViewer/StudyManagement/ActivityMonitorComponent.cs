using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	[ExtensionPoint]
	public sealed class ActivityMonitorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

	[AssociateView(typeof(ActivityMonitorComponentViewExtensionPoint))]
	public class ActivityMonitorComponent : ApplicationComponent
	{
		private readonly Table<WorkItemData> _workItems = new Table<WorkItemData>();
		private WorkItemData _data = new WorkItemData {Progress = new ImportFilesProgress()};

        public ActivityMonitorComponent()
		{
            
			
		}

		public override void Start()
		{
			base.Start();

			_workItems.Columns.Add(new TableColumn<WorkItemData, string>("Patient ID", w => w.Patient != null ? w.Patient.PatientId : null));
			_workItems.Columns.Add(new TableColumn<WorkItemData, string>("Name", w => w.Patient != null ? w.Patient.PatientsName : null));
			_workItems.Columns.Add(new TableColumn<WorkItemData, string>("Birthdate", w => w.Patient != null ? w.Patient.PatientsBirthDate : null));
			_workItems.Columns.Add(new TableColumn<WorkItemData, string>("Sex", w => w.Patient != null ? w.Patient.PatientsSex : null));
			_workItems.Columns.Add(new TableColumn<WorkItemData, string>("Study date", w => w.Study != null ? w.Study.StudyDate : null));
			_workItems.Columns.Add(new TableColumn<WorkItemData, string>("Accession", w => w.Study != null ? w.Study.AccessionNumber : null));
			_workItems.Columns.Add(new TableColumn<WorkItemData, string>("Study Desc.", w => w.Study != null ? w.Study.StudyDescription : null));
			_workItems.Columns.Add(new TableColumn<WorkItemData, string>("Activity", w => w.Request.ActivityType));
			_workItems.Columns.Add(new TableColumn<WorkItemData, string>("Activity Desc.", w => w.Request.ActivityDescription));
			_workItems.Columns.Add(new TableColumn<WorkItemData, string>("Status", w => w.Status.ToString()));
			_workItems.Columns.Add(new TableColumn<WorkItemData, string>("Status Desc.", w => w.Progress != null ? w.Progress.Status : null));
			_workItems.Columns.Add(new TableColumn<WorkItemData, DateTime>("Sched. Time", w => w.ScheduledTime));
			_workItems.Columns.Add(new TableColumn<WorkItemData, string>("Priority", w => w.Priority.ToString()));
			_workItems.Columns.Add(new TableColumn<WorkItemData, IconSet>("Progress", w => GetProgressIcon(w.Progress)));

			_workItems.Items.Add(_data);
		}

		#region Presentation Model

		public ITable WorkItemTable
		{
			get { return _workItems; }
		}

		public void Search()
		{
			_workItems.Items.NotifyItemUpdated(_data);
		}

		#endregion

		private static IconSet GetProgressIcon(WorkItemProgress progress)
		{
			return new ProgressBarIconSet("progress", new Size(60, 10), progress != null ? progress.PercentComplete : 0);
		}
	}
}
