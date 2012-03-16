#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Command;
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
            : base("Insert FilesystemQueue Record of type " + queueType)
        {
            _queueType = queueType;
            _filesystemKey = filesystemKey;
            _studyStorageKey = studyStorageKey;
            _scheduledTime = scheduledTime;
        	_queueXml = queueXml;
        }

        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
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