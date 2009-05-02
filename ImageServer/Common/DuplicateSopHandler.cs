using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common
{
    internal class DuplicateSopHandler
    {
        private ServerCommandProcessor _processor;
        private StudyStorageLocation _studyLocation;
        private ServerPartition _partition;
        private Study _study;
        private string _receiverId;
        private string _sourceId;

        public DuplicateSopHandler(
            String sourceId,
            String receiverId,
            ServerCommandProcessor processor, 
            ServerPartition partition,
            StudyStorageLocation studyLocation)
        {
            Platform.CheckForNullReference(sourceId, "sourceId");
            Platform.CheckForNullReference(receiverId, "receiverId");
            Platform.CheckForNullReference(processor, "processor");
            Platform.CheckForNullReference(studyLocation, "studyLocation");
            Platform.CheckForNullReference(partition, "partition");

            _receiverId = receiverId;
            _sourceId = sourceId;
            _processor = processor;
            _studyLocation = studyLocation;
            _partition = partition;
        }

        protected Study Study
        {
            get
            {
                if (_study==null)
                {
                    _study = _studyLocation.LoadStudy(_processor.ExecutionContext.ReadContext);
                
                }
                return _study;
            }
        }

        private static void SetError(DicomSopProcessingResult result, DicomStatus status, String message)
        {
            result.Sussessful = false;
            result.DicomStatus = status;
            result.ErrorMessage = message;
        }


        public DicomSopProcessingResult Handle(DicomFile file)
        {
            Platform.CheckForNullReference(file, "file");
            DicomSopProcessingResult result = new DicomSopProcessingResult();
            string failureMessage;

            String sopInstanceUid = file.MediaStorageSopInstanceUid;

                
            if (_partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.SendSuccess))
            {
                result.DicomStatus = DicomStatuses.Success;

                Platform.Log(LogLevel.Info, "Duplicate SOP Instance received, sending success response {0}", sopInstanceUid);
                return result;
            }
            else if (_partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.RejectDuplicates))
            {
                failureMessage = String.Format("Duplicate SOP Instance received, rejecting {0}", sopInstanceUid);
                Platform.Log(LogLevel.Info, failureMessage);
                SetError(result, DicomStatuses.DuplicateSOPInstance, failureMessage);
                throw new ApplicationException("Duplicate SOP Instance received.");
            }
            else if (_partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.CompareDuplicates))
            {
                if (Study!=null)
                    Platform.Log(LogLevel.Info, "Receive duplicate SOP {0} for patient {1}", sopInstanceUid, Study.PatientsName);
                else
                    Platform.Log(LogLevel.Info, "Receive duplicate SOP {0}. Existing files haven't been processed.", sopInstanceUid);

                DoCompareDuplicate(file);

            }
            else
            {
                throw new NotSupportedException(
                    String.Format("Not supported duplicate policy: {0}", _partition.DuplicateSopPolicyEnum));
            }
            

            return result;
        }

        private void DoCompareDuplicate(DicomFile file)
        {
            InsertDuplicateQueueEntryCommand insertCommand = new InsertDuplicateQueueEntryCommand(_receiverId, _sourceId, _studyLocation, this.Study, file);
            _processor.AddCommand(insertCommand);

            _processor.AddCommand(new UpdateDuplicateQueueEntryCommand(
                delegate() { return insertCommand.QueueEntry; },
                file));

            String path = Path.Combine(_studyLocation.FilesystemPath, _studyLocation.PartitionFolder);
            _processor.AddCommand(new CreateDirectoryCommand(path));
            
            _processor.AddCommand(new CreateDirectoryCommand(delegate() { 
                                                                            path = Path.Combine(path, "Duplicate");
                                                                            return path;
            }));


            _processor.AddCommand(new CreateDirectoryCommand(delegate()
                                                                 {
                                                                     path = Path.Combine(path, file.DataSet[DicomTags.StudyInstanceUid]);
                                                                     return path;
                                                                 }));
            
            _processor.AddCommand( new CreateDirectoryCommand(delegate()
                                                                  {
                                                                      path = Path.Combine(path, insertCommand.QueueEntry.GetKey().Key.ToString());
                                                                      return path;
                                                                  }));

           
            _processor.AddCommand(new SaveDicomFileCommand(delegate()
                                                               {
                                                                   path = Path.Combine(path, file.MediaStorageSopInstanceUid);
                                                                   path += ".dup";
                                                                   return path;
                                                               }, file, true, true));
        }
    }
}