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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExtensionPoint()]
	public sealed class ReceiveQueueApplicationComponentToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IReceiveQueueApplicationComponentToolContext : IToolContext
	{
		IDesktopWindow DesktopWindow { get; }

		event EventHandler Updated;
		
		IEnumerable<ReceiveQueueItem> SelectedItems { get; }
		IEnumerable<ReceiveQueueItem> Items { get; }
		
		int NumberOfItems { get; }
		int NumberSelected { get; }

		void ClearSelected();
		void ClearAll();

		ClickHandlerDelegate DefaultActionHandler { get; set; }
	}

	[ExtensionPoint]
	public sealed class ReceiveQueueApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public class ReceiveQueueItem : ReceiveProgressItem
	{
		private PersonName _patientsName;
		private string _serverName;

		private ReceiveQueueItem()
		{
			this.StudyInformation = new StudyInformation();
			_patientsName = new PersonName("");
		}

		internal ReceiveQueueItem(ReceiveProgressItem progressItem)
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

		internal void UpdateFromProgressItem(ReceiveProgressItem progressItem)
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
						if (server.AETitle == progressItem.FromAETitle)
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

			public event EventHandler Updated
			{
				add { _component.Updated += value; }
				remove { _component.Updated -= value; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public IEnumerable<ReceiveQueueItem> SelectedItems
			{
				get
				{
					foreach (ReceiveQueueItem item in _component._selection.Items)
					{
						yield return item;
					}
				}
			}

			public IEnumerable<ReceiveQueueItem> Items
			{
				get { return _component._receiveTable.Items; }
			}

			public int NumberOfItems 
			{
				get
				{
					return _component._receiveTable.Items.Count;
				}
			}

			public int NumberSelected
			{
				get 
				{
					if (_component._selection == null || _component._selection.Item == null)
						return 0;

					return _component._selection.Items.Length;
				}
			}

			public void ClearSelected()
			{
				_component.ClearSelected();
			}

			public void ClearAll()
			{
				_component.ClearAll();
			}

			public ClickHandlerDelegate DefaultActionHandler
			{
				get { return _component._defaultActionHandler; }
				set { _component._defaultActionHandler = value; }
			}
				#endregion
		}

		private ToolSet _toolSet;
		private Table<ReceiveQueueItem> _receiveTable;
		private ISelection _selection;
		private event EventHandler _updated;
		private string _title;
		private Timer _timer;
		private ClickHandlerDelegate _defaultActionHandler;
		private ILocalDataStoreEventBroker _localDataStoreEventBroker;

		/// <summary>
		/// Constructor
		/// </summary>
		internal ReceiveQueueApplicationComponent()
		{
		}

		public event EventHandler Updated
		{
			add { _updated += value; }
			remove { _updated -= value; }
		}
		
		public override void Start()
		{
			InitializeTable();
			base.Start();

			_toolSet = new ToolSet(new ReceiveQueueApplicationComponentToolExtensionPoint(), new ReceiveQueueApplicationComponentToolContext(this));

			_timer = new Timer(this.OnTimer);
			_timer.IntervalMilliseconds = 30000;
			_timer.Start();

			_localDataStoreEventBroker = LocalDataStoreActivityMonitor.CreatEventBroker();
			_localDataStoreEventBroker.LostConnection += OnLostConnection;
			_localDataStoreEventBroker.Connected += OnConnected;
			_localDataStoreEventBroker.ReceiveProgressUpdate += OnReceiveProgressUpdate;
			SetTitle();
		}

		public override void Stop()
		{
			base.Stop();

			_timer.Dispose();
			_timer = null;

			_localDataStoreEventBroker.LostConnection -= OnLostConnection;
			_localDataStoreEventBroker.Connected -= OnConnected;
			_localDataStoreEventBroker.ReceiveProgressUpdate -= OnReceiveProgressUpdate;
			_localDataStoreEventBroker.Dispose();
		}

		private void SetTitle()
		{
			if (LocalDataStoreActivityMonitor.IsConnected)
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

		private void OnTimer(object nothing)
		{
			if (_timer == null)
				return;

			foreach (ReceiveQueueItem item in _receiveTable.Items)
			{
				//need to do this to update the 'last active' column.
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
				{
					_receiveTable.Items.Add(new ReceiveQueueItem(e.Item));
				}
			}
			
			OnUpdated();
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
					delegate(ReceiveQueueItem item)
						{
							return FormatString(!String.IsNullOrEmpty(item.ServerName) ? item.ServerName : item.FromAETitle);
						},
					0.75f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					SR.TitlePatientId,
					delegate(ReceiveQueueItem item) { return FormatString(item.StudyInformation.PatientId); },
					1f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					SR.TitlePatientsName,
					delegate(ReceiveQueueItem item) { return item.PatientsName.FormattedName; },
					1.5f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					SR.TitleStudyDate,
					delegate(ReceiveQueueItem item)
					{
						if (item.StudyInformation.StudyDate == null)
							return "";

						return ((DateTime)item.StudyInformation.StudyDate).ToString(Format.DateFormat); 
					},
					null,
					0.5f,
					delegate(ReceiveQueueItem one, ReceiveQueueItem two)
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

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					SR.TitleStudyDescription,
					delegate(ReceiveQueueItem item) { return FormatString(item.StudyInformation.StudyDescription); },
					1.5f);

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

			column = new TableColumn<ReceiveQueueItem, int>(
					SR.TitleFailed,
					delegate(ReceiveQueueItem item) { return item.TotalDataStoreCommitFailures; },
					0.5f);

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					SR.TitleLastActive,
					delegate(ReceiveQueueItem item) { return TimeSpanDisplayHelper.CalculateTimeSpanDisplay(item.LastActive); },
					null,
					1.0f,
					delegate(ReceiveQueueItem one, ReceiveQueueItem two) { return one.LastActive.CompareTo(two.LastActive); });

			// Default: Sort by last active
			_receiveTable.Sort(new TableSortParams(column, false));

			_receiveTable.Columns.Add(column);

			column = new TableColumn<ReceiveQueueItem, string>(
					SR.TitleMessage,
					delegate(ReceiveQueueItem item) { return FormatString(item.StatusMessage); },
					2.0f);

			_receiveTable.Columns.Add(column);
		}

		private void ClearItems(IEnumerable<Guid> progressIdentifiers)
		{
			CancelProgressItemInformation cancelInformation = new CancelProgressItemInformation();
			cancelInformation.CancellationFlags = CancellationFlags.Clear;
			cancelInformation.ProgressItemIdentifiers = progressIdentifiers;

			LocalDataStoreActivityMonitor.Cancel(cancelInformation);
		}

		private void OnUpdated()
		{
			EventsHelper.Fire(_updated, this, EventArgs.Empty);
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
			OnUpdated();
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

		public void ItemDoubleClick()
		{
			if (_defaultActionHandler != null)
				_defaultActionHandler();
		}
	}
}
