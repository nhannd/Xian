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
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.ImageViewer.Services.DicomServer;

namespace ClearCanvas.ImageViewer.Services.LocalDataStore
{
	[Flags]
	public enum CancellationFlags
	{
		None = 0,
		Cancel = 1,
		Clear = 2
	}

	//TODO: One of these days clean up the services and contracts to be in the right places(s).
	//Some of the 'local data store' contracts really belong in the Dicom Server.
	#region Send / Receive contracts

	[DataContract]
	public class RetrieveStudyInformation
	{
		private string _fromAETitle;
		private StudyInformation _studyInformation;
		
		public RetrieveStudyInformation()
		{ 
		}

		[DataMember(IsRequired = true)]
		public string FromAETitle
		{
			get { return _fromAETitle; }
			set { _fromAETitle = value; }
		}

		[DataMember(IsRequired = true)]
		public StudyInformation StudyInformation
		{
			get { return _studyInformation; }
			set { _studyInformation = value; }
		}
	}

	[DataContract]
	public class ReceiveErrorInformation
	{
		private string _fromAETitle;
		private StudyInformation _studyInformation;
		private string _errorMessage;

		public ReceiveErrorInformation()
		{ 
		}

		[DataMember(IsRequired = true)]
		public string FromAETitle
		{
			get { return _fromAETitle; }
			set { _fromAETitle = value; }
		}

		[DataMember(IsRequired = true)]
		public StudyInformation StudyInformation
		{
			get { return _studyInformation; }
			set { _studyInformation = value; }
		}

		[DataMember(IsRequired = true)]
		public string ErrorMessage
		{
			get { return _errorMessage; }
			set { _errorMessage = value;}
		}
	}

	[DataContract]
	public class SendStudyInformation
	{
		private string _toAETitle;
		private StudyInformation _studyInformation;
		private SendOperationReference _sendOperationReference;

		public SendStudyInformation()
		{
		}

		[DataMember(IsRequired = true)]
		public string ToAETitle
		{
			get { return _toAETitle; }
			set { _toAETitle = value; }
		}

		[DataMember(IsRequired = true)]
		public StudyInformation StudyInformation
		{
			get { return _studyInformation; }
			set { _studyInformation = value; }
		}

		[DataMember(IsRequired = false)]
		public SendOperationReference SendOperationReference
		{
			get { return _sendOperationReference; }
			set { _sendOperationReference = value; }
		}
	}

	[DataContract]
	public class SendErrorInformation
	{
		private string _toAETitle;
		private StudyInformation _studyInformation;
		private string _errorMessage;
		private SendOperationReference _sendOperationReference;

		public SendErrorInformation()
		{
		}

		[DataMember(IsRequired = true)]
		public string ToAETitle
		{
			get { return _toAETitle; }
			set { _toAETitle = value; }
		}

		[DataMember(IsRequired = true)]
		public StudyInformation StudyInformation
		{
			get { return _studyInformation; }
			set { _studyInformation = value; }
		}
		
		[DataMember(IsRequired = true)]
		public string ErrorMessage
		{
			get { return _errorMessage; }
			set { _errorMessage = value; }
		}

		[DataMember(IsRequired = false)]
		public SendOperationReference SendOperationReference
		{
			get { return _sendOperationReference; }
			set { _sendOperationReference = value; }
		}
	}

	[DataContract]
	public class StoreScpReceivedFileInformation
	{
		private string _aeTitle;
		private string _fileName;

		public StoreScpReceivedFileInformation()
		{
		}

		[DataMember(IsRequired = true)]
		public string AETitle
		{
			get { return _aeTitle; }
			set { _aeTitle = value; }
		}

		[DataMember(IsRequired = true)]
		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}
	}

	[DataContract]
	public class StoreScuSentFileInformation
	{
		private string _toAETitle;
		private string _fileName;
		private StudyInformation _studyInformation;
		private SendOperationReference _sendOperationReference;

		public StoreScuSentFileInformation()
		{
		}

		[DataMember(IsRequired = false)]
		public StudyInformation StudyInformation
		{
			get { return _studyInformation; }
			set { _studyInformation = value; }
		}

		[DataMember(IsRequired = true)]
		public string ToAETitle
		{
			get { return _toAETitle; }
			set { _toAETitle = value; }
		}

		[DataMember(IsRequired = true)]
		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}

		[DataMember(IsRequired = false)]
		public SendOperationReference SendOperationReference
		{
			get { return _sendOperationReference; }
			set { _sendOperationReference = value; }
		}
	}

	#endregion

	[DataContract]
	public abstract class FileOperationProgressItem
	{
		private Guid _identifier;
		private bool _cancelled;
		private bool _removed;
		private CancellationFlags _allowedCancellationOperations;
		private DateTime _startTime;
		private DateTime _lastActive;
		private string _statusMessage;
		private bool _isBackground;

		public FileOperationProgressItem()
		{ 
		}

		[DataMember(IsRequired = true)]
		public Guid Identifier
		{
			get { return _identifier; }
			set { _identifier = value; }
		}

		[DataMember(IsRequired = true)]
		public CancellationFlags AllowedCancellationOperations
		{
			get { return _allowedCancellationOperations; }
			set { _allowedCancellationOperations = value; }
		}

		[DataMember(IsRequired = true)]
		public bool Cancelled
		{
			get { return _cancelled; }
			set { _cancelled = value; }
		}

		[DataMember(IsRequired = true)]
		public bool Removed
		{
			get { return _removed; }
			set { _removed = value; }
		}

		[DataMember(IsRequired = true)]
		public DateTime StartTime
		{
			get { return _startTime; }
			set { _startTime = value; }
		}

		[DataMember(IsRequired = true)]
		public DateTime LastActive
		{
			get { return _lastActive; }
			set { _lastActive = value; }
		}

		[DataMember(IsRequired = false)]
		public string StatusMessage
		{
			get { return _statusMessage; }
			set { _statusMessage = value; }
		}

		[DataMember(IsRequired = false)]
		public bool IsBackground
		{
			get { return _isBackground; }
			set { _isBackground = value; }
		}

		public void CopyTo(FileOperationProgressItem progressItem)
		{
			progressItem.Identifier = this.Identifier;
			progressItem.AllowedCancellationOperations = this.AllowedCancellationOperations;
			progressItem.Cancelled = this.Cancelled;
			progressItem.Removed = this.Removed;
			progressItem.StartTime = this.StartTime;
			progressItem.LastActive = this.LastActive;
			progressItem.StatusMessage = this.StatusMessage;
			progressItem.IsBackground = this.IsBackground;
		}

		public void CopyFrom(FileOperationProgressItem progressItem)
		{
			this.Identifier = progressItem.Identifier;
			this.AllowedCancellationOperations = progressItem.AllowedCancellationOperations;
			this.Cancelled = progressItem.Cancelled;
			this.Removed = progressItem.Removed;
			this.StartTime = progressItem.StartTime;
			this.LastActive = progressItem.LastActive;
			this.StatusMessage = progressItem.StatusMessage;
			this.IsBackground = progressItem.IsBackground;
		}
	}

	[DataContract]
	public class ImportProgressItem : FileOperationProgressItem
	{
		private string _description;

		private int _totalFilesToImport;

		private int _numberOfParseFailures;
		private int _numberOfImportFailures;
		private int _numberOfDataStoreCommitFailures;

		private int _numberOfFilesParsed;
		private int _numberOfFilesImported;
		private int _numberOfFilesCommittedToDataStore;

		public ImportProgressItem()
		{
		}

		[DataMember(IsRequired = true)]
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		[DataMember(IsRequired = true)]
		public int TotalFilesToImport
		{
			get { return _totalFilesToImport; }
			set { _totalFilesToImport = value; }
		}

		[DataMember(IsRequired = true)]
		public int NumberOfFilesParsed
		{
			get { return _numberOfFilesParsed; }
			set { _numberOfFilesParsed = value; }
		}

		[DataMember(IsRequired = true)]
		public int NumberOfFilesImported
		{
			get { return _numberOfFilesImported; }
			set { _numberOfFilesImported = value; }
		}

		[DataMember(IsRequired = true)]
		public int NumberOfFilesCommittedToDataStore
		{
			get { return _numberOfFilesCommittedToDataStore; }
			set { _numberOfFilesCommittedToDataStore = value; }
		}

		[DataMember(IsRequired = true)]
		public int NumberOfParseFailures
		{
			get { return _numberOfParseFailures; }
			set { _numberOfParseFailures = value; }
		}

		[DataMember(IsRequired = true)]
		public int NumberOfImportFailures
		{
			get { return _numberOfImportFailures; }
			set { _numberOfImportFailures = value; }
		}

		[DataMember(IsRequired = true)]
		public int NumberOfDataStoreCommitFailures
		{
			get { return _numberOfDataStoreCommitFailures; }
			set { _numberOfDataStoreCommitFailures = value; }
		}

		public int TotalImportFailures
		{
			get { return this.NumberOfImportFailures + this.NumberOfParseFailures; }
		}

		public int TotalDataStoreCommitFailures
		{
			get { return this.TotalImportFailures + this.NumberOfDataStoreCommitFailures; }
		}

		public int TotalImportsProcessed
		{
			get { return this.NumberOfFilesImported + this.TotalImportFailures; }
		}

		public int TotalDataStoreCommitsProcessed
		{
			get { return this.NumberOfFilesCommittedToDataStore + this.TotalDataStoreCommitFailures; }
		}

		public bool IsImportComplete()
		{
			return TotalFilesToImport == this.TotalImportsProcessed;
		}

		public bool IsComplete()
		{
			return TotalFilesToImport == this.TotalDataStoreCommitsProcessed;
		}

		public void CopyTo(ImportProgressItem progressItem)
		{
			progressItem.Description = this.Description;

			progressItem.TotalFilesToImport = this.TotalFilesToImport;

			progressItem.NumberOfParseFailures = this.NumberOfParseFailures;
			progressItem.NumberOfImportFailures = this.NumberOfImportFailures;
			progressItem.NumberOfDataStoreCommitFailures = this.NumberOfDataStoreCommitFailures;

			progressItem.NumberOfFilesParsed = this.NumberOfFilesParsed;
			progressItem.NumberOfFilesImported = this.NumberOfFilesImported;
			progressItem.NumberOfFilesCommittedToDataStore = this.NumberOfFilesCommittedToDataStore;

			base.CopyTo(progressItem);
		}

		public void CopyFrom(ImportProgressItem progressItem)
		{
			this.Description = progressItem.Description;

			this.TotalFilesToImport = progressItem.TotalFilesToImport;

			this.NumberOfParseFailures = progressItem.NumberOfParseFailures;
			this.NumberOfImportFailures = progressItem.NumberOfImportFailures;
			this.NumberOfDataStoreCommitFailures = progressItem.NumberOfDataStoreCommitFailures;

			this.NumberOfFilesParsed = progressItem.NumberOfFilesParsed;
			this.NumberOfFilesImported = progressItem.NumberOfFilesImported;
			this.NumberOfFilesCommittedToDataStore = progressItem.NumberOfFilesCommittedToDataStore;

			base.CopyFrom(progressItem);
		}

		public ImportProgressItem Clone()
		{
			ImportProgressItem clone = new ImportProgressItem();
			CopyTo(clone);
			return clone;
		}
	}

	[DataContract]
	public class ExportProgressItem : FileOperationProgressItem
	{
		private int _totalFilesToExport;
		private int _numberOfFilesExported;

		public ExportProgressItem()
		{
			_totalFilesToExport = 0;
			_numberOfFilesExported = 0;
		}

		[DataMember(IsRequired = true)]
		public int TotalFilesToExport
		{
			get { return _totalFilesToExport; }
			set { _totalFilesToExport = value; }
		}

		[DataMember(IsRequired = true)]
		public int NumberOfFilesExported
		{
			get { return _numberOfFilesExported; }
			set { _numberOfFilesExported = value; }
		}

		public void CopyTo(ExportProgressItem progressItem)
		{
			progressItem.TotalFilesToExport = this.TotalFilesToExport;
			progressItem.NumberOfFilesExported = this.NumberOfFilesExported;

			base.CopyTo(progressItem);
		}

		public void CopyFrom(ExportProgressItem progressItem)
		{
			this.TotalFilesToExport = progressItem.TotalFilesToExport;
			this.NumberOfFilesExported = progressItem.NumberOfFilesExported;

			base.CopyFrom(progressItem);
		}

		public ExportProgressItem Clone()
		{
			ExportProgressItem clone = new ExportProgressItem();
			CopyTo(clone);
			return clone;
		}
	}

	[DataContract]
	public class ReceiveProgressItem : ImportProgressItem
	{
		private string _fromAETitle;
		private StudyInformation _studyInformation;

		public ReceiveProgressItem()
		{
		}

		[DataMember(IsRequired = true)]
		public string FromAETitle
		{
			get { return _fromAETitle; }
			set { _fromAETitle = value; }
		}

		[DataMember(IsRequired = true)]
		public StudyInformation StudyInformation
		{
			get { return _studyInformation; }
			set { _studyInformation = value; }
		}

		public int NumberOfFilesReceived
		{
			get { return base.NumberOfFilesParsed; }
		}

		public void CopyTo(ReceiveProgressItem progressItem)
		{
			progressItem.FromAETitle = this.FromAETitle;

			if (this.StudyInformation != null)
				progressItem.StudyInformation = this.StudyInformation.Clone();
			else
				progressItem.StudyInformation = null;

			base.CopyTo(progressItem);
		}

		public void CopyFrom(ReceiveProgressItem progressItem)
		{
			this.FromAETitle = progressItem.FromAETitle;

			if (progressItem.StudyInformation != null)
				this.StudyInformation = progressItem.StudyInformation.Clone();
			else
				this.StudyInformation = null;

			base.CopyFrom(progressItem);
		}

		public new ReceiveProgressItem Clone()
		{
			ReceiveProgressItem clone = new ReceiveProgressItem();
			CopyTo(clone);
			return clone;
		}
	}

	[DataContract]
	public class SendProgressItem : ExportProgressItem
	{
		private string _toAETitle;
		private StudyInformation _studyInformation;
		private SendOperationReference _sendOperationReference;

		public SendProgressItem()
		{
		}

		[DataMember(IsRequired = true)]
		public string ToAETitle
		{
			get { return _toAETitle; }
			set { _toAETitle = value; }
		}

		[DataMember(IsRequired = true)]
		public StudyInformation StudyInformation
		{
			get { return _studyInformation; }
			set { _studyInformation = value; }
		}

		[DataMember(IsRequired = false)]
		public SendOperationReference SendOperationReference
		{
			get { return _sendOperationReference; }
			set { _sendOperationReference = value; }
		}

		public void CopyTo(SendProgressItem progressItem)
		{
			progressItem.ToAETitle = this.ToAETitle;
			progressItem.SendOperationReference = SendOperationReference;

			if (this.StudyInformation != null)
				progressItem.StudyInformation = this.StudyInformation.Clone();
			else
				progressItem.StudyInformation = null;

			base.CopyTo(progressItem);
		}

		public void CopyFrom(SendProgressItem progressItem)
		{
			this.ToAETitle = progressItem.ToAETitle;
			SendOperationReference = progressItem.SendOperationReference;

			if (progressItem.StudyInformation != null)
				this.StudyInformation = progressItem.StudyInformation.Clone();
			else
				this.StudyInformation = null;

			base.CopyFrom(progressItem);
		}

		public new SendProgressItem Clone()
		{
			SendProgressItem clone = new SendProgressItem();
			CopyTo(clone);
			return clone;
		}
	}

	[DataContract]
	public class ReindexProgressItem : ImportProgressItem
	{
		public ReindexProgressItem()
		{
		}

		public new ReindexProgressItem Clone()
		{
			ReindexProgressItem clone = new ReindexProgressItem();
			CopyTo(clone);
			return clone;
		}
	}

	[DataContract]
	public class CancelProgressItemInformation
	{
		private CancellationFlags _cancellationFlags;
		private IEnumerable<Guid> _progressItemIdentifiers;

		public CancelProgressItemInformation()
		{
		}

		[DataMember(IsRequired = true)]
		public CancellationFlags CancellationFlags
		{
			get { return _cancellationFlags; }
			set { _cancellationFlags = value; }
		}

		[DataMember(IsRequired = true)]
		public IEnumerable<Guid> ProgressItemIdentifiers
		{
			get { return _progressItemIdentifiers; }
			set { _progressItemIdentifiers = value; }
		}
	}

	[DataContract]
	public class ImportedSopInstanceInformation
	{
		private string _studyInstanceUid;
		private string _seriesInstanceUid;
		private string _sopInstanceUid;
		private string _sopInstanceFileName;

		public ImportedSopInstanceInformation()
		{
		}

		[DataMember(IsRequired = true)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		[DataMember(IsRequired = true)]
		public string SeriesInstanceUid
		{
			get { return _seriesInstanceUid; }
			set { _seriesInstanceUid = value; }
		}
		
		[DataMember(IsRequired = true)]
		public string SopInstanceUid
		{
			get { return _sopInstanceUid; }
			set { _sopInstanceUid = value; }
		}

		[DataMember(IsRequired = false)]
		public string SopInstanceFileName
		{
			get { return _sopInstanceFileName; }
			set { _sopInstanceFileName = value; }
		}
	}

	public enum BadFileBehaviour
	{ 
		Ignore = 0,
		Move,
		Delete
	}

	public enum FileImportBehaviour
	{ 
		Move = 0,
		Copy
	}

	[DataContract]
	public class FileImportRequest
	{
		private bool _recursive;
		private BadFileBehaviour _badFileBehaviour;
		private IEnumerable<string> _fileExtensions;
		private IEnumerable<string> _filePaths;
		private FileImportBehaviour _fileImportBehaviour;
		private bool _isBackground;

		public FileImportRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public bool Recursive
		{
			get { return _recursive; }
			set { _recursive = value; }
		}

		[DataMember(IsRequired = true)]
		public BadFileBehaviour BadFileBehaviour
		{
			get { return _badFileBehaviour; }
			set { _badFileBehaviour = value; }
		}

		[DataMember(IsRequired = true)]
		public IEnumerable<string> FileExtensions
		{
			get { return _fileExtensions; }
			set { _fileExtensions = value; }
		}

		[DataMember(IsRequired = true)]
		public IEnumerable<string> FilePaths
		{
			get { return _filePaths; }
			set { _filePaths = value; }
		}

		[DataMember(IsRequired = true)]
		public FileImportBehaviour FileImportBehaviour
		{
			get { return _fileImportBehaviour; }
			set { _fileImportBehaviour = value; }
		}

		[DataMember(IsRequired = false)]
		public bool IsBackground
		{
			get { return _isBackground; }
			set { _isBackground = value; }
		}
	}

	[DataContract]
	public class DeletedInstanceInformation : InstanceInformation
	{
		private string _errorMessage;
		private long _totalFreedSpace;

		public DeletedInstanceInformation()
		{ 
		}

		public bool Failed
		{
			get { return _errorMessage != null; }
		}

		[DataMember(IsRequired = true)]
		public string ErrorMessage
		{
			get { return _errorMessage; }
			set { _errorMessage = value; }
		}

		[DataMember(IsRequired = true)]
		public long TotalFreedSpace
		{
			get { return _totalFreedSpace; }
			set { _totalFreedSpace = value; }
		}

		public void CopyTo(DeletedInstanceInformation other)
		{
			other.ErrorMessage = this.ErrorMessage;
			other.TotalFreedSpace = this.TotalFreedSpace;
			base.CopyTo(other);
		}

		public new DeletedInstanceInformation Clone()
		{
			DeletedInstanceInformation clone = new DeletedInstanceInformation();
			CopyTo(clone);
			return clone;
		}
	}

	public enum DeletePriority
	{ 
		Low = 0,
		High
	}

	[DataContract]
	public class DeleteInstancesRequest
	{
		private DeletePriority _deletePriority;
		private InstanceLevel _instanceLevel;
		private IEnumerable<string> _instanceUids;

		public DeleteInstancesRequest()
		{ 
		}

		[DataMember(IsRequired = true)]
		public DeletePriority DeletePriority
		{
			get { return _deletePriority; }
			set { _deletePriority = value; }
		}

		[DataMember(IsRequired = true)]
		public InstanceLevel InstanceLevel
		{
			get { return _instanceLevel; }
			set { _instanceLevel = value; }
		}

		[DataMember(IsRequired = true)]
		public IEnumerable<string> InstanceUids
		{
			get { return _instanceUids; }
			set { _instanceUids = value; }
		}
	}

	[DataContract]
	public class LocalDataStoreServiceConfiguration
	{
		private string _storageDirectory;
		private string _badFileDirectory;

		public LocalDataStoreServiceConfiguration()
		{
		}

		[DataMember(IsRequired = true)]
		public string StorageDirectory
		{
			get { return _storageDirectory; }
			set { _storageDirectory = value; }
		}

		[DataMember(IsRequired = true)]
		public string BadFileDirectory
		{
			get { return _badFileDirectory; }
			set { _badFileDirectory = value; }
		}
	}
}
