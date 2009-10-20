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
    /// <summary>
    /// Represents the context during processing of DICOM object.
    /// </summary>
    public class SopProcessingContext
    {
        #region Private Members
        private readonly ServerCommandProcessor _commandProcessor;
        private readonly StudyStorageLocation _studyLocation;
        private readonly string _group;
        private readonly ServerPartition _partition; 
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="SopProcessingContext"/>
        /// </summary>
        /// <param name="commandProcessor">The <see cref="ServerCommandProcessor"/> used in the context</param>
        /// <param name="studyLocation">The <see cref="StudyStorageLocation"/> of the study being processed</param>
        /// <param name="uidGroup">A String value respresenting the group of SOP instances which are being processed.</param>
        public SopProcessingContext(ServerCommandProcessor commandProcessor, StudyStorageLocation studyLocation, string uidGroup)
        {
            _commandProcessor = commandProcessor;
            _studyLocation = studyLocation;
            _group = uidGroup;
            _partition = studyLocation.ServerPartition;
        }
        
        #endregion

        #region Public Properties

        public ServerCommandProcessor CommandProcessor
        {
            get { return _commandProcessor; }
        }

        public StudyStorageLocation StudyLocation
        {
            get { return _studyLocation; }
        }

        public string Group
        {
            get { return _group; }
        }

        #endregion
    }

    /// <summary>
    /// Provides helper method to process duplicates.
    /// </summary>
    static public class DuplicateSopProcessorHelper
    {
        #region Private Members
        // TODO: Make these values configurable
        private const string DUPLICATE_EXTENSION = "dup";
        private const string RECONCILE_STORAGE_FOLDER = "Reconcile"; 
        #endregion

        #region Public Methods

        /// <summary>
        /// Inserts the duplicate DICOM file into the <see cref="WorkQueue"/> for processing (if applicable).
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="file">Thje duplicate DICOM file being processed.</param>
        /// <returns>A <see cref="DicomProcessingResult"/> that contains the result of the processing.</returns>
        /// <remarks>
        /// This method inserts <see cref="ServerCommand"/> into <paramref name="context.CommandProcessor"/>.
        /// The outcome of the operation depends on the <see cref="DuplicateSopPolicyEnum"/> of the <see cref="ServerPartition"/>.
        /// If it is set to <see cref="DuplicateSopPolicyEnum.CompareDuplicates"/>, the duplicate file will be
        /// inserted into the <see cref="WorkQueue"/> for processing.
        /// </remarks>
        static public DicomProcessingResult Process(SopProcessingContext context, DicomFile file)
        {
            Platform.CheckForNullReference(file, "file");
            Platform.CheckForNullReference(context, "context");
            Platform.CheckMemberIsSet(context.Group, "parameters.Group");
            Platform.CheckMemberIsSet(context.CommandProcessor, "parameters.CommandProcessor");
            Platform.CheckMemberIsSet(context.StudyLocation, "parameters.StudyLocation");

            DicomProcessingResult result = new DicomProcessingResult();
            result.DicomStatus = DicomStatuses.Success;
            result.Successful = true;

            string failureMessage;
            String sopInstanceUid = file.MediaStorageSopInstanceUid;


            if (context.StudyLocation.ServerPartition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.SendSuccess))
            {
                Platform.Log(LogLevel.Info, "Duplicate SOP Instance received, sending success response {0}", sopInstanceUid);
                return result;
            }
            else if (context.StudyLocation.ServerPartition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.RejectDuplicates))
            {
                failureMessage = String.Format("Duplicate SOP Instance received, rejecting {0}", sopInstanceUid);
                Platform.Log(LogLevel.Info, failureMessage);
                result.SetError(DicomStatuses.DuplicateSOPInstance, failureMessage);
                return result;
            }
            else if (context.StudyLocation.ServerPartition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.CompareDuplicates))
            {
                SaveDuplicate(context, file);
                InsertWorkQueue(context, file);
            }
            else
            {
                failureMessage = String.Format("Duplicate SOP Instance received. Unsupported duplicate policy {0}.", context.StudyLocation.ServerPartition.DuplicateSopPolicyEnum);
                result.SetError(DicomStatuses.DuplicateSOPInstance, failureMessage);
                return result;
            }


            return result;
        }

        #endregion

        #region Private Methods
        static private void InsertWorkQueue(SopProcessingContext context, DicomFile file)
        {
            String sopUid = file.DataSet[DicomTags.SopInstanceUid].ToString();

            String relativePath = StringUtilities.Combine(new string[] 
                                                              {
                                                                  context.StudyLocation.StudyInstanceUid, sopUid
                                                              }, Path.DirectorySeparatorChar.ToString());

            relativePath = relativePath + "." + DUPLICATE_EXTENSION;

            context.CommandProcessor.AddCommand(
                new UpdateWorkQueueCommand(file, context.StudyLocation, true, DUPLICATE_EXTENSION, context.Group, relativePath));

        }

        static private void SaveDuplicate(SopProcessingContext context, DicomFile file)
        {
            String sopUid = file.DataSet[DicomTags.SopInstanceUid].ToString();

            String path = Path.Combine(context.StudyLocation.FilesystemPath, context.StudyLocation.PartitionFolder);
            context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, RECONCILE_STORAGE_FOLDER);
            context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, context.Group /* the AE title + timestamp */);
            context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, context.StudyLocation.StudyInstanceUid);
            context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, sopUid);
            path += "." + DUPLICATE_EXTENSION;

            context.CommandProcessor.AddCommand(new SaveDicomFileCommand(path, file, true));

            Platform.Log(ServerPlatform.InstanceLogLevel, "Duplicate ==> {0}", path);
        } 
        #endregion
    }
}