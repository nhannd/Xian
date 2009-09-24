using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Core.Reconcile.CreateStudy;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Class implementing <see cref="IStudyPreProcessor"/> to update a DICOM image
    /// based on past reconciliation.
    /// </summary>
    class AutoReconciler : BasePreprocessor, IStudyPreProcessor
    {
        private readonly static ServerCache<ServerEntityKey, StudyStorageLocation> _duplicatePolicyOverriddenAlerts = new ServerCache<ServerEntityKey, StudyStorageLocation>(TimeSpan.FromMinutes(2), TimeSpan.FromSeconds(30));

        private readonly string _contextID;
        
        class AutoReconcilerResult : PreProcessingResult
        {
            #region Private Members
            private readonly StudyReconcileAction _action;
            private IList<UpdateItem> _changes = new List<UpdateItem>();
            #endregion

            /// <summary>
            /// 
            /// </summary>
            /// <param name="action"></param>
            public AutoReconcilerResult(StudyReconcileAction action)
            {
                AutoReconciled = true; 
                _action = action;
                
            }
                        
            /// <summary>
            /// Gets or sets the list of changes made when <see cref="AutoReconciler"/> was executed on a DICOM file.
            /// </summary>
            public IList<UpdateItem> Changes
            {
                get { return _changes; }
                set { _changes = value; }
            }

            /// <summary>
            /// Gets other action that was used when <see cref="AutoReconciler"/> was executed on a DICOM file.
            /// </summary>
            public StudyReconcileAction Action
            {
                get { return _action; }

            }
 
        }
        
        #region Private Members

        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="AutoReconciler"/> to update
        /// a DICOM file according to the history.
        /// </summary>
        /// <param name="contextID">Context ID of the operation. Used to identify files belonging to the same group (usually from the same association).</param>
        /// <param name="storageLocation"></param>
        public AutoReconciler(String contextID, StudyStorageLocation storageLocation) 
            : base("AUTO-RECONCILE", storageLocation)
        {
            Platform.CheckForEmptyString(contextID, "contextID");
            _contextID = contextID;
        } 
        #endregion

        #region IStudyPreProcessor Members

        public PreProcessingResult Process(DicomFile file)
        {
            Platform.CheckForNullReference(file, "file");

            AutoReconcilerResult preProcessingResult = null;
            string originalSeriesUid = file.DataSet[DicomTags.SeriesInstanceUid].ToString();
            string originalSopUid = file.DataSet[DicomTags.SopInstanceUid].ToString();

            // Update the file based on the reconciliation in the past
            StudyStorageLocation dest;
            IList<StudyHistory> histories = FindReconcileHistories(StorageLocation, file);
            if (histories != null && histories.Count > 0)
            {
                #region Update the files based on history...

                StudyHistory lastHistory = histories[0];

                bool restored;
                StudyReconcileDescriptorParser parser = new StudyReconcileDescriptorParser();
                StudyReconcileDescriptor changeLog = parser.Parse(lastHistory.ChangeDescription);
                switch (changeLog.Action)
                {
                    case StudyReconcileAction.CreateNewStudy:
                    case StudyReconcileAction.Merge:
                        #region Update Images to match destination study

                        UidMapper uidMapper = null;
                        bool seriesMapUpdated = false;

                        bool belongsToAnotherStudy;
                        if (lastHistory.DestStudyStorageKey != null)
                        {
                            preProcessingResult = new AutoReconcilerResult(changeLog.Action);
                            StudyStorage destinationStudy = StudyStorage.Load(lastHistory.DestStudyStorageKey);
                            dest = ServerHelper.GetStudyOnlineStorageLocation(destinationStudy, out restored);

                            belongsToAnotherStudy = !dest.Equals(StorageLocation);
                            ImageUpdateCommandBuilder commandBuilder = new ImageUpdateCommandBuilder();
                            IList<BaseImageLevelUpdateCommand> commands = commandBuilder.BuildCommands<StudyMatchingMap>(dest);

                            if (changeLog.SeriesMappings!=null && changeLog.SeriesMappings.Count>0)
                            {
                                uidMapper = new UidMapper(changeLog.SeriesMappings);
                                uidMapper.SeriesMapUpdated += delegate { seriesMapUpdated = true; };
                                if (uidMapper.ContainsSeries(originalSeriesUid))
                                {
                                    string newSeriesUid = uidMapper.GetNewSeriesUid(originalSeriesUid);
                                    
                                    // Check if there's an image that was originated from the same SOP Instance UID. 
                                    // If so, throw this image away
                                    StudyXml studyXml = dest.LoadStudyXml();
                                    InstanceXml instanceXml = studyXml[newSeriesUid].FindSourceImageInstanceXml(originalSopUid);
                                    if (instanceXml!=null)
                                    {
                                        Platform.Log(LogLevel.Info, "Found SOP {0} with the same SOP Instance UID {1} in Source Image Sequence. Discard this image (Overriding duplicate policy).",
                                                instanceXml.SopInstanceUid, originalSopUid);
                                        // let the caller to delete the file
                                        preProcessingResult = new AutoReconcilerResult(StudyReconcileAction.Discard);
                                        preProcessingResult.DiscardImage = true;

                                        if (!_duplicatePolicyOverriddenAlerts.ContainsKey(dest.Key))
                                        {
                                            ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Warning,
                                                                 "WorkQueue", 0, null, TimeSpan.Zero, 
                                                                 "Duplicate Policy is overridden for study {0}. Image(s) has been assigned new SOP Instance UID and therefore discarded",
                                                                 dest.StudyInstanceUid);
 
                                        }
                                        break;
                                    }
                                }

                                else
                                {
                                    commands.Add(new SeriesSopUpdateCommand(uidMapper));
                                }
                            }

                            List<UpdateItem> updateList = new List<UpdateItem>();
                            foreach(BaseImageLevelUpdateCommand cmd in commands)
                            {
                                if (cmd.UpdateEntry != null && cmd.UpdateEntry.TagPath != null && cmd.UpdateEntry.TagPath.Tag != null)
                                {
                                    UpdateItem item = new UpdateItem(cmd, file);
                                    updateList.Add(item);
                                }
                                
                            }

                            preProcessingResult.Changes = updateList;

                            using (ServerCommandProcessor processor = new ServerCommandProcessor("Update Image According to History"))
                            {
                                foreach (BaseImageLevelUpdateCommand cmd in commands)
                                {
                                    cmd.File = file;
                                    processor.AddCommand(cmd);
                                }

                                if (!processor.Execute())
                                {
                                    if (processor.FailureException != null)
                                    {
                                        throw processor.FailureException;
                                    }
                                    else
                                        throw new ApplicationException(String.Format("AUTO-RECONCILE Failed: Unable to update image to match target study. Reason: {0}", processor.FailureReason), processor.FailureException);
                                }
                            }

                            // First, must update the map
                            if (uidMapper != null && seriesMapUpdated)
                            {
                                using (ServerCommandProcessor processor =new ServerCommandProcessor("Update Series Mapping Processor"))
                                {
                                    processor.AddCommand(new UpdateHistorySeriesMappingCommand(lastHistory, uidMapper));
                                    if (!processor.Execute())
                                    {
                                        throw new ApplicationException(String.Format("AUTO-RECONCILE Failed: Unable to update series mapping. Reason: {0}", processor.FailureReason), processor.FailureException);
                                    }
                                }
                            }



                            if (belongsToAnotherStudy)
                            {
                                SopInstanceImporterContext importContext = new SopInstanceImporterContext(_contextID, file.SourceApplicationEntityTitle, dest.ServerPartition);
                                SopInstanceImporter importer = new SopInstanceImporter(importContext);
                                DicomProcessingResult result = importer.Import(file);

                                if (!result.Successful)
                                {
                                    throw new ApplicationException("Unable to import image to destination study");
                                }
                            }

                        }


                        #endregion

                        break;

                    case StudyReconcileAction.Discard:
                        preProcessingResult = new AutoReconcilerResult(StudyReconcileAction.Discard);
                        preProcessingResult.DiscardImage = true;
                        break;

                    case StudyReconcileAction.ProcessAsIs:
                        if (lastHistory.DestStudyStorageKey != null)
                        {
                            StudyStorage destinationStudy = StudyStorage.Load(lastHistory.DestStudyStorageKey);
                            dest = ServerHelper.GetStudyOnlineStorageLocation(destinationStudy, out restored);
                            preProcessingResult = new AutoReconcilerResult(StudyReconcileAction.ProcessAsIs);

                            belongsToAnotherStudy = !dest.Equals(StorageLocation);

                            if (belongsToAnotherStudy)
                            {
                                preProcessingResult.Changes = new List<UpdateItem>();
                                preProcessingResult.Changes.Add(new UpdateItem(
                                                                    DicomTags.StudyInstanceUid,
                                                                    file.DataSet[DicomTags.StudyInstanceUid].ToString(),
                                                                    dest.StudyInstanceUid));

                                file.DataSet[DicomTags.StudyInstanceUid].SetStringValue(dest.StudyInstanceUid);
                                SopInstanceImporterContext importContext = new SopInstanceImporterContext(
                                    _contextID,
                                    file.SourceApplicationEntityTitle, dest.ServerPartition);
                                SopInstanceImporter importer = new SopInstanceImporter(importContext);
                                DicomProcessingResult result = importer.Import(file);

                                if (!result.Successful)
                                {
                                    throw new ApplicationException("Unable to import image to destination study");
                                }
                            }
                           
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }
                #endregion
            }

            if (preProcessingResult!=null)
            {
                StringBuilder log = new StringBuilder();
                log.AppendLine(String.Format("AUTO-RECONCILE: {0}. SOP {1}", preProcessingResult.Action, file.MediaStorageSopInstanceUid));
                foreach (UpdateItem change in preProcessingResult.Changes)
                {
                    if (change.NewValue != null && !change.NewValue.Equals(change.OriginalValue))
                    {
                        log.AppendLine(String.Format("\tSet {0}: {1} => {2}", change.Tag, change.OriginalValue, change.NewValue));    
                    }
                }
                Platform.Log(LogLevel.Info, log.ToString());
               
            }
            return preProcessingResult;
        }

        #endregion

        private static IList<StudyHistory> FindReconcileHistories(StudyStorageLocation storageLocation, DicomFile file)
        {
            ImageSetDescriptor fileDesc = new ImageSetDescriptor(file.DataSet);

            List<StudyHistory> studyHistoryList = new List<StudyHistory>(
                ServerHelper.FindStudyHistories(storageLocation.StudyStorage,
                                                new StudyHistoryTypeEnum[] { StudyHistoryTypeEnum.StudyReconciled }));

            IList<StudyHistory> reconcileHistories = studyHistoryList.FindAll(
                delegate(StudyHistory item)
                    {
                        ImageSetDescriptor desc = XmlUtils.Deserialize<ImageSetDescriptor>(item.StudyData.DocumentElement);
                        return desc.Equals(fileDesc);
                    });

            if (reconcileHistories == null || reconcileHistories.Count == 0)
            {
                // no history found in cache... reload the list and search again one more time
                studyHistoryList = new List<StudyHistory>(
                    ServerHelper.FindStudyHistories(storageLocation.StudyStorage,
                                                    new StudyHistoryTypeEnum[] { StudyHistoryTypeEnum.StudyReconciled }));

                reconcileHistories = studyHistoryList.FindAll(
                    delegate(StudyHistory item)
                        {
                            ImageSetDescriptor desc = XmlUtils.Deserialize<ImageSetDescriptor>(item.StudyData.DocumentElement);
                            return desc.Equals(fileDesc);
                        });

            }

            return reconcileHistories;
        }

        
    }
}