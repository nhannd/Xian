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
			: base("Insert ArchiveStudyStorage")
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
		/// <param name="theProcessor">The processor executing the command.</param>
		protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
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