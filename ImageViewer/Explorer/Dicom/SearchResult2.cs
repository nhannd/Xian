using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public partial class SearchResult : IDisposable
    {
        private volatile SynchronizationContext _synchronizationContext;
        private IWorkItemActivityMonitor _activityMonitor;
        private DateTime? _lastWorkItemsChangedTime;

        private readonly object _syncLock = new object();
		private readonly Dictionary<string, string> _setChangedStudies;
        private System.Threading.Timer _processChangedStudiesTimer;

        private bool _reindexing;

        public void Dispose()
        {
            StopMonitoringStudies();
        }

        public bool SearchInProgress { get; private set; }

        private bool Reindexing
        {
            get { return _reindexing; }
            set
            {
                if (Equals(value, _reindexing))
                    return;

                _reindexing = value;
                SetResultsTitle();
            }
        }

        private void UpdateReindexing(bool value)
        {
            if (_activityMonitor == null || !_activityMonitor.IsConnected)
                value = false;
            
            Reindexing = value;
        }

        private void UpdateReindexing()
        {
            if (_activityMonitor == null || !_activityMonitor.IsConnected)
            {
                Reindexing = false;
                return;
            }
            
            var request = new WorkItemQueryRequest { Type = ReindexRequest.WorkItemTypeString };
            IEnumerable<WorkItemData> reindexItems = null;
            
            try
            {
                Platform.GetService<IWorkItemService>(s => reindexItems = s.Query(request).Items);
                Reindexing = reindexItems != null && reindexItems.Any(item => item.Status == WorkItemStatusEnum.InProgress);
            }
            catch (Exception e)
            {
                Reindexing = false;
                Platform.Log(LogLevel.Debug, e);
            }
        }

        private void StartMonitoringStudies()
        {
            if (_activityMonitor != null)
                return;

            _synchronizationContext = SynchronizationContext.Current;
            //Don't use the sync context when monitoring work item activity, since we'll be processing
            //the changed studies asynchronously anyway.
            _activityMonitor = WorkItemActivityMonitor.Create(false);
            _activityMonitor.IsConnectedChanged += OnIsConnectedChangedAsync;
            _activityMonitor.WorkItemsChanged += OnWorkItemsChangedAsync;
            _activityMonitor.StudiesCleared += OnStudiesClearedAsync;

            UpdateReindexing();

            _processChangedStudiesTimer = new System.Threading.Timer(ProcessChangedStudiesAsync, null, 100, 100);
        }

        private void StopMonitoringStudies()
        {
            _synchronizationContext = null;
            if (_activityMonitor != null)
            {
                _activityMonitor.IsConnectedChanged -= OnIsConnectedChangedAsync;
                _activityMonitor.WorkItemsChanged -= OnWorkItemsChangedAsync;
                _activityMonitor.StudiesCleared -= OnStudiesClearedAsync;
                _activityMonitor.Dispose();
                _activityMonitor = null;
            }

            if (_processChangedStudiesTimer != null)
            {
                _processChangedStudiesTimer.Dispose();
                _processChangedStudiesTimer = null;
            }
            
            UpdateReindexing();
        }

        private void OnStudiesCleared()
        {
            lock (_syncLock)
            {
                _setChangedStudies.Clear();
            }

            _hiddenItems.Clear();
            StudyTable.Items.Clear();
            SetResultsTitle();
        }

        /// <summary>
        /// Marshaled back to the UI thread from a worker thread.
        /// </summary>
        private void UpdatedChangedStudies(DateTime queryStartTime, IEnumerable<string> deletedStudyUids, IEnumerable<StudyEntry> updatedStudies)
        {
            //If the sync context is null, it's because we're not monitoring studies anymore (e.g. study browser closed).
            if (_synchronizationContext == null)
                return;

            bool hasUserSearched = SearchInProgress || _lastSearchEnded.HasValue;
            bool updateQueryStartedBeforeUserSearchEnded = _lastSearchEnded.HasValue && queryStartTime < _lastSearchEnded.Value;

            //If the user has ever searched, and the query for study updates started before the user's search completed,
            //then we just ignore these updates. Otherwise, the user might get out-of-date updates, and may even see
            //studies appearing in the table while the search is still in progress.
            if (hasUserSearched && updateQueryStartedBeforeUserSearchEnded)
                return;

            foreach (var updatedStudy in updatedStudies)
                UpdateTableItem(updatedStudy);

            foreach (string deletedUid in deletedStudyUids)
                DeleteStudy(deletedUid);

            SetResultsTitle();
        }

        private void UpdateTableItem(StudyEntry entry)
        {
            //don't need to check this again, it's just paranoia
            if (!StudyExists(entry.Study.StudyInstanceUid))
            {
                StudyTable.Items.Add(new StudyTableItem(entry));
            }
            else
            {
                int index = GetStudyIndex(entry.Study.StudyInstanceUid);
                //just update this since the rest won't change.
                StudyTable.Items[index].Entry = entry;
                StudyTable.Items.NotifyItemUpdated(index);
            }
        }

        private void DeleteStudy(string studyInstanceUid)
        {
            int foundIndex = StudyTable.Items.FindIndex(test => test.StudyInstanceUid == studyInstanceUid);
            if (foundIndex >= 0)
                StudyTable.Items.RemoveAt(foundIndex);

            foundIndex = _hiddenItems.FindIndex(test => test.StudyInstanceUid == studyInstanceUid);
            if (foundIndex >= 0)
                _hiddenItems.RemoveAt(foundIndex);
        }

        private bool StudyExists(string studyInstanceUid)
        {
            return GetStudyIndex(studyInstanceUid) >= 0;
        }

        private int GetStudyIndex(string studyInstanceUid)
        {
            return StudyTable.Items.FindIndex(test => test.StudyInstanceUid == studyInstanceUid);
        }

        #region Async Operations

        private void OnIsConnectedChangedAsync(object sender, EventArgs e)
        {
            var syncContext = _synchronizationContext;
            if (syncContext != null)
                syncContext.Post(ignore => UpdateReindexing(), null);
        }

        /// <summary>
        /// Studies cleared (from activity monitor) event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStudiesClearedAsync(object sender, EventArgs e)
        {
            var syncContext = _synchronizationContext;
            if (syncContext != null)
                syncContext.Post(ignore => OnStudiesCleared(), null);
        }

        /// <summary>
        /// Work Items Changed event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnWorkItemsChangedAsync(object sender, WorkItemsChangedEventArgs args)
        {
            if (args.ChangedItems == null)
                return;

            var syncContext = _synchronizationContext;
            if (syncContext == null)
                return;

            lock (_syncLock)
            {
                foreach (var item in args.ChangedItems)
                {
                    var theItem = item;
                    if (item.Type.Equals(ReindexRequest.WorkItemTypeString))
                        syncContext.Post(ignore => UpdateReindexing(theItem.Status == WorkItemStatusEnum.InProgress), false);
                    
                    if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyRead)
                        continue; //If it's a "read" operation, don't update anything.

                    //Otherwise, if it's not a read operation, but it has a Study UID, then it's probably updating studies.
                    //(e.g. re-index, re-apply rules, import, process study).
                    if (!String.IsNullOrEmpty(item.StudyInstanceUid))
                        _setChangedStudies[item.StudyInstanceUid] = item.StudyInstanceUid;
                }

                _lastWorkItemsChangedTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Process studies thread.
        /// </summary>
        /// <param name="state"></param>
        private void ProcessChangedStudiesAsync(object state)
        {
            //If the sync context is null, it's because we're not monitoring studies anymore (e.g. study browser closed).
            var syncContext = _synchronizationContext;
            if (syncContext == null)
                return;

            DateTime? queryStartTime;
            List<string> deletedStudyUids;
            List<StudyEntry> updatedStudies;
                
            ProcessChangedStudiesAsync(out queryStartTime, out deletedStudyUids, out updatedStudies);
            if (deletedStudyUids.Count > 0 || updatedStudies.Count > 0)
                syncContext.Post(ignore => UpdatedChangedStudies(queryStartTime.Value, deletedStudyUids, updatedStudies), null);
        }

        /// <summary>
        /// Figure out which studies have been deleted and/or updated.
        /// </summary>
        private void ProcessChangedStudiesAsync(out DateTime? queryStartTime, out List<string> deletedStudyUids, out List<StudyEntry> updatedStudies)
        {
            deletedStudyUids = new List<string>();
            updatedStudies = new List<StudyEntry>();
            queryStartTime = null;
            DateTime now = DateTime.Now;

            lock (_syncLock)
            {
                if (_setChangedStudies.Count == 0 || _lastWorkItemsChangedTime == null)
                    return;

                //If work items are still coming in, delay the query.
                if (now - _lastWorkItemsChangedTime.Value < TimeSpan.FromMilliseconds(350))
                    return;

                //Add everything to the deleted list.
                deletedStudyUids.AddRange(_setChangedStudies.Keys);
                _setChangedStudies.Clear();
            }

            string studyUids = DicomStringHelper.GetDicomStringArray(deletedStudyUids);
            if (String.IsNullOrEmpty(studyUids))
                return;

            try
            {
                queryStartTime = now;
             
                var criteria = new StudyRootStudyIdentifier { StudyInstanceUid = studyUids };
                var request = new GetStudyEntriesRequest { Criteria = new StudyEntry { Study = criteria } };
                
                IList<StudyEntry> entries = null;
                
                //We're doing it this way here because it's local only.
                Platform.GetService<IStudyStoreQuery>(s => entries = s.GetStudyEntries(request).StudyEntries);

                foreach (var entry in entries)
                {
                    //If we got a result back, then it's not deleted.
                    deletedStudyUids.Remove(entry.Study.StudyInstanceUid);
                    updatedStudies.Add(entry);
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }
        }

        #endregion
    }
}
