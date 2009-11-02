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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ProcessDuplicate
{
    internal class ProcessReplacedSOPInstanceCommand : ServerDatabaseCommand
    {
        #region Private Members
		
        private readonly ServerPartition _partition;
        private readonly DicomFile _file;
        private StudyStorageLocation _storageLocation;
        private ProcessingResult _result;
        private readonly StudyXml _studyXml;
        private readonly bool _compare;
        private readonly Model.WorkQueue _item;
        private readonly WorkQueueUid _uid;
 
        #endregion


        #region Constructors

        public ProcessReplacedSOPInstanceCommand(Model.WorkQueue item, WorkQueueUid uid, ServerPartition partition, StudyXml studyXml, DicomFile file, bool compare)
            : base("ProcessReplacedSOPInstanceCommand", true)
        {
            _item = item;
            _uid = uid;
            _partition = partition;
            _file = file;
            _compare = compare;
            _studyXml = studyXml;
        }

        
        #endregion

        #region Overridden Protected Methods

        protected override void OnExecute(ServerCommandProcessor theProcessor, IUpdateContext updateContext)
        {
            String studyUid = _file.DataSet[DicomTags.StudyInstanceUid].ToString();
            
            if (!FilesystemMonitor.Instance.GetOnlineStudyStorageLocation(updateContext, _partition.GetKey(), studyUid, true, out _storageLocation))
            {
                throw new ApplicationException("No online storage found");
            }
            StudyProcessorContext context = new StudyProcessorContext(_storageLocation);
            SopInstanceProcessor sopInstanceProcessor = new SopInstanceProcessor(context) {EnforceNameRules = true};
            string group = _uid.GroupID ?? ServerHelper.GetUidGroup(_file, _partition, _item.InsertTime);

            _result = sopInstanceProcessor.ProcessFile(group, _file, _studyXml, _compare, null, null);
            if (_result.Status == ProcessingStatus.Failed)
            {
                throw new ApplicationException("Unable to process file");
            }
        }

        protected override void OnUndo()
        {

        }

        #endregion
    }
}