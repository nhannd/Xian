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

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Commands
{
    /// <summary>
    /// Command for reconciling an image by moving it into the correct location 
    /// and create necessary entries in the database for the location.
    /// </summary>
    /// <remark>
    /// A <see cref="ReconcileRelocateFileCommand"/> instance can be reused for different dicom files.
    /// <see cref="DicomFile"/> and <see cref="Context"/> must be set prior 
    /// to the command execution.
    /// </remark>
    sealed class ReconcileRelocateFileCommand : ServerDatabaseCommand, IReconcileServerCommand
    {
        #region Private Members
        private ReconcileStudyProcessorContext _context;
        private ServerFilesystemInfo _destFilesystemInfo;
        private StudyStorageLocation _destStudyStorage;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="ReconcileRelocateFileCommand"/>
        /// </summary>
        public ReconcileRelocateFileCommand()
            : base("Relocate Dicom File", true)
        {
        }
        #endregion

        #region Public Properties
        
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
            string studyInstanceUid = _context.ReconcileImage.DataSet[DicomTags.StudyInstanceUid].GetString(0, String.Empty);

            if (_context.DestStorageLocation==null)
            {
                // DestinationStorageLocation is not yet assigned. Use the ExistingStudyStorageLocation to determine
                // if a new entry should be inserted in to the database.
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
                    Filesystem fs = selector.SelectFilesystem(_context.ReconcileImage);
                    _destFilesystemInfo = FilesystemMonitor.Instance.GetFilesystemInfo(fs.GetKey());
                    _destStudyStorage = CreateStudyLocation();
                } 
            }
            else
            {
                // DestinationStorageLocation is already assigned.
                if (_context.DestStorageLocation.StudyInstanceUid == studyInstanceUid)
                {
                    // reuse
                    _destStudyStorage = _context.DestStorageLocation;
                    _destFilesystemInfo = _context.DestFilesystem;
                }
                else
                {
                    // the study instance uid is not the same as what we used previously... something is wrong.
                    throw new ApplicationException("Different Study Instance Uid detected in the images");
                }
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
            string studyInstanceUid = _context.ReconcileImage.DataSet[DicomTags.StudyInstanceUid].GetString(0, String.Empty);
            string studyDate = _context.ReconcileImage.DataSet[DicomTags.StudyDate].GetString(0, String.Empty);

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using(IUpdateContext updateContext = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IInsertStudyStorage locInsert = updateContext.GetBroker<IInsertStudyStorage>();
                StudyStorageInsertParameters insertParms = new StudyStorageInsertParameters();
                insertParms.ServerPartitionKey = Context.Partition.GetKey();
                insertParms.StudyInstanceUid = studyInstanceUid;
                insertParms.Folder = studyDate;
                insertParms.FilesystemKey = _destFilesystemInfo.Filesystem.GetKey();

                if (_context.ReconcileImage.TransferSyntax.LosslessCompressed)
                {
                    insertParms.TransferSyntaxUid = _context.ReconcileImage.TransferSyntax.UidString;
                    insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossless;
                }
                else if (_context.ReconcileImage.TransferSyntax.LossyCompressed)
                {
                    insertParms.TransferSyntaxUid = _context.ReconcileImage.TransferSyntax.UidString;
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
            
            Initialize();

            String seriesInstanceUid = _context.ReconcileImage.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            String sopInstanceUid = _context.ReconcileImage.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);

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
                Platform.Log(LogLevel.Warn, "Image {0} is discarded because of duplicate", _context.ReconcileImage.Filename);
                return;

                #endregion
            }


            SaveDicomFileCommand saveCommand = new SaveDicomFileCommand(path, _context.ReconcileImage);
            processor.AddCommand(saveCommand);
            processor.AddCommand(new UpdateWorkQueueCommand(_context.ReconcileImage, _destStudyStorage, dupImage, extension));

            if (!processor.Execute())
            {
                throw new ApplicationException(String.Format("Unable to reconcile image {0} : {1}", _context.ReconcileImage.Filename, processor.FailureReason));
            }
        }

        #endregion

    }
}