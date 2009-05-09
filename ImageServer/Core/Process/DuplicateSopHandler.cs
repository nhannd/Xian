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
    internal class DuplicateSopHandler
    {
        private const string DUPLICATE_EXTENSION = "dup";
        private const string RECONCILE_STORAGE_FOLDER = "Reconcile";
        private readonly ServerCommandProcessor _processor;
        private readonly StudyStorageLocation _studyLocation;
        private readonly ServerPartition _partition;
        private Study _study;
        private readonly string _uidGroup;
        private readonly string _sourceId;

        public DuplicateSopHandler(
            String sourceId,
            String uidGroup,
            ServerCommandProcessor processor, 
            ServerPartition partition,
            StudyStorageLocation studyLocation
            )
        {
            Platform.CheckForNullReference(sourceId, "sourceId");
            Platform.CheckForNullReference(processor, "processor");
            Platform.CheckForNullReference(studyLocation, "studyLocation");
            Platform.CheckForNullReference(partition, "partition");
            Platform.CheckForNullReference(uidGroup, "uidGroup");

            _sourceId = sourceId;
            _processor = processor;
            _studyLocation = studyLocation;
            _partition = partition;
            _uidGroup = uidGroup;
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

                SaveDuplicate(file);
                InsertWorkQueue(file);

            }
            else
            {
                throw new NotSupportedException(
                    String.Format("Not supported duplicate policy: {0}", _partition.DuplicateSopPolicyEnum));
            }
            

            return result;
        }

        private void InsertWorkQueue(DicomFile file)
        {
            String seriesUid = file.DataSet[DicomTags.SeriesInstanceUid].ToString();
            String sopUid = file.DataSet[DicomTags.SopInstanceUid].ToString();
            String queueGroup = _sourceId;
            String uidGroup = _uidGroup;

            String relativePath = StringUtilities.Combine(new string[] 
                                                              {
                                                                  _studyLocation.StudyInstanceUid, seriesUid, sopUid
                                                              }, Path.DirectorySeparatorChar.ToString());

            relativePath = relativePath + "." + DUPLICATE_EXTENSION;

            _processor.AddCommand(new UpdateWorkQueueCommand(file, _studyLocation, true, DUPLICATE_EXTENSION, uidGroup, relativePath));

        }

        private void SaveDuplicate(DicomFile file)
        {
            String seriesUid = file.DataSet[DicomTags.SeriesInstanceUid].ToString();
            String sopUid = file.DataSet[DicomTags.SopInstanceUid].ToString();

            String path = Path.Combine(_studyLocation.FilesystemPath, _studyLocation.PartitionFolder);
            _processor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, RECONCILE_STORAGE_FOLDER);
            _processor.AddCommand(new CreateDirectoryCommand(path));


            path = Path.Combine(path, _uidGroup);
            _processor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, _studyLocation.StudyInstanceUid);
            _processor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, seriesUid);
            _processor.AddCommand(new CreateDirectoryCommand(path));
            
            path = Path.Combine(path, sopUid);
            path += "." +DUPLICATE_EXTENSION;

            _processor.AddCommand(new SaveDicomFileCommand(path, file, true, true));

        }

        //private void DoCompareDuplicate(DicomFile file)
        //{
        //    InsertDuplicateQueueEntryCommand insertCommand = new InsertDuplicateQueueEntryCommand(_receiverId, _sourceId, _studyLocation, Study, file);
        //    _processor.AddCommand(insertCommand);

        //    _processor.AddCommand(new UpdateDuplicateQueueEntryCommand(
        //                            delegate { return insertCommand.QueueEntry; },
        //                            file));

        //    String path = Path.Combine(_studyLocation.FilesystemPath, _studyLocation.PartitionFolder);
        //    _processor.AddCommand(new CreateDirectoryCommand(path));
            
        //    _processor.AddCommand(new CreateDirectoryCommand(delegate
        //                                                        { 
        //                                                                    path = Path.Combine(path, "Duplicate");
        //                                                                    return path;
        //    }));


        //    _processor.AddCommand(new CreateDirectoryCommand(delegate
        //                                                        {
        //                                                            path = Path.Combine(path, file.DataSet[DicomTags.StudyInstanceUid]);
        //                                                            return path;
        //                                                        }));
            
        //    _processor.AddCommand( new CreateDirectoryCommand(delegate
        //                                                        {
        //                                                            path = Path.Combine(path, insertCommand.QueueEntry.GetKey().Key.ToString());
        //                                                            return path;
        //                                                        }));

           
        //    _processor.AddCommand(new SaveDicomFileCommand(delegate
        //                                                    {
        //                                                        path = Path.Combine(path, file.MediaStorageSopInstanceUid);
        //                                                        path += ".dup";
        //                                                        return path;
        //                                                    }, file, true, true));
        //}
    }
}