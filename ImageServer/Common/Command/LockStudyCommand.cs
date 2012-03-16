#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Common.Command
{
	/// <summary>
	/// <see cref="ServerDatabaseCommand"/> for Locking or Unlocking a Study.
	/// </summary>
	public class LockStudyCommand : ServerDatabaseCommand
	{
		private readonly QueueStudyStateEnum _queueStudyState;
		private readonly bool? _writeLock;
		private readonly bool? _readLock;
		private readonly ServerEntityKey _studyStorageKey;

		public LockStudyCommand(ServerEntityKey studyStorageKey, QueueStudyStateEnum studyState) : base("LockStudy")
		{
			_studyStorageKey = studyStorageKey;
			_queueStudyState = studyState;
		}

		public LockStudyCommand(ServerEntityKey studyStorageKey, bool writeLock, bool readLock)
			: base("LockStudy")
		{
			_studyStorageKey = studyStorageKey;
			_readLock = readLock;
			_writeLock = writeLock;
		}

		protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
		{
			ILockStudy lockStudyBroker = updateContext.GetBroker<ILockStudy>();
			LockStudyParameters lockParms = new LockStudyParameters();
			lockParms.StudyStorageKey = _studyStorageKey;
			if (_queueStudyState != null)
				lockParms.QueueStudyStateEnum = _queueStudyState;
			if (_writeLock.HasValue)
				lockParms.WriteLock = _writeLock.Value;
			if (_readLock.HasValue)
				lockParms.ReadLock = _readLock.Value;
			bool retVal = lockStudyBroker.Execute(lockParms);

			if (!retVal || !lockParms.Successful)
			{
				throw new ApplicationException(String.Format("Unable to lock the study: {0}", lockParms.FailureReason));
			}
		}
	}
}
