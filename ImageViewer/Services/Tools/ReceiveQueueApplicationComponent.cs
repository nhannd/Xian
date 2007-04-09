using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExtensionPoint()]
	public class ReceiveQueueApplicationComponentToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IReceiveQueueApplicationComponentToolContext : IToolContext
	{
		IDesktopWindow DesktopWindow { get; }

		event EventHandler Updated;

		bool ItemsSelected { get; }
		bool AnyItems { get; }
		
		void ClearSelected();
		void ClearAll();
	}

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
					_lastActiveDisplay = SR.MessageOneMinuteAgo;
				else
					_lastActiveDisplay = String.Format(SR.FormatXMinutesAgo, lastActiveSpan.Minutes);
			}
		}
	}

	[AssociateView(typeof(ReceiveQueueApplicationComponentViewExtensionPoint))]
	public class ReceiveQueueApplicationComponent : ApplicationComponent
	{
		private class ReceiveQueueApplicationComponentToolContext : IReceiveQueueApplicationComponentToolContext
		{
			private ReceiveQueueApplicationComponent _component;

			public ReceiveQueueApplicationComponentToolContext(ReceiveQueueApplicationComponent component)
			{
				_component = component;
			}

			#region IReceiveQueueApplicationComponentToolContext Members

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public bool ItemsSelected
			{
				get { return _component._selection != null && _component._selection.Item != null; }
			}

			public bool AnyItems
			{
				get { return _component._receiveTable.Items.Count > 0; }
			}

			public event EventHandler Updated
			{
				add { _component.SelectionUpdated += value; }
				remove { _component.SelectionUpdated -= value; }
			}
			
			public void ClearSelected()
			{
				_component.ClearSelected();
			}

			public void ClearAll()
			{
				_component.ClearAll();
			}

			#endregion
		}

		private ToolSet _toolSet;
		private Table<ReceiveQueueItem> _receiveTable;
		private ISelection _selection;
		private event EventHandler _selectionUpdated;
		private string _title;
		private Timer _timer;

		/// <summary>
		/// Constructor
		/// </summary>
		public ReceiveQueueApplicationComponent()
		{
		}

		public event EventHandler SelectionUpdated
		{
			add { _selectionUpdated += value; }
			remove { _selectionUpdated -= value; }
		}
		
		public override void Start()
		{
			InitializeTable();
			base.Start();

			_toolSet = new ToolSet(new ReceiveQueueApplicationComponentToolExtensionPoint(), new ReceiveQueueApplicationComponentToolContext(this));

			_timer = new Timer(this.OnTimer, 30000, 30000);

			LocalDataStoreActivityMonitor.Instance.LostConnection += new EventHandler(OnLostConnection);
			LocalDataStoreActivityMonitor.Instance.Connected += new EventHandler(OnConnected);
			LocalDataStoreActivityMonitor.Instance.ReceiveProgressUpdate += new EventHandler<ItemEventArgs<ReceiveProgressItem>>(OnReceiveProgressUpdate);
			SetTitle();
		}

		public override void Stop()
		{
			base.Stop();

			_timer.Dispose();
			_timer = null;

			LocalDataStoreActivityMonitor.Instance.LostConnection -= new EventHandler(OnLostConnection);
			LocalDataStoreActivityMonitor.Instance.Connected -= new EventHandler(OnConnected);
			LocalDataStoreActivityMonitor.Instance.ReceiveProgressUpdate -= new EventHandler<ItemEventArgs<ReceiveProgressItem>>(OnReceiveProgressUpdate);
		}

		private void SetTitle()
		{
			if (LocalDataStoreActivityMonitor.Instance.IsConnected)
				this.Title = SR.TitleReceive;
			else
				this.Title = String.Format("{0} ({1})", SR.TitleReceive, SR.MessageActivityMonitorServiceUnavailable);

		}

		private void OnConnected(object sender, EventArgs e)
		{
			SetTitle();
		}

		private void OnLostConnection(object sender, EventArgs e)
		{
			_receiveTable.Items.Clear();
			SetTitle();
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
				if (e.Item.Removed)
				{
					_receiveTable.Items.Remove(_receiveTable.Items[index]);
				}
				else
				{
					_receiveTable.Items[index].UpdateFromProgressItem(e.Item);
					_receiveTable.Items.NotifyItemUpdated(index);
				}
			}
			else
			{
				if (!e.Item.Removed)
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

		private void ClearItems(IEnumerable<Guid> progressIdentifiers)
		{
			CancelProgressItemInformation cancelInformation = new CancelProgressItemInformation();
			cancelInformation.CancellationFlags = CancellationFlags.Clear;
			cancelInformation.ProgressItemIdentifiers = progressIdentifiers;

			LocalDataStoreActivityMonitor.Instance.Cancel(cancelInformation);
		}

		public ActionModelNode ToolbarModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "receive-queue-toolbar", _toolSet.Actions); }
		}

		public ActionModelNode ContextMenuModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "receive-queue-contextmenu", _toolSet.Actions); }
		}

		public string Title
		{
			get
			{
				return _title;
			}
			protected set
			{
				if (_title != value)
				{
					_title = value;
					NotifyPropertyChanged("Title");
				}
			}
		}

		public ITable ReceiveTable
		{
			get { return _receiveTable; }
		}

		public void SetSelection(ISelection selection)
		{
			_selection = selection;
			EventsHelper.Fire(_selectionUpdated, this, EventArgs.Empty);
		}

		public void ClearSelected()
		{
			if (_selection == null)
				return;

			List<Guid> progressIdentifiers = new List<Guid>();
			foreach (ReceiveQueueItem item in _selection.Items)
			{
				progressIdentifiers.Add(item.Identifier);
			}

			ClearItems(progressIdentifiers);
		}

		public void ClearAll()
		{
			List<Guid> progressIdentifiers = new List<Guid>();
			foreach (ReceiveQueueItem item in _receiveTable.Items)
			{
				progressIdentifiers.Add(item.Identifier);
			}

			ClearItems(progressIdentifiers);
		}
	}
}
