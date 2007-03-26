using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;
using System.Threading;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExtensionPoint]
	public class ReceiveQueueApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public class ReceiveQueueItem : ReceiveProgressItem
	{
		private string _lastActiveDisplay;

		private ReceiveQueueItem()
		{
			this.StudyInformation = new StudyInformation();
		}

		internal ReceiveQueueItem(ReceiveProgressItem progressItem)
			: this()
		{
			this.Identifier = progressItem.Identifier;

			UpdateFromProgressItem(progressItem);
		}

		internal void UpdateFromProgressItem(ReceiveProgressItem progressItem)
		{
			if (!this.Identifier.Equals(this.Identifier))
				throw new InvalidOperationException("the identifiers must match!");

			this.FromAETitle = progressItem.FromAETitle;
			this.NumberOfFilesReceived = progressItem.NumberOfFilesReceived;
			this.NumberOfFilesImported = progressItem.NumberOfFilesImported;
			this.LastActive = progressItem.LastActive;

			this.StudyInformation.PatientId = progressItem.StudyInformation.PatientId;
			this.StudyInformation.PatientsName = progressItem.StudyInformation.PatientsName;
			this.StudyInformation.StudyInstanceUid = progressItem.StudyInformation.StudyInstanceUid;
			this.StudyInformation.StudyDescription = progressItem.StudyInformation.StudyDescription;
			this.StudyInformation.StudyDate = progressItem.StudyInformation.StudyDate;

			CalculateLastActiveDisplay();
		}

		public string LastActiveDisplay
		{
			get { return _lastActiveDisplay; }
		}

		internal void CalculateLastActiveDisplay()
		{
			TimeSpan lastActiveSpan = DateTime.Now.Subtract(this.LastActive);
			if (lastActiveSpan.Minutes < 60)
			{
				if (lastActiveSpan.Minutes == 1)
					_lastActiveDisplay = "1 minute ago";
				else
					_lastActiveDisplay = String.Format("{0} minutes ago", lastActiveSpan.Minutes);
			}
			else if (lastActiveSpan.Hours >= 1 && lastActiveSpan.Days == 0)
			{
				if (lastActiveSpan.Hours == 1)
					_lastActiveDisplay = "1 hour, ";
				else
					_lastActiveDisplay = String.Format("{0} hours, ", lastActiveSpan.Hours);

				if (lastActiveSpan.Minutes == 1)
					_lastActiveDisplay += "1 minute ago";
				else
					_lastActiveDisplay += String.Format("{0} minutes ago", lastActiveSpan.Minutes);
			}
			else
			{
				_lastActiveDisplay = String.Format("{0} day(s)", lastActiveSpan.Days);
			}
		}
	}

	[AssociateView(typeof(ReceiveQueueApplicationComponentViewExtensionPoint))]
	public class ReceiveQueueApplicationComponent : ApplicationComponent
	{
		private Table<ReceiveQueueItem> _receiveTable;
		private ISelection _selection;
		private LocalDataStoreActivityMonitor _monitor;

		private Timer _timer;
		private InterthreadMarshaler _marshaler;

		/// <summary>
		/// Constructor
		/// </summary>
		public ReceiveQueueApplicationComponent(LocalDataStoreActivityMonitor monitor)
		{
			_monitor = monitor;
			_marshaler = new InterthreadMarshaler();
		}

		public override void Start()
		{
			InitializeTable();
			base.Start();

			TimerCallback callback = delegate(object state)
			{
				_marshaler.QueueInvoke(
					delegate
					{
						foreach (ReceiveQueueItem item in _receiveTable.Items)
						{
							item.CalculateLastActiveDisplay();
							_receiveTable.Items.NotifyItemUpdated(item);
						}
					});
			};

			_timer = new Timer(callback, null, 30000, 30000);

			_monitor.ReceiveProgressUpdate += new EventHandler<ItemEventArgs<ReceiveProgressItem>>(OnReceiveProgressUpdate);
		}

		public override void Stop()
		{
			base.Stop();
			_timer.Dispose();
			_timer = null;
			_monitor.ReceiveProgressUpdate -= new EventHandler<ItemEventArgs<ReceiveProgressItem>>(OnReceiveProgressUpdate);
		}

		private void OnReceiveProgressUpdate(object sender, ItemEventArgs<ReceiveProgressItem> e)
		{
			int index = _receiveTable.Items.FindIndex(delegate(ReceiveQueueItem testItem)
				{
					return testItem.Identifier.Equals(e.Item.Identifier);
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

			TableColumnBase<ReceiveQueueItem> column;

			column = new TableColumn<ReceiveQueueItem, string>(
					"From",
					delegate(ReceiveQueueItem item) { return FormatString(item.FromAETitle); },
					0.75f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					"Patient Id",
					delegate(ReceiveQueueItem item) { return FormatString(item.StudyInformation.PatientId); },
					1f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					"Patient's Name",
					delegate(ReceiveQueueItem item) { return FormatString(item.StudyInformation.PatientsName); },
					1.5f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					"Study Date",
					delegate(ReceiveQueueItem item)
					{
						if (item.StudyInformation.StudyDate == default(DateTime))
							return "";

						return item.StudyInformation.StudyDate.ToString(Format.DateFormat); 
					},
					0.5f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					"Study Description",
					delegate(ReceiveQueueItem item) { return FormatString(item.StudyInformation.StudyDescription); },
					2f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, int>(
					"Received",
					delegate(ReceiveQueueItem item) { return item.NumberOfFilesReceived; },
					0.5f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, int>(
					"Available",
					delegate(ReceiveQueueItem item) { return item.NumberOfFilesImported; },
					0.5f);

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
