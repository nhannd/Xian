using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.Timers;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExtensionPoint]
	public class DicomNetworkReceiveQueueApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public class ReceiveQueueItem
	{
		private Guid _progressIdentifier;
		private string _fromAETitle;
		private string _patientId;
		private string _patientsName;
		private string _studyInstanceUid;
		private string _studyDescription;
		private DateTime _studyDate;
		private int _numberOfFilesImported;
		private DateTime _lastActive;
		private string _lastActiveDisplay;

		private ReceiveQueueItem()
		{ 
		}

		internal ReceiveQueueItem(ReceiveProgressItem progressItem)
		{
			_progressIdentifier = progressItem.Identifier;

			UpdateFromProgressItem(progressItem);
		}

		internal void UpdateFromProgressItem(ReceiveProgressItem progressItem)
		{
			if (!_progressIdentifier.Equals(this.ProgressIdentifier))
				throw new InvalidOperationException("the identifiers must match!");

			this.FromAETitle = progressItem.FromAETitle;
			this.PatientId = progressItem.PatientId;
			this.PatientsName = progressItem.PatientsName;
			this.StudyInstanceUid = progressItem.StudyInstanceUid;
			this.StudyDescription = progressItem.StudyDescription;
			this.StudyDate = progressItem.StudyDate;
			this.NumberOfFilesImported = progressItem.NumberOfFilesImported;
			this.LastActive = progressItem.LastActive;

			CalculateLastActiveDisplay();
		}

		public Guid ProgressIdentifier
		{
			get { return _progressIdentifier; }
		}

		public string FromAETitle
		{
			get { return _fromAETitle; }
			protected set
			{
				if (_fromAETitle != value)
					_fromAETitle = value;
			}
		}

		public string PatientId
		{
			get { return _patientId; }
			protected set
			{
				if (_patientId != value)
					_patientId = value;
			}
		}

		public string PatientsName
		{
			get { return _patientsName; }
			protected set
			{
				if (_patientsName != value)
					_patientsName = value;
			}
		}

		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			protected set
			{
				if (_studyInstanceUid != value)
					_studyInstanceUid = value;
			}
		}

		public string StudyDescription
		{
			get { return _studyDescription; }
			protected set
			{
				if (_studyDescription != value)
					_studyDescription = value;
			}
		}

		public DateTime StudyDate
		{
			get { return _studyDate; }
			protected set
			{
				if (_studyDate != value)
					_studyDate = value;
			}
		}

		public int NumberOfFilesImported
		{
			get { return _numberOfFilesImported; }
			protected set
			{
				if (_numberOfFilesImported != value)
					_numberOfFilesImported = value;
			}
		}

		public DateTime LastActive
		{
			get { return _lastActive; }
			protected set
			{
				if (!_lastActive.Equals(value))
					_lastActive = value;
			}
		}

		public string LastActiveDisplay
		{
			get { return _lastActiveDisplay; }
		}

		internal void CalculateLastActiveDisplay()
		{
			TimeSpan lastActiveSpan = DateTime.Now.Subtract(_lastActive);
			if (lastActiveSpan.Minutes < 60)
				_lastActiveDisplay = String.Format("{0} minute(s)", lastActiveSpan.Minutes);
			else if (lastActiveSpan.Hours >= 1 && lastActiveSpan.Days == 0)
				_lastActiveDisplay = String.Format("{0} hours, {1} minute(s)", lastActiveSpan.Hours, lastActiveSpan.Minutes);
			else
				_lastActiveDisplay = String.Format("{0} day(s)", lastActiveSpan.Days);
		}
	}

	[AssociateView(typeof(DicomNetworkReceiveQueueApplicationComponentViewExtensionPoint))]
	public class DicomNetworkReceiveQueueApplicationComponent : ApplicationComponent
	{
		private Table<ReceiveQueueItem> _receiveTable;
		private ISelection _selection;
		private LocalDataStoreActivityMonitor _monitor;
		private Timer _timer;

		/// <summary>
		/// Constructor
		/// </summary>
		public DicomNetworkReceiveQueueApplicationComponent(LocalDataStoreActivityMonitor monitor)
		{
			_monitor = monitor;
		}

		public override void Start()
		{
			InitializeTable();
			base.Start();
			_timer = new Timer(3000);
			_timer.Start();
			_timer.Elapsed += new ElapsedEventHandler(OnTimer);

			_monitor.ReceiveProgressUpdate += new EventHandler<ReceiveProgressUpdateEventArgs>(OnReceiveProgressUpdate);
		}

		public override void Stop()
		{
			base.Stop();
			_timer.Stop();
			_timer.Dispose();
			_timer = null;
			_monitor.ReceiveProgressUpdate -= new EventHandler<ReceiveProgressUpdateEventArgs>(OnReceiveProgressUpdate);
		}

		private void OnTimer(object sender, ElapsedEventArgs e)
		{
			foreach (ReceiveQueueItem item in _receiveTable.Items)
			{
				item.CalculateLastActiveDisplay();
				_receiveTable.Items.NotifyItemUpdated(item);
			}
		}

		private void OnReceiveProgressUpdate(object sender, ReceiveProgressUpdateEventArgs e)
		{
			int index = _receiveTable.Items.FindIndex(delegate(ReceiveQueueItem testItem)
				{
					return testItem.ProgressIdentifier.Equals(e.Item.Identifier);
				});

			if (index >= 0)
			{
				_receiveTable.Items[index].UpdateFromProgressItem(e.Item);
				_receiveTable.Items.NotifyItemUpdated(index);
			}
			else
			{
				_receiveTable.Items.Add(new ReceiveQueueItem(e.Item));
			}
		}

		private string FormatString(string input)
		{
			return String.IsNullOrEmpty(input) ? "" : input; 
		}

		private void InitializeTable()
		{
			_receiveTable = new Table<ReceiveQueueItem>();

			TableColumn<ReceiveQueueItem, string> column;

			column = new TableColumn<ReceiveQueueItem, string>(
					"From",
					delegate(ReceiveQueueItem item) { return FormatString(item.FromAETitle); },
					1.5f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					"Patient Id",
					delegate(ReceiveQueueItem item) { return FormatString(item.PatientId); },
					1.5f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					"Patient's Name",
					delegate(ReceiveQueueItem item) { return FormatString(item.PatientsName); },
					1.5f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					"Study Date",
					delegate(ReceiveQueueItem item)
					{
						if (item.StudyDate == default(DateTime))
							return "";

						return item.StudyDate.ToString(Format.DateFormat); 
					},
					1.5f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					"Study Description",
					delegate(ReceiveQueueItem item) { return FormatString(item.StudyDescription); },
					1.5f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					"Files Imported",
					delegate(ReceiveQueueItem item) { return item.NumberOfFilesImported.ToString(); },
					1.5f);

			_receiveTable.Columns.Add(column);

			//column = new TableColumn<ReceiveQueueItem, string>(
			//        "Message",
			//        delegate(ReceiveQueueItem item) { return item.StatusMessage; },
			//        1.5f);

			//_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					"Last Active",
					delegate(ReceiveQueueItem item) { return item.LastActiveDisplay;  },
					1.5f);

			_receiveTable.Columns.Add(column);
		}

		public string Title
		{
			get { return "Receive Queue"; }
		}

		public ITable ReceiveTable
		{
			get { return _receiveTable; }
		}

		public void SetSelection(ISelection selection)
		{
			_selection = selection;
		}
	}
}
