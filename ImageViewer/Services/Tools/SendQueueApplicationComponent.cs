using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExtensionPoint]
	public class SendQueueApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public class SendQueueItem : SendProgressItem
	{
		private string _lastActiveDisplay;

		private SendQueueItem()
		{
			this.StudyInformation = new StudyInformation();
		}

		internal SendQueueItem(SendProgressItem progressItem)
			: this()
		{
			this.Identifier = progressItem.Identifier;

			UpdateFromProgressItem(progressItem);
		}

		public string LastActiveDisplay
		{
			get { return _lastActiveDisplay; }
		}

		internal void UpdateFromProgressItem(SendProgressItem progressItem)
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

	/// <summary>
	/// SendQueueApplicationComponent class
	/// </summary>
	[AssociateView(typeof(SendQueueApplicationComponentViewExtensionPoint))]
	public class SendQueueApplicationComponent : ApplicationComponent
	{
		private Table<SendQueueItem> _sendTable;
		private ISelection _selection;

		private Timer _timer;

		/// <summary>
		/// Constructor
		/// </summary>
		public SendQueueApplicationComponent()
		{
		}

		public override void Start()
		{
			InitializeTable();
			base.Start();

			_timer = new Timer(this.OnTimer, 30000, 30000);

			LocalDataStoreActivityMonitor.Instance.SendProgressUpdate += new EventHandler<ItemEventArgs<SendProgressItem>>(OnSendProgressUpdate);
		}

		public override void Stop()
		{
			base.Stop();
			_timer.Dispose();
			_timer = null;

			LocalDataStoreActivityMonitor.Instance.SendProgressUpdate -= new EventHandler<ItemEventArgs<SendProgressItem>>(OnSendProgressUpdate);
		}

		private void OnTimer()
		{
			foreach (SendQueueItem item in _sendTable.Items)
			{
				item.CalculateLastActiveDisplay();
				_sendTable.Items.NotifyItemUpdated(item);
			}
		}

		private void OnSendProgressUpdate(object sender, ItemEventArgs<SendProgressItem> e)
		{
			int index = _sendTable.Items.FindIndex(delegate(SendQueueItem testItem)
				{
					return testItem.Identifier.Equals(e.Item.Identifier);
				});

			if (index >= 0)
			{
				_sendTable.Items[index].UpdateFromProgressItem(e.Item);
				_sendTable.Items.NotifyItemUpdated(index);
			}
			else
			{
				_sendTable.Items.Add(new SendQueueItem(e.Item));
			}
		}

		private string FormatString(string input)
		{
			return String.IsNullOrEmpty(input) ? "" : input; 
		}

		private void InitializeTable()
		{
			_sendTable = new Table<SendQueueItem>();

			TableColumnBase<SendQueueItem> column;

			column = new TableColumn<SendQueueItem, string>(
					SR.TitleTo,
					delegate(SendQueueItem item) { return FormatString(item.ToAETitle); },
					0.75f);

			_sendTable.Columns.Add(column);

			column = new TableColumn<SendQueueItem, string>(
					SR.TitlePatientId,
					delegate(SendQueueItem item) { return FormatString(item.StudyInformation.PatientId); },
					1f);

			_sendTable.Columns.Add(column);

			column = new TableColumn<SendQueueItem, string>(
					SR.TitlePatientId,
					delegate(SendQueueItem item) { return FormatString(item.StudyInformation.PatientsName); },
					1.5f);

			_sendTable.Columns.Add(column);

			column = new TableColumn<SendQueueItem, string>(
					SR.TitleStudyDate,
					delegate(SendQueueItem item)
					{
						if (item.StudyInformation.StudyDate == default(DateTime))
							return "";

						return item.StudyInformation.StudyDate.ToString(Format.DateFormat); 
					},
					0.5f);

			_sendTable.Columns.Add(column);

			column = new TableColumn<SendQueueItem, string>(
					SR.TitleStudyDescription,
					delegate(SendQueueItem item) { return FormatString(item.StudyInformation.StudyDescription); },
					2f);

			_sendTable.Columns.Add(column);

			column = new TableColumn<SendQueueItem, int>(
					SR.TitleSent,
					delegate(SendQueueItem item) { return item.NumberOfFilesExported; },
					0.5f);

			_sendTable.Columns.Add(column);

			//column = new TableColumn<SendQueueItem, string>(
			//        "Message",
			//        delegate(SendQueueItem item) { return item.StatusMessage; },
			//        1.5f);

			//_receiveTable.Columns.Add(column);

			column = new TableColumn<SendQueueItem, string>(
					SR.TitleLastActive,
					delegate(SendQueueItem item) { return item.LastActiveDisplay;  },
					1.5f);

			_sendTable.Columns.Add(column);
		}

		public string Title
		{
			get { return SR.TitleSend; }
		}

		public ITable SendTable
		{
			get { return _sendTable; }
		}

		public void SetSelection(ISelection selection)
		{
			_selection = selection;
		}
	}
}
