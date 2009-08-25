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
	/// <see cref="ServerCommand"/> for inserting into the <see cref="ArchiveQueue"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Note that This stored procedure checks to see if a study delete record has been 
	/// inserted into the database, so it should be called after the rules engine has 
	/// been run & appropriate records inserted into the database.
	/// </para>
	/// <para>
	/// Note also that it can be called when we reprocess a study.
	/// </para>
	/// </remarks>
	public class InsertArchiveQueueCommand : ServerDatabaseCommand
	{
		private readonly ServerEntityKey _serverPartitionKey;
		private readonly ServerEntityKey _studyStorageKey;
		

		public InsertArchiveQueueCommand(ServerEntityKey serverPartitionKey, ServerEntityKey studyStorageKey)
			: base("Insert ArchiveQueue record", true)
		{
			_serverPartitionKey = serverPartitionKey;

			_studyStorageKey = studyStorageKey;

		}

		protected override void OnExecute(ServerCommandProcessor theProcessor, IUpdateContext updateContext)
		{
			// Setup the insert parameters
			InsertArchiveQueueParameters parms = new InsertArchiveQueueParameters();
			parms.ServerPartitionKey = _serverPartitionKey;
			parms.StudyStorageKey = _studyStorageKey;
			
			// Get the Insert ArchiveQueue broker and do the insert
			IInsertArchiveQueue insert = updateContext.GetBroker<IInsertArchiveQueue>();

			// Do the insert
            if (!insert.Execute(parms))
                throw new ApplicationException("InsertArchiveQueueCommand failed");
		}
	}
}
