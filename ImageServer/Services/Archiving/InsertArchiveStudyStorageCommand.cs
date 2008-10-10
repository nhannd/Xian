#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Archiving
{
	/// <summary>
	/// Command to insert an <see cref="ArchiveStudyStorage"/> record and update the <see cref="ArchiveQueue"/>
	/// </summary>
	public class InsertArchiveStudyStorageCommand : ServerDatabaseCommand
	{
		private readonly ServerEntityKey _studyStorageKey;
		private readonly ServerEntityKey _partitionArchiveKey;
		private readonly ServerEntityKey _archiveQueueKey;
		private readonly ServerEntityKey _serverTransferSyntaxKey;
		private readonly XmlDocument _archiveXml;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="studyStorageKey">The <see cref="StudyStorage"/> table key.</param>
		/// <param name="partitionArchiveKey">The <see cref="PartitionArchive"/> table key.</param>
		/// <param name="archiveQueueKey">The <see cref="ArchiveQueue"/> entry key.</param>
		/// <param name="serverTransferSyntaxKey">The <see cref="ServerTransferSyntax"/> entity key.</param>
		/// <param name="archiveXml">XML Specific archive data to be stored in the <see cref="ArchiveStudyStorage"/> table.</param>
		public InsertArchiveStudyStorageCommand(ServerEntityKey studyStorageKey,
		                                        ServerEntityKey partitionArchiveKey,
		                                        ServerEntityKey archiveQueueKey,
		                                        ServerEntityKey serverTransferSyntaxKey,
		                                        XmlDocument archiveXml)
			: base("Insert ArchiveStudyStorage", true)
		{
			_studyStorageKey = studyStorageKey;
			_partitionArchiveKey = partitionArchiveKey;
			_archiveQueueKey = archiveQueueKey;
			_serverTransferSyntaxKey = serverTransferSyntaxKey;
			_archiveXml = archiveXml;
		}

		/// <summary>
		/// Execute the command
		/// </summary>
		/// <param name="updateContext">Database update context.</param>
		protected override void OnExecute(IUpdateContext updateContext)
		{
			ArchiveStudyStorageUpdateColumns columns = new ArchiveStudyStorageUpdateColumns();

			columns.ArchiveTime = Platform.Time;
			columns.PartitionArchiveKey = _partitionArchiveKey;
			columns.StudyStorageKey = _studyStorageKey;
			columns.ArchiveXml = _archiveXml;
			columns.ServerTransferSyntaxKey = _serverTransferSyntaxKey;

			IArchiveStudyStorageEntityBroker insertBroker = updateContext.GetBroker<IArchiveStudyStorageEntityBroker>();

			ArchiveStudyStorage storage = insertBroker.Insert(columns);


			UpdateArchiveQueueParameters parms = new UpdateArchiveQueueParameters();
			parms.ArchiveQueueKey = _archiveQueueKey;
			parms.ArchiveQueueStatusEnum = ArchiveQueueStatusEnum.Completed;
			parms.ScheduledTime = Platform.Time;
			parms.StudyStorageKey = _studyStorageKey;		

			IUpdateArchiveQueue broker = updateContext.GetBroker<IUpdateArchiveQueue>();

            if (!broker.Execute(parms))
                throw new ApplicationException("InsertArchiveStudyStorageCommand failed");
		}
	}
}