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

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core
{
	/// <summary>
	/// Update the <see cref="StudyStorage"/> and <see cref="FilesystemStudyStorage"/> tables according to 
	/// a new Transfersyntax the Study is encoded as.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This method checks the status and transfer syntax set in the current <see cref="StudyStorageLocation"/>
	/// instance passed to the constructor and compares it to the <see cref="DicomFile"/> passed to the constructor.
	/// If they vary, it will then update the database and update the structure with the new values.  If they're
	/// the same, a database update is bypassed.
	/// </para>
	/// <para>
	/// For efficiency sake, this assumes that the same <see cref="StudyStorageLocation"/> instance is used for 
	/// consecutive calls to this <see cref="ServerDatabaseCommand"/> object.
	/// </para>
	/// </remarks>
	public class UpdateStudyStatusCommand : ServerDatabaseCommand
	{
		private readonly StudyStorageLocation _location;
		private readonly DicomFile _file;

		public UpdateStudyStatusCommand(StudyStorageLocation location, DicomFile file)
			: base("Update StudyStorage and FilesystemStudyStorage", false)
		{
			_location = location;
			_file = file;
		}

		protected override void OnExecute(IUpdateContext updateContext)
		{
			// Check if the File is the same syntax as the 
			TransferSyntax fileSyntax = _file.TransferSyntax;
			TransferSyntax dbSyntax = TransferSyntax.GetTransferSyntax(_location.TransferSyntaxUid);

			// Check if the syntaxes match the location
			if ((!fileSyntax.Encapsulated && !dbSyntax.Encapsulated)
			  || (fileSyntax.LosslessCompressed && dbSyntax.LosslessCompressed)
			  || (fileSyntax.LossyCompressed && dbSyntax.LossyCompressed))
			{
				// no changes necessary, just return;
				return;
			}

			// Select the Server Transfer Syntax
			ServerTransferSyntaxSelectCriteria syntaxCriteria = new ServerTransferSyntaxSelectCriteria();
			IServerTransferSyntaxEntityBroker syntaxBroker = updateContext.GetBroker<IServerTransferSyntaxEntityBroker>();
			syntaxCriteria.Uid.EqualTo(fileSyntax.UidString);

			ServerTransferSyntax serverSyntax = syntaxBroker.FindOne(syntaxCriteria);
			if (serverSyntax == null)
			{
				Platform.Log(LogLevel.Error, "Unable to load ServerTransferSyntax for {0}.  Unable to update study status.", fileSyntax.Name);
				return;
			}

			// Get the FilesystemStudyStorage update broker ready
			IFilesystemStudyStorageEntityBroker filesystemStudyStorageEntityBroker = updateContext.GetBroker<IFilesystemStudyStorageEntityBroker>();
			FilesystemStudyStorageUpdateColumns filesystemStorageUpdate = new FilesystemStudyStorageUpdateColumns();
			FilesystemStudyStorageSelectCriteria filesystemStorageCritiera = new FilesystemStudyStorageSelectCriteria();

			filesystemStorageUpdate.ServerTransferSyntaxKey = serverSyntax.Key;
			filesystemStorageCritiera.StudyStorageKey.EqualTo(_location.Key);

			// Get the StudyStorage update broker ready
			IStudyStorageEntityBroker studyStorageBroker =
				updateContext.GetBroker<IStudyStorageEntityBroker>();
			StudyStorageUpdateColumns studyStorageUpdate = new StudyStorageUpdateColumns();
			StudyStatusEnum statusEnum = _location.StudyStatusEnum;
			if (fileSyntax.LossyCompressed)
				studyStorageUpdate.StudyStatusEnum = statusEnum = StudyStatusEnum.OnlineLossy;
			else if (fileSyntax.LosslessCompressed)
				studyStorageUpdate.StudyStatusEnum = statusEnum = StudyStatusEnum.OnlineLossless;

			studyStorageUpdate.LastAccessedTime = Platform.Time;

			if (!filesystemStudyStorageEntityBroker.Update(filesystemStorageCritiera, filesystemStorageUpdate))
			{
				Platform.Log(LogLevel.Error, "Unable to update FilesystemQueue row: Study {0}, Server Entity {1}",
							 _location.StudyInstanceUid, _location.ServerPartitionKey);

			}
			else if (!studyStorageBroker.Update(_location.GetKey(), studyStorageUpdate))
			{
				Platform.Log(LogLevel.Error, "Unable to update StudyStorage row: Study {0}, Server Entity {1}",
							 _location.StudyInstanceUid, _location.ServerPartitionKey);
			}
			else
			{
				// Update the location, so the next time we come in here, we don't try and update the database
				// for another sop in the study.
				_location.StudyStatusEnum = statusEnum;
				_location.TransferSyntaxUid = fileSyntax.UidString;
				_location.ServerTransferSyntaxKey = serverSyntax.Key;
			}
		}
	}
}
