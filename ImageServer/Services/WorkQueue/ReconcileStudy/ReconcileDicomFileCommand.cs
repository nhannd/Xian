using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Services.Dicom;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    /// <summary>
    /// Command for reconciling a dicom file.
    /// </summary>
    /// <remark>
    /// A <see cref="ReconcileDicomFileCommand"/> instance can be reused for different dicom files.
    /// <see cref="ReconcileDicomFileCommand.DicomFile"/> and <see cref="Context"/> must be set prior 
    /// to the command execution.
    /// </remark>
    class ReconcileDicomFileCommand : ServerDatabaseCommand
    {
        #region Private Members
        private DicomFile _file;
        private ReconcileStudyProcessorContext _context;
        private ServerFilesystemInfo _destFilesystemInfo;
        private StudyStorageLocation _destStudyStorage;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="ReconcileDicomFileCommand"/>
        /// </summary>
        public ReconcileDicomFileCommand()
			: base("Reconcile Dicom File Command", true)
		{
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Sets or gets the <see cref="DicomFile"/> that will be reconciled.
        /// </summary>
        public DicomFile DicomFile
        {
            get { return _file; }
            set { _file = value; }
        }

        /// <summary>
        /// Sets or gets the <see cref="ReconcileStudyProcessorContext"/>
        /// </summary>
        public ReconcileStudyProcessorContext Context
        {
            get { return _context; }
            set { _context = value; }
        }
        #endregion

        #region Private Methods
        private void Initialize()
        {
            Platform.CheckForNullReference(_context, "_context");
            Platform.CheckForNullReference(_context.ExistingStudyStorageLocation, "_context.ExistingStudyStorageLocation");

            // this should be the updated study instance
            string studyInstanceUid = DicomFile.DataSet[DicomTags.StudyInstanceUid].GetString(0, String.Empty);

            if (_context.ExistingStudyStorageLocation.StudyInstanceUid == studyInstanceUid)
            {
                // study instance uid doesn't change. Use the same storage location as the existing study
                _destStudyStorage = Context.ExistingStudyStorageLocation;
                _destFilesystemInfo = FilesystemMonitor.Instance.GetFilesystemInfo(_destStudyStorage.FilesystemKey);
            }
            else
            {
                // new study instance uid is assigned. WE have to pick a new location
                FilesystemSelector selector = new FilesystemSelector(FilesystemMonitor.Instance);
                Filesystem fs = selector.SelectFilesystem(DicomFile);
                _destFilesystemInfo = FilesystemMonitor.Instance.GetFilesystemInfo(fs.GetKey());
                _destStudyStorage = CreateStudyLocation();
            }

            Debug.Assert(_destFilesystemInfo != null);
            Debug.Assert(_destStudyStorage!= null);

            
            if (!_destFilesystemInfo.Writeable)
            {
                throw new ApplicationException(
                    String.Format("Cannot reconcile image. Filesystem '{0}' is not writable.",
                                  _destFilesystemInfo.Filesystem.Description));
            }

            _context.DestFilesystem = _destFilesystemInfo;
            _context.DestStorageLocation = _destStudyStorage;
        }

        private StudyStorageLocation CreateStudyLocation()
        {
            string studyInstanceUid = DicomFile.DataSet[DicomTags.StudyInstanceUid].GetString(0, String.Empty);
            string studyDate = DicomFile.DataSet[DicomTags.StudyDate].GetString(0, String.Empty);

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using(IUpdateContext updateContext = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IInsertStudyStorage locInsert = updateContext.GetBroker<IInsertStudyStorage>();
                StudyStorageInsertParameters insertParms = new StudyStorageInsertParameters();
                insertParms.ServerPartitionKey = Context.Partition.GetKey();
                insertParms.StudyInstanceUid = studyInstanceUid;
                insertParms.Folder = studyDate;
                insertParms.FilesystemKey = _destFilesystemInfo.Filesystem.GetKey();

                if (DicomFile.TransferSyntax.LosslessCompressed)
                {
                    insertParms.TransferSyntaxUid = DicomFile.TransferSyntax.UidString;
                    insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossless;
                }
                else if (DicomFile.TransferSyntax.LossyCompressed)
                {
                    insertParms.TransferSyntaxUid = DicomFile.TransferSyntax.UidString;
                    insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossy;
                }
                else
                {
                    insertParms.TransferSyntaxUid = TransferSyntax.ExplicitVrLittleEndianUid;
                    insertParms.StudyStatusEnum = StudyStatusEnum.Online;
                }

                StudyStorageLocation location = locInsert.FindOne(insertParms);

                if (location!=null)
                {
                    updateContext.Commit();
                    return location;
                }
                else
                {
                    throw new ApplicationException("Unable to create new storage location");
                }
            }

        }

        #endregion

        #region Overridden Protected Methods
        protected override void OnExecute(IUpdateContext updateContext)
        {
            Platform.CheckForNullReference(_context, "_context");
            Platform.CheckForNullReference(_file, "_file");

            Initialize();

            String seriesInstanceUid = DicomFile.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            String sopInstanceUid = DicomFile.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);

            String path = _destStudyStorage.FilesystemPath;
            String extension = ".dcm";
            bool dupImage = false;

            ServerCommandProcessor processor = new ServerCommandProcessor("Reconciling image processor");
            processor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, _destStudyStorage.PartitionFolder);
            processor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, _destStudyStorage.StudyFolder);
            processor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, _destStudyStorage.StudyInstanceUid);
            processor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, seriesInstanceUid);
            processor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, sopInstanceUid);
            path += extension;

            if (File.Exists(path))
            {
                #region Duplicate SOP
                
                // TODO: Add code to handle duplicate sop here
                Platform.Log(LogLevel.Warn, "Image {0} is discarded because of duplicate", DicomFile.Filename);
                return;

                #endregion
            }
            
            SaveDicomFileCommand saveCommand = new SaveDicomFileCommand(path, DicomFile);
            processor.AddCommand(saveCommand);
            processor.AddCommand(new UpdateWorkQueueCommand(DicomFile, _destStudyStorage, dupImage, extension));

            if (!processor.Execute())
            {
                throw new ApplicationException(String.Format("Unable to reconcile image {0} : {1}", DicomFile.Filename, processor.FailureReason));
            }
        }

        #endregion

    }
}
