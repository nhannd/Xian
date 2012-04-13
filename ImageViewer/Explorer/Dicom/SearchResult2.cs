using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement;

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
            
            var request = new WorkItemQueryRequest { Type = WorkItemTypeEnum.ReIndex };
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
            _activityMonitor.WorkItemChanged += OnWorkItemChanged;
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
                _activityMonitor.WorkItemChanged -= OnWorkItemChanged;
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

        private void OnWorkItemChanged(object sender, WorkItemChangedEventArgs args)
        {
            if (args.ItemData == null)
                return;

            switch (args.ItemData.Type)
            {
                case WorkItemTypeEnum.ReIndex:
                    Reindexing = args.ItemData.Status == WorkItemStatusEnum.InProgress;
                    return;
                case WorkItemTypeEnum.DicomSend:
                case WorkItemTypeEnum.ReapplyRules:
                    return;
                default:
                    if (!String.IsNullOrEmpty(args.ItemData.StudyInstanceUid))
                        _setChangedStudies[args.ItemData.StudyInstanceUid] = args.ItemData.StudyInstanceUid;
                    break;
            }

            _processStudiesEventPublisher.Publish(this, EventArgs.Empty);
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

            var queryParams = new QueryParameters();
            queryParams["StudyInstanceUid"] = studyUids;

            try
            {
                //TODO (Marmot): use service node?
                StudyItemList studies = ImageViewerComponent.FindStudy(queryParams, null, "DICOM_LOCAL");
                foreach (StudyItem item in studies)
                {
                    //What's left over in this list has been deleted.
                    changed.Remove(item.StudyInstanceUid);
                    UpdateStudy(item);
                }
            }
            catch (StudyFinderNotFoundException e)
            {
                //should never get here, really.
                Platform.Log(LogLevel.Error, e);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }

            //Anything left over in changed no longer exists.
            foreach (string deletedUid in changed)
                DeleteStudy(deletedUid);
        }

        private void UpdateStudy(StudyItem study)
        {
            //don't need to check this again, it's just paranoia
            if (!StudyExists(study.StudyInstanceUid))
            {
                StudyTable.Items.Add(study);
            }
            else
            {
                int index = GetStudyIndex(study.StudyInstanceUid);
                //just update this since the rest won't change.
                UpdateItem(StudyTable.Items[index], study);
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
        
        private static void UpdateItem(StudyItem existingItem, StudyItem sourceItem)
        {
            //TODO: later, make each item have a 'changed' event for the properties instead of doing this
            existingItem.AccessionNumber = sourceItem.AccessionNumber;
            existingItem.ReferringPhysiciansName = sourceItem.ReferringPhysiciansName;
            existingItem.ModalitiesInStudy = sourceItem.ModalitiesInStudy;
            existingItem.NumberOfStudyRelatedInstances = sourceItem.NumberOfStudyRelatedInstances;
            existingItem.PatientId = sourceItem.PatientId;
            existingItem.PatientsName = sourceItem.PatientsName;
            existingItem.PatientsBirthDate = sourceItem.PatientsBirthDate;
            existingItem.SpecificCharacterSet = sourceItem.SpecificCharacterSet;
            existingItem.StudyDate = sourceItem.StudyDate;
            existingItem.StudyDescription = sourceItem.StudyDescription;

            existingItem.PatientSpeciesDescription = sourceItem.PatientSpeciesDescription;
            existingItem.PatientSpeciesCodeSequenceCodingSchemeDesignator = sourceItem.PatientSpeciesCodeSequenceCodingSchemeDesignator;
            existingItem.PatientSpeciesCodeSequenceCodeValue = sourceItem.PatientSpeciesCodeSequenceCodeValue;
            existingItem.PatientSpeciesCodeSequenceCodeMeaning = sourceItem.PatientSpeciesCodeSequenceCodeMeaning;
            existingItem.PatientBreedDescription = sourceItem.PatientBreedDescription;
            existingItem.PatientBreedCodeSequenceCodingSchemeDesignator = sourceItem.PatientBreedCodeSequenceCodingSchemeDesignator;
            existingItem.PatientBreedCodeSequenceCodeValue = sourceItem.PatientBreedCodeSequenceCodeValue;
            existingItem.PatientBreedCodeSequenceCodeMeaning = sourceItem.PatientBreedCodeSequenceCodeMeaning;
            existingItem.ResponsibleOrganization = sourceItem.ResponsibleOrganization;
            existingItem.ResponsiblePersonRole = sourceItem.ResponsiblePersonRole;
            existingItem.ResponsiblePerson = sourceItem.ResponsiblePerson;
        }
    }
}
