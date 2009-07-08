#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.ImageServer.Common;
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

    public class DuplicateSopProcessorHelper
    {
        private const string DUPLICATE_EXTENSION = "dup";
        private const string RECONCILE_STORAGE_FOLDER = "Reconcile";
        private readonly DuplicateSopProcessorContext _context;

        public DuplicateSopProcessorHelper(
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

       

        public DicomProcessingResult Process(String sourceId,String uidGroup, DicomFile file)
        {
            Platform.CheckForNullReference(sourceId, "sourceId");
            Platform.CheckForNullReference(uidGroup, "uidGroup");
            Platform.CheckForNullReference(file, "file");

            Platform.CheckForNullReference(_context, "_context");
            Platform.CheckForNullReference(_context.StudyLocation, "_context.StudyLocation");

            DicomProcessingResult result = new DicomProcessingResult();
            result.DicomStatus = DicomStatuses.Success;
            result.Successful = true;

            string failureMessage;
            String sopInstanceUid = file.MediaStorageSopInstanceUid;
            ServerPartition partition = _context.Partition;

            if (_context.Partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.SendSuccess))
            {
                Platform.Log(LogLevel.Info, "Duplicate SOP Instance received, sending success response {0}", sopInstanceUid);
                return result;
            }
            else if (_context.Partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.RejectDuplicates))
            {
                failureMessage = String.Format("Duplicate SOP Instance received, rejecting {0}", sopInstanceUid);
                Platform.Log(LogLevel.Info, failureMessage);
                result.SetError(DicomStatuses.DuplicateSOPInstance, failureMessage);
            	return result;
            }
            else if (_context.Partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.CompareDuplicates))
            {
                SaveDuplicate(file, uidGroup);
                InsertWorkQueue(file, uidGroup);
            }
            else
            {
                failureMessage = String.Format("Duplicate SOP Instance received. Unsupported duplicate policy {0}.", partition.DuplicateSopPolicyEnum);
                result.SetError(DicomStatuses.DuplicateSOPInstance, failureMessage);
            	return result;
            }
            

            return result;
        }

        private void InsertWorkQueue(DicomFile file, string uidGroup)
        {
            String sopUid = file.DataSet[DicomTags.SopInstanceUid].ToString();
            
            String relativePath = StringUtilities.Combine(new string[] 
                                                              {
                                                                  _context.StudyLocation.StudyInstanceUid, sopUid
                                                              }, Path.DirectorySeparatorChar.ToString());

            relativePath = relativePath + "." + DUPLICATE_EXTENSION;

            _context.CommandProcessor.AddCommand(
                new UpdateWorkQueueCommand(file, _context.StudyLocation, true, DUPLICATE_EXTENSION, uidGroup, relativePath));

        }

        private void SaveDuplicate(DicomFile file, string uidGroup)
        {
            String sopUid = file.DataSet[DicomTags.SopInstanceUid].ToString();

            String path = Path.Combine(_context.StudyLocation.FilesystemPath, _context.StudyLocation.PartitionFolder);
            _context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, RECONCILE_STORAGE_FOLDER);
            _context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, uidGroup /* the AE title + timestamp */);
            _context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, _context.StudyLocation.StudyInstanceUid);
            _context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));
            
            path = Path.Combine(path, sopUid);
            path += "." +DUPLICATE_EXTENSION;

            _context.CommandProcessor.AddCommand(new SaveDicomFileCommand(path, file, true, true));

            Platform.Log(ServerPlatform.InstanceLogLevel, "Duplicate ==> {0}", path);
        }
    }
}