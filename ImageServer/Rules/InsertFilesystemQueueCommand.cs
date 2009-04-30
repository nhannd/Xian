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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Rules
{
	/// <summary>
	/// Command for inserting a FilesystemQueue record into the persistent store.
	/// </summary>
    public class InsertFilesystemQueueCommand : ServerDatabaseCommand
    {
        private readonly ServerEntityKey _filesystemKey;
        private readonly FilesystemQueueTypeEnum _queueType;
        private readonly DateTime _scheduledTime;
        private readonly ServerEntityKey _studyStorageKey;
    	private readonly XmlDocument _queueXml;

        public InsertFilesystemQueueCommand(FilesystemQueueTypeEnum queueType, ServerEntityKey filesystemKey,
                                            ServerEntityKey studyStorageKey, DateTime scheduledTime, XmlDocument queueXml)
            : base("Insert FilesystemQueue Record of type " + queueType, true)
        {
            _queueType = queueType;
            _filesystemKey = filesystemKey;
            _studyStorageKey = studyStorageKey;
            _scheduledTime = scheduledTime;
        	_queueXml = queueXml;
        }

        protected override void OnExecute(IUpdateContext updateContext)
        {
            FilesystemQueueInsertParameters parms = new FilesystemQueueInsertParameters();

            parms.FilesystemQueueTypeEnum = _queueType;
            parms.ScheduledTime = _scheduledTime;
            parms.StudyStorageKey = _studyStorageKey;
            parms.FilesystemKey = _filesystemKey;

			if (_queueXml != null)
				parms.QueueXml = _queueXml;

            IInsertFilesystemQueue insertQueue = updateContext.GetBroker<IInsertFilesystemQueue>();

            if (false == insertQueue.Execute(parms))
            {
                Platform.Log(LogLevel.Error, "Unexpected failure inserting FilesystemQueue entry");
                throw new PersistenceException("Unexpected failure inserting FilesystemQueue entry", null);
            }
        }
    }
}