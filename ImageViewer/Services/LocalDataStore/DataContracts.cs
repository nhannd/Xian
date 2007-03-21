using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Services.LocalDataStore
{
	[Flags]
	public enum CancellationFlags
	{
		None = 0,
		Cancel = 1,
		Remove = 2
	}

	public enum FileOperationProgressItemState
	{
		Pending = 0,
		InProgress,
		Completed,
		Paused,
		Cancelled,
		Removed,
		Error
	}

	[DataContract]
	public class SopInstanceInformation
	{
		private string _sopInstanceUid;
		private string _sopInstanceFileName;

		public SopInstanceInformation()
		{
		}

		[DataMember(IsRequired = true)]
		public string SopInstanceUid
		{
			get { return _sopInstanceUid; }
			set { _sopInstanceUid = value; }
		}

		[DataMember(IsRequired = true)]
		public string SopInstanceFileName
		{
			get { return _sopInstanceFileName; }
			set { _sopInstanceFileName = value; }
		}
	}

	[DataContract]
	public class StudyInformation
	{
		private string _studyInstanceUid;
		private string _patientId;
		private string _patientsName;
		private string _studyDescription;
		private DateTime _studyDate;

		public StudyInformation()
		{
		}

		[DataMember(IsRequired = true)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		[DataMember(IsRequired = true)]
		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}

		[DataMember(IsRequired = true)]
		public string PatientsName
		{
			get { return _patientsName; }
			set { _patientsName = value; }
		}

		[DataMember(IsRequired = true)]
		public string StudyDescription
		{
			get { return _studyDescription; }
			set { _studyDescription = value; }
		}

		[DataMember(IsRequired = true)]
		public DateTime StudyDate
		{
			get { return _studyDate; }
			set { _studyDate = value; }
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
	public class StoreScuBeginSendInformation
	{
		private string _toAETitle;
		private StudyInformation _studyInformation;
		private int _totalFilesToSend;

		public StoreScuBeginSendInformation()
		{
		}

		[DataMember(IsRequired = true)]
		public string ToAETitle
		{
			get { return _toAETitle; }
			set { _toAETitle = value; }
		}

		[DataMember(IsRequired = true)]
		public int TotalFilesToSend
		{
			get { return _totalFilesToSend; }
			set { _totalFilesToSend = value; }
		}
		
		[DataMember(IsRequired = true)]
		public StudyInformation StudyInformation
		{
			get { return _studyInformation; }
			set { _studyInformation = value; }
		}
	}
		
	[DataContract]
	public class StoreScuSentFileInformation
	{
		private string _toAETitle;
		private StudyInformation _studyInformation; 
		private SopInstanceInformation _sopInstanceInformation;
		
		public StoreScuSentFileInformation()
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
		public SopInstanceInformation SopInstanceInformation
		{
			get { return _sopInstanceInformation; }
			set { _sopInstanceInformation = value; }
		}
	}

	[DataContract]
	public abstract class FileOperationProgressItem
	{
		private Guid _identifier;
		private FileOperationProgressItemState _state;
		private CancellationFlags _allowedCancellationOperations;
		private string _statusMessage;
		private DateTime _startTime;
		private DateTime _lastActive;

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
		public FileOperationProgressItemState State
		{
			get { return _state; }
			set { _state = value; }
		}

		[DataMember(IsRequired = true)]
		public string StatusMessage
		{
			get { return _statusMessage; }
			set { _statusMessage = value; }
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
	}

	[DataContract]
	public class ImportProgressItem : FileOperationProgressItem
	{
		private int _totalFilesToImport;
		private int _numberOfFailedImports;
		private int _numberOfFilesImported;

		public ImportProgressItem()
		{
		}

		[DataMember(IsRequired = true)]
		public int TotalFilesToImport
		{
			get { return _totalFilesToImport; }
			set { _totalFilesToImport = value; }
		}

		[DataMember(IsRequired = true)]
		public int NumberOfFilesImported
		{
			get { return _numberOfFilesImported; }
			set { _numberOfFilesImported = value; }
		}

		[DataMember(IsRequired = true)]
		public int NumberOfFailedImports
		{
			get { return _numberOfFailedImports; }
			set { _numberOfFailedImports = value; }
		}
	}

	[DataContract]
	public class ExportProgressItem : FileOperationProgressItem
	{
		private int _totalFilesToExport;
		private int _numberOfFilesExported;

		public ExportProgressItem()
		{
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
	}

	[DataContract]
	public class SendProgressItem : ExportProgressItem
	{
		private string _toAETitle;
		private StudyInformation _studyInformation;

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
	}

	[DataContract]
	public class ReindexProgressItem : ImportProgressItem
	{
		public ReindexProgressItem()
		{
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
		private SopInstanceInformation _sopInstanceInformation;

		public ImportedSopInstanceInformation()
		{
		}

		[DataMember(IsRequired = false)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		[DataMember(IsRequired = false)]
		public string SeriesInstanceUid
		{
			get { return _seriesInstanceUid; }
			set { _seriesInstanceUid = value; }
		}
		
		[DataMember(IsRequired = true)]
		public SopInstanceInformation SopInstanceInformation
		{
			get { return _sopInstanceInformation; }
			set { _sopInstanceInformation = value; }
		}
	}

	public enum BadFileBehaviour
	{ 
		Ignore = 0,
		Move,
		Delete
	}

	[DataContract]
	public class FileImportRequest
	{
		private BadFileBehaviour _badFileBehaviour;
		private IEnumerable<string> _fileExtensions;
		private bool _recursive;
		private IEnumerable<string> _filePaths;

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
	}

	#region Fault Exceptions

	[DataContract]
	public class LocalDataStoreFaultException : Exception
	{
		public LocalDataStoreFaultException()
			: base()
		{
		}

		public LocalDataStoreFaultException(string message)
			: base(message)
		{
		}

		public LocalDataStoreFaultException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}

	#endregion
}
