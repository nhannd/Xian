#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using Action = ClearCanvas.Desktop.Actions.Action;
using Timer = ClearCanvas.Common.Utilities.Timer;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	[ExtensionPoint]
	public sealed class ActivityMonitorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

	[AssociateView(typeof(ActivityMonitorComponentViewExtensionPoint))]
	public class ActivityMonitorComponent : ApplicationComponent
	{
		#region ConnectionState classes

		abstract class ConnectionState
		{
			protected ConnectionState(ActivityMonitorComponent component)
			{
				Component = component;
			}

			public abstract ConnectionState Update();

			protected ActivityMonitorComponent Component { get; private set; }
		}

		class ConnectedState : ConnectionState
		{
			public ConnectedState(ActivityMonitorComponent component)
				: base(component)
			{
			}

			public override ConnectionState Update()
			{
				if (this.Component.ActivityMonitor.IsConnected)
					return this;

				this.Component.ActivityMonitor.WorkItemsChanged -= this.Component.WorkItemsChanged;
				return new DisconnectedState(this.Component);
			}
		}

		class DisconnectedState : ConnectionState
		{
			public DisconnectedState(ActivityMonitorComponent component)
				: base(component)
			{
			}

			public override ConnectionState Update()
			{
				if (!this.Component.ActivityMonitor.IsConnected)
					return this;

				// whatever is on the screen is out-of-date and should be refreshed
				this.Component.ActivityMonitor.WorkItemsChanged += this.Component.WorkItemsChanged;
				this.Component.RefreshInternal();
				return new ConnectedState(this.Component);
			}
		}

		#endregion

		#region WorkItem class

		internal class WorkItem
		{
			private readonly WorkItemData _data;

			public WorkItem(WorkItemData data)
			{
				_data = data;
			}

			public WorkItemData Data
			{
				get { return _data; }
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

			public bool CancellationCanResultInPartialStudy
			{
				get { return _data.Request.CancellationCanResultInPartialStudy; }
			}

			public string PatientInfo
			{
				get
				{
					var p = _data.Patient;
					if (p == null)
						return null;
					return string.Format("{0} \u00B7 {1}", new PersonName(p.PatientsName).FormattedName, p.PatientId);
				}
			}

			public string StudyDate
			{
				get { return _data.Study != null ? _data.Study.StudyDate : null; }
			}

			public string StudyTime
			{
				get { return _data.Study != null ? _data.Study.StudyTime : null; }
			}

			public string AccessionNumber
			{
				get { return _data.Study != null ? _data.Study.AccessionNumber : null; }
			}

			public string StudyDescription
			{
				get { return _data.Study != null ? _data.Study.StudyDescription : null; }
			}

			public string StudyInfo
			{
				get
				{
					if (_data.Study == null) return string.Empty;

					// TODO (Marmot) Code taken from DicomImageSetDescriptor.GetName()  Should be consolidated?  TBD
					DateTime studyDate;
					DateParser.Parse(StudyDate, out studyDate);
					DateTime studyTime;
					TimeParser.Parse(StudyTime, out studyTime);

					string modalitiesInStudy = null;
					if (_data.Study != null && _data.Study.ModalitiesInStudy != null)
						modalitiesInStudy = StringUtilities.Combine(_data.Study.ModalitiesInStudy, ", ");

					var nameBuilder = new StringBuilder();
					nameBuilder.AppendFormat("{0} {1}", studyDate.ToString(Format.DateFormat),
														studyTime.ToString(Format.TimeFormat));

					if (!String.IsNullOrEmpty(AccessionNumber))
						nameBuilder.AppendFormat(", A#: {0}", AccessionNumber);

					nameBuilder.AppendFormat(", [{0}] {1}", modalitiesInStudy ?? string.Empty, StudyDescription);

					return nameBuilder.ToString();
				}
			}

			public WorkItemPriorityEnum Priority
			{
				get { return _data.Priority; }
			}

			public WorkItemStatusEnum Status
			{
				get { return _data.Status; }
			}

			public string ActivityType
			{
				get { return _data.Request != null ? _data.Request.ActivityTypeString : ReindexRequest.WorkItemTypeString; }
			}

			public string ActivityDescription
			{
				get { return _data.Request != null ? _data.Request.ActivityDescription : null; }
			}

			public DateTime ScheduledTime
			{
				get { return _data.ScheduledTime; }
			}

			public string ProgressStatus
			{
				get
				{
                    if (_data.Progress == null) return null;

                    if (!string.IsNullOrEmpty(ProgressStatusDescription))
                        return string.Format("{0} [{1}]", _data.Progress.Status, ProgressStatusDescription);
                    return _data.Progress.Status;
				}
			}

			public string ProgressStatusDescription
			{
				get { return _data.Progress != null ? _data.Progress.StatusDetails : null; }
			}

			public decimal ProgressValue
			{
				get { return _data.Progress != null ? _data.Progress.PercentComplete : 0; }
			}

            public bool IsCancelable
            {
                get { return _data.Progress == null || _data.Progress.IsCancelable; }
            }

			public IconSet ProgressIcon
			{
				get { return GetProgressIcon(_data.Progress, _data.Status); }
			}

			public bool ContainsText(string text)
			{
				return ContainsText(text,
									w => w.PatientInfo,
									w => w.StudyInfo,
									w => w.ActivityDescription,
									w => w.ProgressStatus,
									w => w.ProgressStatusDescription,
									w => w.ActivityType,
									w => w.Priority.GetDescription(),
									w => w.Status.GetDescription());
			}

			private bool ContainsText(string text, params Func<WorkItem, string>[] fields)
			{
				text = text.ToLower();
				return fields.Any(f =>
									{
										var value = f(this);
										return !String.IsNullOrEmpty(value) && value.ToLower().Contains(text);
									});
			}
		}

		#endregion

		#region WorkItemUpdateManager class

		internal class WorkItemUpdateManager
		{
			private readonly ItemCollection<WorkItem> _items;
			private readonly Dictionary<long, WorkItem> _failures;
			private readonly Predicate<WorkItem> _filter;
		    private readonly System.Action _failedItemCountChanged;

		    public WorkItemUpdateManager(ItemCollection<WorkItem> itemCollection, Predicate<WorkItem> filter, System.Action failedItemCountChanged)
			{
				_items = itemCollection;
				_filter = filter;
			    _failedItemCountChanged = failedItemCountChanged;
			    _failures = new Dictionary<long, WorkItem>();
			}

			public int FailedItemCount
			{
				get { return _failures.Values.Count(item => item.Status == WorkItemStatusEnum.Failed); }
			}

			public void Clear()
			{
				_items.Clear();
			    if (_failures.Count == 0)
                    return;

			    _failures.Clear();
			    _failedItemCountChanged();
			}

			public void Update(IEnumerable<WorkItem> items)
			{
				var adds = new List<WorkItem>();
				foreach (var item in items)
				{
				    WorkItem theItem = item;
                    var index = _items.FindIndex(w => w.Id == theItem.Id);
					if (index > -1)
					{
						// the item is currently in the list
						// if the item is marked deleted, or if it no longer meets the filter criteria, remove it
						// otherwise update it
						if (item.Status == WorkItemStatusEnum.Deleted || item.Status == WorkItemStatusEnum.DeleteInProgress || !Include(item))
							_items.RemoveAt(index);
						else
							_items[index] = item;
					}
					else
					{
						// the item is not currently in the list
						// if not deleted and it meets the filter criteria, add it
						if (item.Status != WorkItemStatusEnum.Deleted && item.Status != WorkItemStatusEnum.DeleteInProgress && Include(item))
							adds.Add(item);
					}

                    int failureCount = _failures.Count;

                    if (item.Status != WorkItemStatusEnum.Failed) //remove anything that's not failed, which includes restarted items.
                        _failures.Remove(item.Id);
					else
						_failures[item.Id] = item;

                    if (_failures.Count != failureCount)
                        _failedItemCountChanged();
				}

				// more efficient to add everything new at once (so GUI only re-draws once)
				_items.AddRange(adds);
			}

			private bool Include(WorkItem item)
			{
				return _filter(item);
			}
		}

		#endregion

		#region StudyCountWatcher class

		class StudyCountWatcher : IDisposable
		{
			private int _studyCount = -1;
			private readonly Timer _throttleTimer;
			private readonly System.Action _onChanged;

			public StudyCountWatcher(System.Action onChanged)
			{
				_throttleTimer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(5));
				_onChanged = onChanged;
			}

			public void Invalidate()
			{
				_throttleTimer.Start();
			}

			public void Dispose()
			{
				_throttleTimer.Dispose();
			}

			public int StudyCount
			{
				get
				{
					if (_studyCount == -1)
					{
						try
						{
							Platform.GetService<IStudyStoreQuery>(s => _studyCount = s.GetStudyCount(new GetStudyCountRequest()).StudyCount);
						}
						catch (Exception e)
						{
							//TODO (Marmot): Show something to the user?
							Platform.Log(LogLevel.Error, e, "Error getting the count of studies in the local store.");
						}
					}
					return _studyCount;
				}
			}

			private void OnTimerElapsed(object state)
			{
				_throttleTimer.Stop();
				_studyCount = -1;	// invalidate
				_onChanged();
			}
		}

		#endregion

		#region LocalServerWatcher class

		class LocalServerWatcher : IDisposable
		{
		    private class AsyncState
		    {
		        public SynchronizationContext SynchronizationContext;
		        public StorageConfiguration CurrentStorageConfiguration;
                public DicomServerConfiguration CurrentDicomServerConfiguration;
            }

			private Timer _refreshTimer;

			private DicomServerConfiguration _dicomServerConfiguration;
			private StorageConfiguration _storageConfiguration;

			private readonly System.Action _dicomServerConfigurationChanged;
			private readonly System.Action _studyStorageConfigurationChanged;
			private readonly System.Action _diskSpaceUsageChanged;

		    public LocalServerWatcher(System.Action dicomServerConfigurationChanged, System.Action studyStorageConfigurationChanged, System.Action diskSpaceUsageChanged)
			{
				_dicomServerConfigurationChanged = dicomServerConfigurationChanged;
				_studyStorageConfigurationChanged = studyStorageConfigurationChanged;
				_diskSpaceUsageChanged = diskSpaceUsageChanged;
			}

			public void Start()
			{
                if (_refreshTimer != null)
                    return;

                _refreshTimer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(20));
                _refreshTimer.Start();
            }

			public void Dispose()
			{
                if (_refreshTimer == null)
                    return;

				_refreshTimer.Dispose();
			    _refreshTimer = null;
			}

		    private DicomServerConfiguration DicomServerConfiguration
		    {
				get { return _dicomServerConfiguration ?? (_dicomServerConfiguration = DicomServer.GetConfiguration()); }
				set
		        {
					if (Equals(value, _dicomServerConfiguration))
						return;

					_dicomServerConfiguration = value;
					_dicomServerConfigurationChanged();
		        }
		    }

			private StorageConfiguration StorageConfiguration
			{
				get { return _storageConfiguration ?? (_storageConfiguration = StudyStore.GetConfiguration()); }
				set
				{
					if (Equals(value, _storageConfiguration))
						return;

					_storageConfiguration = value;
					_studyStorageConfigurationChanged();
				}
			}

            #region Dicom Server
            public string AETitle
		    {
                get { return DicomServerConfiguration.AETitle; }
		    }

		    public string HostName
		    {
		        get { return DicomServerConfiguration.HostName; }
		    }

		    public int Port
		    {
                get { return DicomServerConfiguration.Port; }
		    }

            #endregion

            #region Study Storage Configuration

            public string FileStoreDirectory
			{
				get { return StorageConfiguration.FileStoreDirectory; }
			}

		    //TODO (Marmot): at some point, we may want to show this on the meter.
            //public double MinimumFreeSpacePercent
            //{
            //    get { return StorageConfiguration.MinimumFreeSpacePercent; }
            //}

		    public bool IsMaximumDiskspaceUsageExceeded
            {
                get { return StorageConfiguration.IsMaximumUsedSpaceExceeded; }
            }

            #endregion

            #region Disk Space

            public long DiskSpaceTotal
            {
                get { return StorageConfiguration.FileStoreDiskSpace.TotalSpace; }
            }

            public long DiskSpaceUsed
		    {
                get { return StorageConfiguration.FileStoreDiskSpace.UsedSpace; }
		    }

            public double DiskSpaceUsedPercent
            {
                get { return StorageConfiguration.FileStoreDiskSpace.UsedSpacePercent; }
            }

            #endregion

            private void OnTimerElapsed(object state)
			{
			    var asyncState = new AsyncState
			                    {
			                        SynchronizationContext = SynchronizationContext.Current,
			                        CurrentDicomServerConfiguration = _dicomServerConfiguration,
			                        CurrentStorageConfiguration = _storageConfiguration
			                    };

                ThreadPool.QueueUserWorkItem(UpdateConfigurationsAsync, asyncState);
		    }

		    private void UpdateConfigurationsAsync(object state)
		    {
		        var asyncState = (AsyncState) state;
                //This stuff can actually add up over time because it's hitting the database so frequently.
                //Better to do it asynchronously.
                var storageConfiguration = StudyStore.GetConfiguration();
                var dicomServerConfiguration = DicomServer.GetConfiguration();

                if (!Equals(asyncState.CurrentDicomServerConfiguration, dicomServerConfiguration))
                    asyncState.SynchronizationContext.Post(ignore => DicomServerConfigurationChanged(dicomServerConfiguration), null);

                var storageConfigurationChanged = HasStorageConfigurationChanged(asyncState.CurrentStorageConfiguration, storageConfiguration);
                //Access all the disk usage properties here, since they can take some time.
                bool diskUsageChanged = HasDiskUsageChanged(asyncState.CurrentStorageConfiguration, storageConfiguration);

                if (storageConfigurationChanged)
                    asyncState.SynchronizationContext.Post(ignore => StorageConfigurationChanged(storageConfiguration), null);
                else if (diskUsageChanged)
                    asyncState.SynchronizationContext.Post(ignore => DiskUsageChanged(storageConfiguration), null);
		    }

            private bool HasStorageConfigurationChanged(StorageConfiguration old, StorageConfiguration @new)
            {
                return old == null
                       || old.FileStoreDirectory != @new.FileStoreDirectory
                       || Math.Abs(old.MinimumFreeSpacePercent - @new.MinimumFreeSpacePercent) > 0.0001;
            }

            private bool HasDiskUsageChanged(StorageConfiguration old, StorageConfiguration @new)
            {
                //We don't want to cause updates when the disk usage has changed non-significantly.
                return old == null
                       || Math.Abs(old.FileStoreDiskSpace.UsedSpacePercent - @new.FileStoreDiskSpace.UsedSpacePercent) > 0.0001
                       || old.FileStoreDiskSpace.TotalSpace != @new.FileStoreDiskSpace.TotalSpace;
            }

		    private void DicomServerConfigurationChanged(DicomServerConfiguration dicomServerConfiguration)
		    {
                if (_refreshTimer == null)
                    return;

		        _dicomServerConfiguration = dicomServerConfiguration;
		        _dicomServerConfigurationChanged();
		    }

            private void StorageConfigurationChanged(StorageConfiguration storageConfiguration)
            {
                if (_refreshTimer == null)
                    return;

                _storageConfiguration = storageConfiguration;
                _studyStorageConfigurationChanged();
                _diskSpaceUsageChanged(); //The file store directory may have changed, so just update this, too.
            }

            private void DiskUsageChanged(StorageConfiguration storageConfiguration)
            {
                if (_refreshTimer == null)
                    return;

                //We still reassign this value, even if it's only the disk usage that's changed because
                //the DiskSpace class caches its values, so we need to swap it out for a new one.
                _storageConfiguration = storageConfiguration;
                _diskSpaceUsageChanged();
            }
        }

		#endregion

		#region WorkItemActionModel

		class WorkItemActionModel : SimpleActionModel
		{
			private readonly ActivityMonitorComponent _owner;
			private readonly IItemCollection<WorkItem> _workItems;
			private IList<long> _selectedWorkItemIDs;

			public WorkItemActionModel(IItemCollection<WorkItem> workItems, ActivityMonitorComponent owner)
				: base(new ApplicationThemeResourceResolver(typeof(ActivityMonitorComponent).Assembly, new ApplicationThemeResourceResolver(typeof(CrudActionModel).Assembly)))
			{
				_workItems = workItems;
				_owner = owner;
				this.CancelAction = AddAction("cancel", SR.MenuStopWorkItem, "CancelToolSmall.png", SR.TooltipStopWorkItem, CancelSelectedWorkItems);
				this.RestartAction = AddAction("restart", SR.MenuRestartWorkItem, "RestartToolSmall.png", SR.TooltipRestartWorkItem, RestartSelectedWorkItems);
				this.DeleteAction = AddAction("delete", SR.MenuDeleteWorkItem, "DeleteToolSmall.png", SR.TooltipDeleteWorkItem, DeleteSelectedWorkItems);
				this.StatAction = AddAction("stat", SR.MenuStatWorkItem, "StatToolSmall.png", SR.TooltipStatWorkItem, StatSelectedWorkItems);
			}

			public IList<long> SelectedWorkItemIDs
			{
				get { return _selectedWorkItemIDs ?? (_selectedWorkItemIDs = new List<long>()); }
				set
				{
					_selectedWorkItemIDs = value;
					UpdateActionEnablement();
				}
			}

			private IEnumerable<WorkItem> SelectedWorkItems
			{
				get { return _workItems.Where(w => SelectedWorkItemIDs.Contains(w.Id)); }
			}

			public void OnWorkItemsChanged(IEnumerable<WorkItem> items)
			{
				if (items.Any(item => SelectedWorkItemIDs.Contains(item.Id)))
					UpdateActionEnablement();
			}

			private void UpdateActionEnablement()
			{
				var items = this.SelectedWorkItems.ToList();
				var nonEmpty = items.Count > 0;

				DeleteAction.Enabled = nonEmpty && items.All(IsDeletable);
				CancelAction.Enabled = nonEmpty && items.All(IsCancelable);
				RestartAction.Enabled = nonEmpty && items.All(IsRestartable);
				StatAction.Enabled = nonEmpty && items.All(IsStatable);
			}

			private bool IsDeletable(WorkItem w)
			{
				return w.Status == WorkItemStatusEnum.Complete
                       || (w.Status == WorkItemStatusEnum.Pending && w.IsCancelable)
					   || w.Status == WorkItemStatusEnum.Failed
					   || w.Status == WorkItemStatusEnum.Canceled;
			}
			private bool IsCancelable(WorkItem w)
			{
				return (w.Status == WorkItemStatusEnum.InProgress
						 || w.Status == WorkItemStatusEnum.Idle
						 || w.Status == WorkItemStatusEnum.Pending) && w.IsCancelable;
			}
			private bool IsRestartable(WorkItem w)
			{
                // Cannot restart Delete Study Requests
				return (w.Status == WorkItemStatusEnum.Canceled
						 || w.Status == WorkItemStatusEnum.Failed) 
                         && !w.Data.Request.WorkItemType.Equals(DeleteStudyRequest.WorkItemTypeString);
			}
			private bool IsStatable(WorkItem w)
			{
				// if the item is already stat, can't change it
				if (w.Priority == WorkItemPriorityEnum.Stat)
					return false;

				// item must be in Pending or Idle status
				return w.Status == WorkItemStatusEnum.Pending
					   || w.Status == WorkItemStatusEnum.Idle;
			}

			private void RestartSelectedWorkItems()
			{
				try
				{
					var client = new WorkItemBridge();
					ProcessItems(SelectedWorkItems, workItem =>
					{
						client.WorkItem = workItem.Data;
						client.Reset();
					}, false);
				}
				catch (EndpointNotFoundException)
				{
					HandleEndpointNotFound();
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, _owner.Host.DesktopWindow);
				}
			}

			private void CancelSelectedWorkItems()
			{
				var items = this.SelectedWorkItems.ToList();
				if (items.Any(item => item.CancellationCanResultInPartialStudy))
				{
					var action = _owner.Host.ShowMessageBox(SR.MessageConfirmCancelWorkItems, MessageBoxActions.YesNo);
					if (action == DialogBoxAction.No)
						return;
				}

				try
				{
					var client = new WorkItemBridge();
					ProcessItems(items, workItem =>
					{
						client.WorkItem = workItem.Data;
						client.Cancel();
					}, false);
				}
				catch (EndpointNotFoundException)
				{
					HandleEndpointNotFound();
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, _owner.Host.DesktopWindow);
				}
			}

			private void DeleteSelectedWorkItems()
			{
				try
				{
					var client = new WorkItemBridge();
					ProcessItems(SelectedWorkItems, workItem =>
					{
						client.WorkItem = workItem.Data;
						client.Delete();
					}, false);
				}
				catch (EndpointNotFoundException)
				{
					HandleEndpointNotFound();
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, _owner.Host.DesktopWindow);
				}
			}

			private void StatSelectedWorkItems()
			{
				try
				{
					var client = new WorkItemBridge();
					ProcessItems(SelectedWorkItems, workItem =>
					{
						client.WorkItem = workItem.Data;
						client.Reprioritize(WorkItemPriorityEnum.Stat);
					}, false);
				}
				catch (EndpointNotFoundException)
				{
					HandleEndpointNotFound();
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, _owner.Host.DesktopWindow);
				}
			}

			private void ProcessItems<T>(IEnumerable<T> items, Action<T> processAction, bool cancelable)
			{
				var itemsToProcess = items.ToList();

				// if just one item, process it synchronously
				if (itemsToProcess.Count == 1)
				{
					processAction(itemsToProcess[0]);
					return;
				}

				// otherwise do progress dialog
				ProgressDialog.Show(_owner.Host.DesktopWindow,
					itemsToProcess,
					(item, i) =>
					{
						processAction(item);
						return string.Format(SR.MessageProcessedItemsProgress, i + 1, itemsToProcess.Count);
					},
					cancelable);
			}

			private void HandleEndpointNotFound()
			{
				_owner.Host.ShowMessageBox(SR.MessageLocalServerNotRunning, MessageBoxActions.Ok);
			}

			private Action DeleteAction { get; set; }
			private Action CancelAction { get; set; }
			private Action RestartAction { get; set; }
			private Action StatAction { get; set; }
		}

		#endregion

		enum StatusFilterValue
		{
			Active,
			Complete,
			Stopped,
			Failed
		}

		private static readonly object NoFilter = new object();

		private readonly WorkItemActionModel _workItemActionModel;
		private readonly Table<WorkItem> _workItems = new Table<WorkItem>();
		private readonly WorkItemUpdateManager _workItemManager;

		private ConnectionState _connectionState;
		private StatusFilterValue? _statusFilter;
		private string _activityFilter;

		private string _textFilter;
		private readonly Timer _textFilterTimer;

		private readonly LocalServerWatcher _localServerWatcher;
		private readonly StudyCountWatcher _studyCountWatcher;

		private readonly IActivityMonitorQuickLinkHandler[] _linkHandlers;

		public ActivityMonitorComponent()
		{
			_connectionState = new DisconnectedState(this);
			_textFilterTimer = new Timer(OnTextFilterTimerElapsed, null, 1000);
			_localServerWatcher = new LocalServerWatcher(OnDicomServerConfigurationChanged, OnStorageConfigurationChanged, OnDiskspaceChanged);
			_studyCountWatcher = new StudyCountWatcher(OnStudyCountChanged);
		    _workItemManager = new WorkItemUpdateManager(_workItems.Items, Include, OnFailureCountChanged);
			_workItemActionModel = new WorkItemActionModel(_workItems.Items, this);
			_linkHandlers = (new ActivityMonitorQuickLinkHandlerExtensionPoint()).CreateExtensions().Cast<IActivityMonitorQuickLinkHandler>().ToArray();
		}

	    public override void Start()
		{
			base.Start();

            _workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnPatient, w => w.PatientInfo) { WidthFactor = .7f });
            _workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnStudy, w => w.StudyInfo) { WidthFactor = .9f });
			_workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnActivityDescription, w => w.ActivityDescription) { WidthFactor = .7f });
			_workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnStatus, w => w.Status.GetDescription())
									   {
										   WidthFactor = .3f,
										   TooltipTextProvider = w => string.IsNullOrEmpty(w.ProgressStatusDescription)
																		? string.Empty
																		: w.ProgressStatusDescription
									   });
			_workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnStatusDescription, w => w.ProgressStatus){WidthFactor = 1.5f});
			_workItems.Columns.Add(new DateTimeTableColumn<WorkItem>(SR.ColumnScheduledTime, w => w.ScheduledTime) { WidthFactor = .5f });
			_workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnPriority, w => w.Priority.GetDescription()) { WidthFactor = .25f });
			_workItems.Columns.Add(new TableColumn<WorkItem, IconSet>(SR.ColumnProgress, w => w.ProgressIcon) { WidthFactor = .4f, Comparison = (x, y) => x.ProgressValue.CompareTo(y.ProgressValue) });

			this.ActivityMonitor = WorkItemActivityMonitor.Create(true);
			_connectionState = _connectionState.Update();

			this.ActivityMonitor.IsConnectedChanged += ActivityMonitorIsConnectedChanged;

			_localServerWatcher.Start();
		}

		public override void Stop()
		{
			ActivityMonitor.WorkItemsChanged -= WorkItemsChanged;
			ActivityMonitor.IsConnectedChanged -= ActivityMonitorIsConnectedChanged;
			ActivityMonitor.Dispose();
			ActivityMonitor = null;

			_textFilterTimer.Dispose();
			_localServerWatcher.Dispose();
			_studyCountWatcher.Dispose();

			base.Stop();
		}

		#region Presentation Model

		public bool IsConnected
		{
			get { return _connectionState is ConnectedState; }
		}

		public string AeTitle
		{
			get { return _localServerWatcher.AETitle; }
		}

		public string HostName
		{
			get { return _localServerWatcher.HostName; }
		}

		public int Port
		{
			get { return _localServerWatcher.Port; }
		}

		public string FileStore
		{
			get { return _localServerWatcher.FileStoreDirectory; }
		}

		public string DiskspaceUsed
		{
			get
			{
				return string.Format(SR.DiskspaceTemplate,
										Diskspace.FormatBytes(_localServerWatcher.DiskSpaceUsed),
										Diskspace.FormatBytes(_localServerWatcher.DiskSpaceTotal),
										_localServerWatcher.DiskSpaceUsedPercent.ToString("F1"));
			}
		}

		public int DiskspaceUsedPercent
		{
            get { return (int)Math.Round(_localServerWatcher.DiskSpaceUsedPercent); }
		}

		public bool IsMaximumDiskspaceUsageExceeded
		{
			get { return _localServerWatcher.IsMaximumDiskspaceUsageExceeded; }
		}

		public string DiskSpaceWarningMessage
		{
			get
			{
				if (!IsMaximumDiskspaceUsageExceeded)
					return String.Empty;

				return SR.MessageMaximumDiskUsageExceeded;
			}
		}

		public string DiskSpaceWarningDescription
		{
			get
			{
				if (!IsMaximumDiskspaceUsageExceeded)
					return String.Empty;

				return SR.DescriptionMaximumDiskUsageExceeded;
			}
		}

		public int TotalStudies
		{
			get { return _studyCountWatcher.StudyCount; }
		}

		public int Failures
		{
			get { return _workItemManager.FailedItemCount; }
		}

		public ActionModelNode WorkItemActions
		{
			get { return _workItemActionModel; }
		}

		public ITable WorkItemTable
		{
			get { return _workItems; }
		}

		public void SetWorkItemSelection(ISelection selection)
		{
			_workItemActionModel.SelectedWorkItemIDs = selection.Items.Cast<WorkItem>().Select(w => w.Id).ToList();
		}

		public IList StatusFilterChoices
		{
			get { return new[] { NoFilter }.Concat(Enum.GetValues(typeof(StatusFilterValue)).Cast<object>().OrderBy<object, string>(FormatStatusFilter)).ToList(); }
		}

		public string FormatStatusFilter(object value)
		{
			if (value == NoFilter)
				return SR.NoFilterItem;
			switch ((StatusFilterValue)value)
			{
				case StatusFilterValue.Active:
					return SR.StatusFilterValueActive;
				case StatusFilterValue.Complete:
					return SR.StatusFilterValueComplete;
				case StatusFilterValue.Stopped:
					return SR.StatusFilterValueStopped;
				case StatusFilterValue.Failed:
					return SR.StatusFilterValueFailed;
				default:
					throw new ArgumentOutOfRangeException("value");
			}
		}

		public object StatusFilter
		{
			get { return _statusFilter.HasValue ? _statusFilter.Value : NoFilter; }
			set
			{
				var v = (value == NoFilter) ? (StatusFilterValue?)null : (StatusFilterValue)value;
				if (_statusFilter != v)
				{
					_statusFilter = v;
					NotifyPropertyChanged("StatusFilter");
					RefreshInternal();
				}
			}
		}

		public IList ActivityTypeFilterChoices
		{
			get { return new[] { NoFilter }.Concat(ActivityTypeHelper.GetActivityTypeList().OrderBy<object, string>(FormatActivityTypeFilter)).ToList(); }
		}

		public string FormatActivityTypeFilter(object value)
		{
			return value == NoFilter ? SR.NoFilterItem : ((string)value);
		}

		public object ActivityTypeFilter
		{
			get { return !string.IsNullOrEmpty(_activityFilter) ? _activityFilter : NoFilter; }
			set
			{
				var v = (value == NoFilter) ? (string)null : (string)value;
				if (_activityFilter != v)
				{
					_activityFilter = v;
					NotifyPropertyChanged("ActivityTypeFilter");
					RefreshInternal();
				}
			}
		}

		public string TextFilter
		{
			get { return _textFilter; }
			set
			{
				if (value != _textFilter)
				{
					_textFilter = value;
					NotifyPropertyChanged("TextFilter");

					// we don't want to do a full refresh on every keystroke, so
					// rather than refreshing immediately, we just re-start the timer
					_textFilterTimer.Stop();
					_textFilterTimer.Start();
				}
			}
		}

		public void StartReindex()
		{
			ReindexTool.StartReindex(Host.DesktopWindow);
		}

		public void OpenFileStore()
		{
			var path = this.FileStore;
			if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
			{
				Process.Start(path);
			}
			else
			{
				this.Host.ShowMessageBox(SR.MessageFilestoreDirectoryDoesNotExist, MessageBoxActions.Ok);
			}
		}

		public void OpenLocalServerConfigurationPage()
		{
			HandleLink(ActivityMonitorQuickLink.LocalServerConfiguration);
		}

		public bool StudyManagementRulesLinkVisible
		{
			get { return _linkHandlers.Any(h => h.CanHandle(ActivityMonitorQuickLink.StudyManagementRules)); }
		}

		public void OpenStudyManagementRules()
		{
			HandleLink(ActivityMonitorQuickLink.StudyManagementRules);
		}

		public void OpenLogFiles()
		{
			var logdir = Platform.LogDirectory;
			if (!string.IsNullOrEmpty(logdir) && Directory.Exists(logdir))
				Process.Start(logdir);
		}

		#endregion

		private IWorkItemActivityMonitor ActivityMonitor { get; set; }

		private static IconSet GetProgressIcon(WorkItemProgress progress, WorkItemStatusEnum status)
		{
			if (progress == null)
				return null;

			return new ProgressBarIconSet("progress", new Size(80, 10), progress.PercentComplete * 100, GetProgressState(status));
		}

		private static ProgressBarState GetProgressState(WorkItemStatusEnum status)
		{
			// determine progress state based on workItem status
			switch (status)
			{
				case WorkItemStatusEnum.InProgress:
				case WorkItemStatusEnum.Complete:
				case WorkItemStatusEnum.Deleted:
				case WorkItemStatusEnum.Canceled:
					return ProgressBarState.Active;

				case WorkItemStatusEnum.Pending:
				case WorkItemStatusEnum.Idle:
                case WorkItemStatusEnum.Canceling:
                    return ProgressBarState.Paused;

				case WorkItemStatusEnum.Failed:
					return ProgressBarState.Error;
			}
			throw new NotImplementedException();
		}

		private void ActivityMonitorIsConnectedChanged(object sender, EventArgs e)
		{
			_connectionState = _connectionState.Update();
			NotifyPropertyChanged("IsConnected");
		}

		private void WorkItemsChanged(object sender, WorkItemsChangedEventArgs e)
		{
			var workItems = e.ChangedItems;
			if (workItems.Any(item => item.Request.ConcurrencyType == WorkItemConcurrency.StudyInsert 
                                    || item.Request.ConcurrencyType == WorkItemConcurrency.StudyDelete
                                    || item.Type.Equals(ReindexRequest.WorkItemTypeString)))
			{
				_studyCountWatcher.Invalidate();
			}

			var items = workItems.Select(item => new WorkItem(item)).ToList();
			_workItemManager.Update(items);
			_workItemActionModel.OnWorkItemsChanged(items);
		}

		private void RefreshInternal()
		{
			_workItemManager.Clear();

			try
			{
				this.ActivityMonitor.Refresh();
			}
			catch (Exception e)
			{
				// don't show a message box here, since the user may not even be looking at this workspace
				Platform.Log(LogLevel.Error, e);
			}
		}

		private bool Include(WorkItem item)
		{
			if (!string.IsNullOrEmpty(_activityFilter) && !item.ActivityType.Equals(_activityFilter))
				return false;

			if (_statusFilter.HasValue && WorkItemStatuses(_statusFilter.Value).All(s => s != item.Status))
				return false;

			if (!string.IsNullOrEmpty(_textFilter) && !item.ContainsText(_textFilter))
				return false;

			return true;
		}

		private void OnTextFilterTimerElapsed(object state)
		{
			_textFilterTimer.Stop();
			RefreshInternal();
		}

		private void OnStudyCountChanged()
		{
			NotifyPropertyChanged("TotalStudies");
		}

        private void OnFailureCountChanged()
        {
            NotifyPropertyChanged("Failures");
        }

		private void OnDicomServerConfigurationChanged()
		{
			NotifyPropertyChanged("AeTitle");
			NotifyPropertyChanged("HostName");
			NotifyPropertyChanged("Port");
		}

		private void OnStorageConfigurationChanged()
		{
			NotifyPropertyChanged("FileStore");
            NotifyPropertyChanged("IsMaximumDiskspaceUsageExceeded");

			// if FileStore path changed, diskspace may have changed too
		    OnDiskspaceChanged();
		}

		private void OnDiskspaceChanged()
		{
            NotifyPropertyChanged("DiskspaceUsed");
			NotifyPropertyChanged("DiskspaceUsedPercent");
			NotifyPropertyChanged("DiskSpaceWarningLabel");
			NotifyPropertyChanged("DiskSpaceWarningMessage");
		}

		private void HandleLink(ActivityMonitorQuickLink link)
		{
			try
			{
				var handler = _linkHandlers.FirstOrDefault(h => h.CanHandle(link));
				if (handler != null)
				{
					handler.Handle(link, this.Host.DesktopWindow);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		private WorkItemStatusEnum[] WorkItemStatuses(StatusFilterValue filterValue)
		{
			switch (filterValue)
			{
				case StatusFilterValue.Active:
					return new[] { WorkItemStatusEnum.Pending, WorkItemStatusEnum.InProgress, WorkItemStatusEnum.Idle };
				case StatusFilterValue.Complete:
					return new[] { WorkItemStatusEnum.Complete };
				case StatusFilterValue.Stopped:
					return new[] { WorkItemStatusEnum.Canceled };
				case StatusFilterValue.Failed:
					return new[] { WorkItemStatusEnum.Failed };
				default:
					throw new ArgumentOutOfRangeException("filterValue");
			}
		}
	}
}
