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
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Rules
{
	/// <summary>
	/// <see cref="ArchiveQueue"/> for inserting into the <see cref="CommandBase"/>.
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
			: base("Insert ArchiveQueue record")
		{
			_serverPartitionKey = serverPartitionKey;

			_studyStorageKey = studyStorageKey;

		}

		protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
		{
			// Setup the insert parameters
		    var parms = new InsertArchiveQueueParameters
		                    {
                                ServerPartitionKey = _serverPartitionKey, 
                                StudyStorageKey = _studyStorageKey
                            };

		    // Get the Insert ArchiveQueue broker and do the insert
			var insert = updateContext.GetBroker<IInsertArchiveQueue>();

			// Do the insert
            if (!insert.Execute(parms))
                throw new ApplicationException("InsertArchiveQueueCommand failed");
		}
	}
}
