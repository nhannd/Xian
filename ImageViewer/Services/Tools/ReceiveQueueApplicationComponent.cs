using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;

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

		public string LastActiveDisplay
		{
			get { return _lastActiveDisplay; }
		}

		internal void UpdateFromProgressItem(ReceiveProgressItem progressItem)
		{
			if (!this.Identifier.Equals(this.Identifier))
				throw new InvalidOperationException(SR.ExceptionIdentifiersMustMatch);

			base.CopyFrom(progressItem);

			CalculateLastActiveDisplay();
		}

		internal void CalculateLastActiveDisplay()
		{
			TimeSpan lastActiveSpan = DateTime.Now.Subtract(this.LastActive);
			if (lastActiveSpan.Days > 0)
			{
				if (lastActiveSpan.Days == 1)
					_lastActiveDisplay = SR.MessageOneDayAgo;
				else
					_lastActiveDisplay = String.Format(SR.FormatXDaysAgo, lastActiveSpan.Days);
			}
			else if (lastActiveSpan.Hours > 0)
			{
				if (lastActiveSpan.Hours == 1)
					_lastActiveDisplay = SR.MessageOneHourAgo;
				else
					_lastActiveDisplay = String.Format(SR.FormatXHoursAgo, lastActiveSpan.Hours);

				if (lastActiveSpan.Minutes == 1)
					_lastActiveDisplay += SR.MessageOneMinuteAgo;
				else
					_lastActiveDisplay += String.Format(SR.FormatXMinutesAgo, lastActiveSpan.Minutes);
			}
			else
			{
				if (lastActiveSpan.Minutes == 1)
					_lastActiveDisplay = SR.MessageOneDayAgo;
				else
					_lastActiveDisplay = String.Format(SR.FormatXMinutesAgo, lastActiveSpan.Minutes);
			}
		}
	}

	[AssociateView(typeof(ReceiveQueueApplicationComponentViewExtensionPoint))]
	public class ReceiveQueueApplicationComponent : ApplicationComponent
	{
		private Table<ReceiveQueueItem> _receiveTable;
		private ISelection _selection;
		private Timer _timer;

		/// <summary>
		/// Constructor
		/// </summary>
		public ReceiveQueueApplicationComponent()
		{
		}

		public override void Start()
		{
			InitializeTable();
			base.Start();

			_timer = new Timer(this.OnTimer, 30000, 30000);

			LocalDataStoreActivityMonitor.Instance.ReceiveProgressUpdate += new EventHandler<ItemEventArgs<ReceiveProgressItem>>(OnReceiveProgressUpdate);
		}

		public override void Stop()
		{
			base.Stop();

			_timer.Dispose();
			_timer = null;

			LocalDataStoreActivityMonitor.Instance.ReceiveProgressUpdate -= new EventHandler<ItemEventArgs<ReceiveProgressItem>>(OnReceiveProgressUpdate);
		}

		private void OnTimer()
		{
			foreach (ReceiveQueueItem item in _receiveTable.Items)
			{
				item.CalculateLastActiveDisplay();
				_receiveTable.Items.NotifyItemUpdated(item);
			}
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
					SR.TitleFrom,
					delegate(ReceiveQueueItem item) { return FormatString(item.FromAETitle); },
					0.75f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					SR.TitlePatientId,
					delegate(ReceiveQueueItem item) { return FormatString(item.StudyInformation.PatientId); },
					1f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					SR.TitlePatientsName,
					delegate(ReceiveQueueItem item) { return FormatString(item.StudyInformation.PatientsName); },
					1.5f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					SR.TitleStudyDate,
					delegate(ReceiveQueueItem item)
					{
						if (item.StudyInformation.StudyDate == default(DateTime))
							return "";

						return item.StudyInformation.StudyDate.ToString(Format.DateFormat); 
					},
					0.5f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					SR.TitleStudyDescription,
					delegate(ReceiveQueueItem item) { return FormatString(item.StudyInformation.StudyDescription); },
					2f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, int>(
					SR.TitleReceived,
					delegate(ReceiveQueueItem item) { return item.NumberOfFilesReceived; },
					0.5f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, int>(
					SR.TitleAvailable,
					delegate(ReceiveQueueItem item) { return item.NumberOfFilesCommittedToDataStore; },
					0.5f);

			_receiveTable.Columns.Add(column);

			//column = new TableColumn<ReceiveQueueItem, string>(
			//        "Message",
			//        delegate(ReceiveQueueItem item) { return item.StatusMessage; },
			//        1.5f);

			//_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					SR.TitleLastActive,
					delegate(ReceiveQueueItem item) { return item.LastActiveDisplay;  },
					1.5f);

			_receiveTable.Columns.Add(column);
		}

		public string Title
		{
			get { return SR.TitleReceive; }
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
