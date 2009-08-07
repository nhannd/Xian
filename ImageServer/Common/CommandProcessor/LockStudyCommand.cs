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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
	/// <summary>
	/// <see cref="ServerDatabaseCommand"/> for Locking or Unlocking a Study.
	/// </summary>
	public class LockStudyCommand : ServerDatabaseCommand
	{
		private readonly QueueStudyStateEnum _queueStudyState;
		private readonly bool? _lock;
		private readonly ServerEntityKey _studyStorageKey;

		public LockStudyCommand(ServerEntityKey studyStorageKey, QueueStudyStateEnum studyState) : base("LockStudy", true)
		{
			_studyStorageKey = studyStorageKey;
			_queueStudyState = studyState;
		}

		public LockStudyCommand(ServerEntityKey studyStorageKey, bool lockValue)
			: base("LockStudy", true)
		{
			_studyStorageKey = studyStorageKey;
			_lock = lockValue;
		}

		protected override void OnExecute(IUpdateContext updateContext)
		{
			ILockStudy lockStudyBroker = updateContext.GetBroker<ILockStudy>();
			LockStudyParameters lockParms = new LockStudyParameters();
			lockParms.StudyStorageKey = _studyStorageKey;
			if (_queueStudyState != null)
				lockParms.QueueStudyStateEnum = _queueStudyState;
			if (_lock.HasValue)
				lockParms.Lock = _lock.Value;
			bool retVal = lockStudyBroker.Execute(lockParms);

			if (!retVal || !lockParms.Successful)
			{
				throw new ApplicationException(String.Format("Unable to lock the study: {0}", lockParms.FailureReason));
			}
		}
	}
}
