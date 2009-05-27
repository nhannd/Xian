#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExtensionPoint()]
	public sealed class SendQueueApplicationComponentToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface ISendQueueApplicationComponentToolContext : IToolContext
	{
		IDesktopWindow DesktopWindow { get; }

		event EventHandler Updated;

		bool ItemsSelected { get; }
		bool AnyItems { get; }
		bool ShowBackgroundSends { get; set; }

		void ClearSelected();
		void ClearAll();
	}

	[ExtensionPoint]
	public sealed class SendQueueApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public class SendQueueItem : SendProgressItem
	{
		private string _serverName;
		private PersonName _patientsName;

		private SendQueueItem()
		{
			this.StudyInformation = new StudyInformation();
			_patientsName = new PersonName("");
			_serverName = null;
		}

		internal SendQueueItem(SendProgressItem progressItem)
			: this()
		{
			this.Identifier = progressItem.Identifier;

			UpdateFromProgressItem(progressItem);
		}

		public PersonName PatientsName
		{
			get { return _patientsName; }
		}

		public string ServerName
		{
			get { return _serverName; }	
		}

		internal void UpdateFromProgressItem(SendProgressItem progressItem)
		{
			if (!this.Identifier.Equals(this.Identifier))
				throw new InvalidOperationException(SR.ExceptionIdentifiersMustMatch);

			base.CopyFrom(progressItem);

			if (!String.IsNullOrEmpty(this.StudyInformation.PatientsName) && this.StudyInformation.PatientsName != _patientsName.ToString())
				_patientsName = new PersonName(this.StudyInformation.PatientsName);

			if (_serverName == null)
			{
				_serverName = "";
				try
				{
					ServerTree.ServerTree serverTree = new ServerTree.ServerTree();
					List<IServerTreeNode> servers = serverTree.FindChildServers(serverTree.RootNode.ServerGroupNode);
					foreach (Server server in servers)
					{
						if (server.AETitle == progressItem.ToAETitle)
						{
							_serverName = server.Name;
							break;
						}
					}
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex);
				}
			}
		}
	}

	/// <summary>
	/// SendQueueApplicationComponent class
	/// </summary>
	[AssociateView(typeof(SendQueueApplicationComponentViewExtensionPoint))]
	public class SendQueueApplicationComponent : ApplicationComponent
	{
		private class SendQueueApplicationComponentToolContext : ISendQueueApplicationComponentToolContext
		{
			private SendQueueApplicationComponent _component;

			public SendQueueApplicationComponentToolContext(SendQueueApplicationComponent component)
			{
				_component = component;
			}

			#region ISendQueueApplicationComponentToolContext Members

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
				get { return _component._sendTable.Items.Count > 0; }
			}

			public bool ShowBackgroundSends
			{
				get { return _component.ShowBackgroundSends; }
				set { _component.ShowBackgroundSends = value; }
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

		private string _title;
		private ToolSet _toolSet;
		private Table<SendQueueItem> _sendTable;
		private RootFilteredGroup<SendQueueItem> _filteredItems;
		private bool _showBackgroundSends;
		private ISelection _selection;
		private event EventHandler _selectionUpdated;

		private Timer _timer;
		private ILocalDataStoreEventBroker _localDataStoreEventBroker;

		/// <summary>
		/// Constructor
		/// </summary>
		internal SendQueueApplicationComponent()
		{
		}

		public event EventHandler SelectionUpdated
		{
			add { _selectionUpdated += value; }
			remove { _selectionUpdated -= value; }
		}
		
		public override void Start()
		{
			_filteredItems = new RootFilteredGroup<SendQueueItem>();
			_filteredItems.ItemAdded += FilteredItemAdded;
			_filteredItems.ItemRemoved += FilteredItemRemoved;

			InitializeTable();
			UpdateTable();

			base.Start();

			_toolSet = new ToolSet(new SendQueueApplicationComponentToolExtensionPoint(), new SendQueueApplicationComponentToolContext(this));

			_timer = new Timer(this.OnTimer);
			_timer.IntervalMilliseconds = 30000;
			_timer.Start();

			_localDataStoreEventBroker = LocalDataStoreActivityMonitor.CreatEventBroker();
			_localDataStoreEventBroker.LostConnection += OnLostConnection;
			_localDataStoreEventBroker.Connected += OnConnected;
			_localDataStoreEventBroker.SendProgressUpdate += OnSendProgressUpdate;

			SetTitle();
		}

		private void FilteredItemAdded(object sender, ItemEventArgs<SendQueueItem> e)
		{
			_sendTable.Items.Add(e.Item);
		}

		private void FilteredItemRemoved(object sender, ItemEventArgs<SendQueueItem> e)
		{
			_sendTable.Items.Remove(e.Item);
		}

		private void UpdateTable()
		{
			if (_showBackgroundSends)
			{
				_filteredItems.ChildGroups.Clear();
			}
			else
			{
				if (_filteredItems.ChildGroups.Count > 0)
					return;

				_filteredItems.ChildGroups.Add(new FilteredGroup<SendQueueItem>("Background", "Background",
												delegate(SendQueueItem item) { return item.IsBackground; }));
			}
		}

		public override void Stop()
		{
			base.Stop();
			_timer.Dispose();
			_timer = null;

			_localDataStoreEventBroker.LostConnection -= OnLostConnection;
			_localDataStoreEventBroker.Connected -= OnConnected;
			_localDataStoreEventBroker.SendProgressUpdate -= OnSendProgressUpdate;
			_localDataStoreEventBroker.Dispose();
		}

		private void SetTitle()
		{
			if (LocalDataStoreActivityMonitor.IsConnected)
				this.Title = SR.TitleSend;
			else
				this.Title = String.Format("{0} ({1})", SR.TitleSend, SR.MessageActivityMonitorServiceUnavailable);
		}

		private void OnConnected(object sender, EventArgs e)
		{
			SetTitle();
		}

		private void OnLostConnection(object sender, EventArgs e)
		{
			_filteredItems.Clear();
			SetTitle();
		}

		private void OnTimer(object nothing)
		{
			if (_timer == null)
				return;

			foreach (SendQueueItem item in _sendTable.Items)
			{
				//need to do this to update the 'last active' column.
				_sendTable.Items.NotifyItemUpdated(item);
			}
		}

		private void OnSendProgressUpdate(object sender, ItemEventArgs<SendProgressItem> e)
		{
			SendQueueItem foundItem = CollectionUtils.SelectFirst(_filteredItems.GetAllItems(), delegate(SendQueueItem testItem)
				{
					return testItem.Identifier.Equals(e.Item.Identifier);
				});

			if (foundItem != null)
			{
				if (e.Item.Removed)
				{
					_filteredItems.Remove(foundItem);
				}
				else
				{
					foundItem.UpdateFromProgressItem(e.Item);
					if (_sendTable.Items.Contains(foundItem))
						_sendTable.Items.NotifyItemUpdated(foundItem);
				}
			}
			else
			{
				if (!e.Item.Removed)
				{
					_filteredItems.Add(new SendQueueItem(e.Item));
				}
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
					delegate(SendQueueItem item)
					{
						return FormatString(!String.IsNullOrEmpty(item.ServerName) ? item.ServerName : item.ToAETitle);
					},
					0.75f);

			_sendTable.Columns.Add(column);

			column = new TableColumn<SendQueueItem, string>(
					SR.TitlePatientId,
					delegate(SendQueueItem item) { return FormatString(item.StudyInformation.PatientId); },
					1f);

			_sendTable.Columns.Add(column);

			column = new TableColumn<SendQueueItem, string>(
					SR.TitlePatientsName,
					delegate(SendQueueItem item) { return item.PatientsName.FormattedName; },
					1.5f);

			_sendTable.Columns.Add(column);

			column = new TableColumn<SendQueueItem, string>(
					SR.TitleStudyDate,
					delegate(SendQueueItem item)
					{
						if (item.StudyInformation.StudyDate == null)
							return "";

						return ((DateTime)item.StudyInformation.StudyDate).ToString(Format.DateFormat);
					},
					null,
					0.5f,
					delegate(SendQueueItem one, SendQueueItem two)
					{
						if (one.StudyInformation.StudyDate == null)
						{
							if (two.StudyInformation.StudyDate == null)
								return 0;

							return -1;
						}
						else if (two.StudyInformation.StudyDate == null)
							return 1;

						DateTime date1 = (DateTime)one.StudyInformation.StudyDate;
						DateTime date2 = (DateTime)two.StudyInformation.StudyDate;
						return date1.CompareTo(date2);
					});

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

			column = new TableColumn<SendQueueItem, string>(
					SR.TitleLastActive,
					delegate(SendQueueItem item) { return TimeSpanDisplayHelper.CalculateTimeSpanDisplay(item.LastActive); },
					null,
					1.5f,
					delegate(SendQueueItem one, SendQueueItem two) { return one.LastActive.CompareTo(two.LastActive); });

			// Default: Sort by last active
			_sendTable.Sort(new TableSortParams(column, false));

			_sendTable.Columns.Add(column);

			column = new TableColumn<SendQueueItem, string>(
				SR.TitleMessage,
				delegate(SendQueueItem item) { return FormatString(item.StatusMessage); },
				2.0f);

			_sendTable.Columns.Add(column);
		}

		private void ClearItems(IEnumerable<Guid> progressIdentifiers)
		{
			CancelProgressItemInformation cancelInformation = new CancelProgressItemInformation();
			cancelInformation.CancellationFlags = CancellationFlags.Clear;
			cancelInformation.ProgressItemIdentifiers = progressIdentifiers;

			LocalDataStoreActivityMonitor.Cancel(cancelInformation);
		}

		public ActionModelNode ToolbarModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "send-queue-toolbar", _toolSet.Actions); }
		}

		public ActionModelNode ContextMenuModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "send-queue-contextmenu", _toolSet.Actions); }
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

		public ITable SendTable
		{
			get { return _sendTable; }
		}

		public bool ShowBackgroundSends
		{
			get { return _showBackgroundSends; }
			set
			{
				if (_showBackgroundSends == value)
					return;

				_showBackgroundSends = value;
				UpdateTable();
				NotifyPropertyChanged("ShowBackgroundSends");
			}
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
			foreach(SendQueueItem item in _selection.Items)
			{
				progressIdentifiers.Add(item.Identifier);
			}

			ClearItems(progressIdentifiers);
		}

		public void ClearAll()
		{
			List<Guid> progressIdentifiers = new List<Guid>();
			foreach (SendQueueItem item in _filteredItems.GetAllItems())
			{
				progressIdentifiers.Add(item.Identifier);
			}

			ClearItems(progressIdentifiers);
		}
	}
}
