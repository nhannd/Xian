using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public partial class SearchResult : IDisposable
    {
        private IWorkItemActivityMonitor _activityMonitor;
		private readonly Dictionary<string, string> _setChangedStudies;
        private DelayedEventPublisher _processStudiesEventPublisher;
        private bool _reindexing;

        public void Dispose()
        {
            StopMonitoringStudies();
        }

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
            _activityMonitor = WorkItemActivityMonitor.Create();
            _activityMonitor.IsConnectedChanged += OnIsConnectedChanged;
            _activityMonitor.WorkItemsChanged += OnWorkItemsChanged;
            _activityMonitor.StudiesCleared += OnStudiesCleared;

            UpdateReindexing();
            _processStudiesEventPublisher = new DelayedEventPublisher((s,e) => ProcessChangedStudies());
        }

        private void StopMonitoringStudies()
        {
            if (_processStudiesEventPublisher != null)
            {
                _processStudiesEventPublisher.Dispose();
                _processStudiesEventPublisher = null;
            }

            if (_activityMonitor != null)
            {
                _activityMonitor.IsConnectedChanged -= OnIsConnectedChanged;
                _activityMonitor.WorkItemsChanged -= OnWorkItemsChanged;
                _activityMonitor.StudiesCleared -= OnStudiesCleared;
                _activityMonitor.Dispose();
                _activityMonitor = null;
            }

            UpdateReindexing();
        }

        private void OnIsConnectedChanged(object sender, EventArgs e)
        {
            UpdateReindexing();
        }

        private void OnWorkItemsChanged(object sender, WorkItemsChangedEventArgs args)
        {
            if (args.ChangedItems == null)
                return;

        	foreach (var item in args.ChangedItems)
        	{
				//todo
                //if (item.Type.Equals(DicomSendRequest.WorkItemTypeString) || item.Type.Equals(ReapplyRulesRequest.WorkItemTypeString))
                //    return;

                if (item.Type.Equals(ReindexRequest.WorkItemTypeString))
                {
                    Reindexing = item.Status == WorkItemStatusEnum.InProgress;
                    break;
                }

        	    if (!String.IsNullOrEmpty(item.StudyInstanceUid))
					_setChangedStudies[item.StudyInstanceUid] = item.StudyInstanceUid;

				_processStudiesEventPublisher.Publish(this, EventArgs.Empty);
			}

        }

        private void OnStudiesCleared(object sender, EventArgs e)
        {
            _setChangedStudies.Clear();
            _hiddenItems.Clear();
            StudyTable.Items.Clear();
        }

        private void ProcessChangedStudies()
        {
            if (_setChangedStudies.Count == 0)
                return;

            var changed = _setChangedStudies.Keys.ToList();
            _setChangedStudies.Clear();

            string studyUids = DicomStringHelper.GetDicomStringArray(changed);
            if (String.IsNullOrEmpty(studyUids))
                return;

            try
            {
                var criteria = new StudyRootStudyIdentifier { StudyInstanceUid = studyUids };
                var request = new GetStudyEntriesRequest { Criteria = new StudyEntry { Study = criteria } };
                
                IList<StudyEntry> entries = null;
                
                //We're doing it this way here because it's local only.
                Platform.GetService<IStudyStoreQuery>(s => entries = s.GetStudyEntries(request).StudyEntries);

                foreach (var entry in entries)
                {
                    //What's left over in this list has been deleted.
                    changed.Remove(entry.Study.StudyInstanceUid);
                    UpdateTableItem(entry);
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }

            //Anything left over in changed no longer exists.
            foreach (string deletedUid in changed)
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
    }
}
