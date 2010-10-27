#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	public class UpdateStudyStateCommand : ServerDatabaseCommand
	{
		private readonly StudyStatusEnum _newStatus;
		private readonly ServerTransferSyntax _newSyntax;
		private readonly StudyStorageLocation _location;

		public UpdateStudyStateCommand(StudyStorageLocation location, StudyStatusEnum status, ServerTransferSyntax newSyntax) : base("Update the StudyState", true)
		{
			_location = location;
			_newStatus = status;
			_newSyntax = newSyntax;
		}

		protected override void OnExecute(ServerCommandProcessor theProcessor, IUpdateContext updateContext)
		{
			// Update StudyStatusEnum in the StudyStorageTable
			IStudyStorageEntityBroker studyStorageUpdate = updateContext.GetBroker<IStudyStorageEntityBroker>();
			StudyStorageUpdateColumns studyStorageUpdateColumns = new StudyStorageUpdateColumns();
			studyStorageUpdateColumns.StudyStatusEnum = _newStatus;
			studyStorageUpdate.Update(_location.Key, studyStorageUpdateColumns);

			// Update ServerTransferSyntaxGUID in FilesystemStudyStorage
			IFilesystemStudyStorageEntityBroker filesystemUpdate = updateContext.GetBroker<IFilesystemStudyStorageEntityBroker>();
			FilesystemStudyStorageUpdateColumns filesystemUpdateColumns = new FilesystemStudyStorageUpdateColumns();
			filesystemUpdateColumns.ServerTransferSyntaxKey = _newSyntax.Key;
			filesystemUpdate.Update(_location.FilesystemStudyStorageKey, filesystemUpdateColumns);
		}
	}
}
