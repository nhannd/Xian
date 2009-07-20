using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Defines the interface of the Pre-Processors that is called
    /// by the <see cref="StudyProcessItemProcessor"/> 
    /// </summary>
    internal interface IStudyPreProcessor
    {
        /// <summary>
        /// Called to process a DICOM file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>An instance of <see cref="PreProcessingResult"/> containing the result of the processing. NULL if 
        /// the change has been made to the file.</returns>
        PreProcessingResult Process(DicomFile file);

        /// <summary>
        /// Gets or sets the <see cref="StudyStorageLocation"/> of the study which the 
        /// DICOM file(s) belong to.
        /// </summary>
        StudyStorageLocation StorageLocation { get; set;}

        /// <summary>
        /// Gets or sets the description of the pre-processor.
        /// </summary>
        string Description
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the result of the DICOM file preprocessing.
    /// </summary>
    public class PreProcessingResult
    {
        #region Private Members
        private bool _autoReconciled;
        private bool _discardImage;
        #endregion

        /// <summary>
        /// Indicates whether the file has been updated 
        /// as part of auto-reconciliation process.
        /// </summary>
        public bool AutoReconciled
        {
            get { return _autoReconciled; }
            set { _autoReconciled = value; }
        }

        /// <summary>
        /// Indicates whether or not the file should be discarded.
        /// </summary>
        public bool DiscardImage
        {
            get { return _discardImage; }
            set { _discardImage = value; }
        }
    }
    
    /// <summary>
    /// Represents changes applied to a DICOM
    /// </summary>
    public class UpdateItem
    {
        #region Private Members
        private readonly DicomTag _tag;
        private readonly string _originalValue;
        private readonly string _newValue;
        
        #endregion
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="UpdateItem"/>
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="originalValue"></param>
        /// <param name="newValue"></param>
        public UpdateItem(uint tag, string originalValue, string newValue)
        {
            _tag = DicomTagDictionary.GetDicomTag(tag);
            _originalValue = originalValue;
            _newValue = newValue;
        }

        /// <summary>
        /// Creates an instance of <see cref="UpdateItem"/>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="file"></param>
        public UpdateItem(IUpdateImageTagCommand command, DicomFile file)
        {
            this._tag = command.UpdateEntry.TagPath.Tag;
            this._originalValue = file.DataSet[Tag].ToString();
            this._newValue = command.UpdateEntry.Value != null ? command.UpdateEntry.Value.ToString() : String.Empty;
        }
        
        #endregion
        #region Public Properties

        /// <summary>
        /// Gets the DICOM tag being updated.
        /// </summary>
        public DicomTag Tag
        {
            get { return _tag; }
        }

        /// <summary>
        /// Gets the original value of the DICOM tag being updated.
        /// </summary>
        public string OriginalValue
        {
            get { return _originalValue; }
        }

        /// <summary>
        /// Gets the new value of the DICOM tag being updated.
        /// </summary>
        public string NewValue
        {
            get { return _newValue; }
        } 
        #endregion
    }


    
    /// <summary>
    /// Class implementing <see cref="IStudyPreProcessor"/> to update a DICOM image
    /// based on past reconciliation.
    /// </summary>
    class AutoReconciler : BasePreprocessor, IStudyPreProcessor
    {
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

            // Update the file based on the reconciliation in the past
            StudyStorageLocation dest = StorageLocation;
            bool belongsToAnotherStudy = false;
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
                        if (lastHistory.DestStudyStorageKey != null)
                        {
                            preProcessingResult = new AutoReconcilerResult(changeLog.Action);
                            StudyStorage destinationStudy = StudyStorage.Load(lastHistory.DestStudyStorageKey);
                            dest = ServerHelper.GetStudyOnlineStorageLocation(destinationStudy, out restored);

                            belongsToAnotherStudy = !dest.Equals(StorageLocation);
                            ImageUpdateCommandBuilder commandBuilder = new ImageUpdateCommandBuilder();
                            IList<BaseImageLevelUpdateCommand> commands = commandBuilder.BuildCommands<StudyMatchingMap>(destinationStudy);


                            preProcessingResult.Changes = CollectionUtils.Map<IUpdateImageTagCommand, UpdateItem>(
                                            CollectionUtils.Select(commands, delegate(BaseImageLevelUpdateCommand item) { return item is IUpdateImageTagCommand; }),
                                            delegate(IUpdateImageTagCommand input)
                                            {
                                                UpdateItem item = new UpdateItem(input, file);
                                                return item;
                                            });

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
                                        throw new ApplicationException(String.Format("AUTO-RECONCILE Failed: Unable to update image to match target study. Reason: {0}", processor.FailureReason));
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
                            preProcessingResult = new AutoReconcilerResult(StudyReconcileAction.ProcessAsIs);
                        
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

    /// <summary>
    /// Class implementing <see cref="IStudyPreProcessor"/> to correct the patient's name
    /// in the DICOM file if it doesn't match what is in the system.
    /// </summary>
    class PatientNameAutoCorrection: BasePreprocessor, IStudyPreProcessor
    {
        class PatientNameAutoCorrectionResult : PreProcessingResult
        {
            private string _oldPatientsName;
            private string _newPatientsName;

            public string NewPatientsName
            {
                get { return _newPatientsName; }
                set { _newPatientsName = value; }
            }

            public string OldPatientsName
            {
                get { return _oldPatientsName; }
                set { _oldPatientsName = value; }
            }

        }
        
        #region Private Members
        private string _contextID; 
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="PatientNameAutoCorrection"/> to fix
        /// the patient's name in a DICOM file to match a study.
        /// </summary>
        /// <param name="contextID"></param>
        /// <param name="storageLocation"></param>
        public PatientNameAutoCorrection(String contextID, StudyStorageLocation storageLocation)
            : base("AUTO-CORRECTION : PATIENT'S NAME", storageLocation)
        {
            _contextID = contextID;
        }
        #endregion

        #region IStudyPreProcessor Members

        public PreProcessingResult Process(DicomFile file)
        {

            PatientNameAutoCorrectionResult preProcessingResult = null;

            if (StorageLocation.Study!=null)
            {
                StudyComparer comparer = new StudyComparer();
                DifferenceCollection list = comparer.Compare(file, StorageLocation.Study, StorageLocation.ServerPartition.GetComparisonOptions());

                if (list.Count == 1)
                {
                    ComparisionDifference different = list[0];
                    if (different.DicomTag.TagValue == DicomTags.PatientsName)
                    {
                        #region Fix the patient's name if possible
                        string patientsNameInFile = file.DataSet[DicomTags.PatientsName].ToString();
                        if (DicomNameUtils.LookLikeSameNames(patientsNameInFile, StorageLocation.Study.PatientsName))
                        {
                            using (ServerCommandProcessor processor = new ServerCommandProcessor("Update Image According to History"))
                            {
                                SetTagCommand command = new SetTagCommand(file, DicomTags.PatientsName, StorageLocation.Study.PatientsName);
                                processor.AddCommand(command);
                                
                                if (!processor.Execute())
                                {
                                    if (processor.FailureException != null)
                                    {
                                        throw processor.FailureException;
                                    }
                                    else
                                        throw new ApplicationException(String.Format("AUTO-RECONCILE Failed: Unable to correct the patient's name in the image. Reason: {0}", processor.FailureReason));
                                }

                                preProcessingResult = new PatientNameAutoCorrectionResult();
                                preProcessingResult.OldPatientsName = patientsNameInFile;
                                preProcessingResult.NewPatientsName = StorageLocation.Study.PatientsName;

                                
                            }
                        }
                        else
                        {
                            // patients name does not look like what's in the db.. will be copied to reconcile queue later.
                        }
                        #endregion
                    }
                }
            }

            if (preProcessingResult != null)
            {
                StringBuilder log = new StringBuilder();
                log.AppendLine(
                    String.Format("AUTO-CORRECTION: Patient's Name: {0} ==> {1}. SOP {2}", 
                                    preProcessingResult.OldPatientsName,
                                    preProcessingResult.NewPatientsName,
                                    file.MediaStorageSopInstanceUid));
                Platform.Log(LogLevel.Info, log.ToString());

            }
            return preProcessingResult;
        }


       

        #endregion
    }

}
