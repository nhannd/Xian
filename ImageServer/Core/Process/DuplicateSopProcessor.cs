using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Process
{
    class DuplicateSopProcessorContext
    {
        public ServerCommandProcessor CommandProcessor;
        public ServerPartition Partition;
        public StudyStorageLocation StudyLocation;
    }

    public class DuplicateSopProcessor
    {
        private const string DUPLICATE_EXTENSION = "dup";
        private const string RECONCILE_STORAGE_FOLDER = "Reconcile";
        private readonly DuplicateSopProcessorContext _context;

        public DuplicateSopProcessor(
            ServerCommandProcessor processor, 
            ServerPartition partition,
            StudyStorageLocation studyLocation
            )
        {
            Platform.CheckForNullReference(processor, "processor");
            Platform.CheckForNullReference(studyLocation, "studyLocation");
            Platform.CheckForNullReference(partition, "partition");

            _context = new DuplicateSopProcessorContext();
            _context.CommandProcessor = processor;
            _context.StudyLocation = studyLocation;
            _context.Partition = partition;

            if (studyLocation.Study==null)
            {
                studyLocation.LoadStudy(ExecutionContext.Current.PersistenceContext);
            }

        }

        private static void SetError(DicomSopProcessingResult result, DicomStatus status, String message)
        {
            result.Sussessful = false;
            result.DicomStatus = status;
            result.ErrorMessage = message;
        }

        public DicomSopProcessingResult Process(String sourceId,String uidGroup, DicomFile file)
        {
            Platform.CheckForNullReference(sourceId, "sourceId");
            Platform.CheckForNullReference(uidGroup, "uidGroup");
            Platform.CheckForNullReference(file, "file");

            Platform.CheckForNullReference(_context, "_context");
            Platform.CheckForNullReference(_context.StudyLocation, "_context.StudyLocation");

            DicomSopProcessingResult result = new DicomSopProcessingResult();
            string failureMessage;

            String sopInstanceUid = file.MediaStorageSopInstanceUid;
            Study study = _context.StudyLocation.Study;
            ServerPartition partition = _context.Partition;

            if (_context.Partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.SendSuccess))
            {
                result.DicomStatus = DicomStatuses.Success;

                Platform.Log(LogLevel.Info, "Duplicate SOP Instance received, sending success response {0}", sopInstanceUid);
                return result;
            }
            else if (_context.Partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.RejectDuplicates))
            {
                failureMessage = String.Format("Duplicate SOP Instance received, rejecting {0}", sopInstanceUid);
                Platform.Log(LogLevel.Info, failureMessage);
                SetError(result, DicomStatuses.DuplicateSOPInstance, failureMessage);
                throw new ApplicationException("Duplicate SOP Instance received.");
            }
            else if (_context.Partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.CompareDuplicates))
            {
                SaveDuplicate(file, uidGroup);
                InsertWorkQueue(file, uidGroup);
            }
            else
            {
                failureMessage = String.Format("Duplicate SOP Instance received. Unsupported duplicate policy {0}.", partition.DuplicateSopPolicyEnum);
                SetError(result, DicomStatuses.DuplicateSOPInstance, failureMessage);
                throw new NotSupportedException(
                    String.Format("Not supported duplicate policy: {0}", partition.DuplicateSopPolicyEnum));
            }
            

            return result;
        }

        private void InsertWorkQueue(DicomFile file, string uidGroup)
        {
            String seriesUid = file.DataSet[DicomTags.SeriesInstanceUid].ToString();
            String sopUid = file.DataSet[DicomTags.SopInstanceUid].ToString();
            
            String relativePath = StringUtilities.Combine(new string[] 
                                                              {
                                                                  _context.StudyLocation.StudyInstanceUid, seriesUid, sopUid
                                                              }, Path.DirectorySeparatorChar.ToString());

            relativePath = relativePath + "." + DUPLICATE_EXTENSION;

            _context.CommandProcessor.AddCommand(
                new UpdateWorkQueueCommand(file, _context.StudyLocation, true, DUPLICATE_EXTENSION, uidGroup, relativePath));

        }

        private void SaveDuplicate(DicomFile file, string uidGroup)
        {
            String seriesUid = file.DataSet[DicomTags.SeriesInstanceUid].ToString();
            String sopUid = file.DataSet[DicomTags.SopInstanceUid].ToString();

            String path = Path.Combine(_context.StudyLocation.FilesystemPath, _context.StudyLocation.PartitionFolder);
            _context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, RECONCILE_STORAGE_FOLDER);
            _context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));


            path = Path.Combine(path, uidGroup);
            _context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, _context.StudyLocation.StudyInstanceUid);
            _context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, seriesUid);
            _context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));
            
            path = Path.Combine(path, sopUid);
            path += "." +DUPLICATE_EXTENSION;

            _context.CommandProcessor.AddCommand(new SaveDicomFileCommand(path, file, true, true));

        }
    }
}