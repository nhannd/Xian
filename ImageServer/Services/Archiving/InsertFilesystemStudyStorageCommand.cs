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

using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Archiving
{
	/// <summary>
	/// Command to insert a <see cref="FilesystemStudyStorage"/> record in the database
	/// and update the Study status.
	/// </summary>
	public class InsertFilesystemStudyStorageCommand : ServerDatabaseCommand
	{
		private readonly ServerEntityKey _serverPartitionKey;
		private readonly string _studyInstanceUid;
		private readonly string _folder;
		private readonly ServerEntityKey _filesystemKey;
		private readonly TransferSyntax _transfersyntax;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="serverPartitionKey">The <see cref="ServerPartition"/> the study belongs to.</param>
		/// <param name="studyInstanceUid">The Study</param>
		/// <param name="folder">The folder (typically the study date) where the study is stored.</param>
		/// <param name="filesystemKey">The filesystem the study is stored on.</param>
		/// <param name="transferSyntax">The <see cref="TransferSyntax"/> of the study.</param>
		public InsertFilesystemStudyStorageCommand(ServerEntityKey serverPartitionKey,
													 string studyInstanceUid,
													 string folder,
													 ServerEntityKey filesystemKey,
													 TransferSyntax transferSyntax)
			: base("Insert FilesystemStudyStorage", true)
		{
			_serverPartitionKey = serverPartitionKey;
			_studyInstanceUid = studyInstanceUid;
			_folder = folder;
			_filesystemKey = filesystemKey;
			_transfersyntax = transferSyntax;
		}

		/// <summary>
		/// Execute the insert.
		/// </summary>
		/// <param name="updateContext">The persistent store connection to use for the update.</param>
		protected override void OnExecute(IUpdateContext updateContext)
		{
			IInsertStudyStorage locInsert = updateContext.GetBroker<IInsertStudyStorage>();
			InsertStudyStorageParameters insertParms = new InsertStudyStorageParameters();
			insertParms.ServerPartitionKey = _serverPartitionKey;
			insertParms.StudyInstanceUid = _studyInstanceUid;
			insertParms.Folder = _folder;
			insertParms.FilesystemKey = _filesystemKey;
			insertParms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;

			if (_transfersyntax.LosslessCompressed)
			{
				insertParms.TransferSyntaxUid = _transfersyntax.UidString;
				insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossless;
			}
			else if (_transfersyntax.LossyCompressed)
			{
				insertParms.TransferSyntaxUid = _transfersyntax.UidString;
				insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossy;
			}
			else
			{
				insertParms.TransferSyntaxUid = TransferSyntax.ExplicitVrLittleEndianUid;
				insertParms.StudyStatusEnum = StudyStatusEnum.Online;
			}

			// Find one so we don't uselessly process all the results.
			locInsert.FindOne(insertParms);
		}
	}
}
