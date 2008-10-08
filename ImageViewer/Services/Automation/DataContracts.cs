using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Services.Automation
{
	public static class AutomationNamespace
	{
		public const string Value = "http://www.clearcanvas.ca/imageViewer/automation";
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class NoActiveViewerSessionsFault
	{
		public NoActiveViewerSessionsFault()
		{}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class ViewerSessionNotFoundFault
	{
		private string _failureDescription;

		public ViewerSessionNotFoundFault()
		{
		}

		public ViewerSessionNotFoundFault(string failureDescription)
		{
			FailureDescription = failureDescription;
		}

		[DataMember(IsRequired = false)]
		public string FailureDescription
		{
			get { return _failureDescription; }
			set { _failureDescription = value; }
		}

	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenStudiesFault
	{
		private string _failureDescription;

		public OpenStudiesFault()
		{
		}

		public OpenStudiesFault(string failureDescription)
		{
			_failureDescription = failureDescription;
		}

		[DataMember(IsRequired = false)]
		public string FailureDescription
		{
			get { return _failureDescription; }
			set { _failureDescription = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class ViewerSession
	{
		private Guid _sessionId;
		private string _primaryStudyInstanceUid;

		public ViewerSession(Guid sessionId, string primaryStudyInstanceUid)
		{
			_sessionId = sessionId;
			_primaryStudyInstanceUid = primaryStudyInstanceUid;
		}

		public ViewerSession(Guid sessionId)
			: this(sessionId, null)
		{
		}

		public ViewerSession()
		{
		}

		[DataMember(IsRequired = true)]
		public Guid SessionId
		{
			get { return _sessionId; }
			set { _sessionId = value; }
		}

		[DataMember(IsRequired = true)]
		public string PrimaryStudyInstanceUid
		{
			get { return _primaryStudyInstanceUid; }
			set { _primaryStudyInstanceUid = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class GetActiveViewerSessionsResult
	{
		private List<ViewerSession> _activeViewerSessions;

		public GetActiveViewerSessionsResult()
		{
			_activeViewerSessions = new List<ViewerSession>();
		}

		[DataMember(IsRequired = true)]
		public List<ViewerSession> ActiveViewerSessions
		{
			get { return _activeViewerSessions; }
			set { _activeViewerSessions = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class GetViewerSessionInfoRequest
	{
		private ViewerSession _viewerSession;

		public GetViewerSessionInfoRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public ViewerSession ViewerSession
		{
			get { return _viewerSession; }
			set { _viewerSession = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class GetViewerSessionInfoResult
	{
		private List<string> _additionalStudyInstanceUids;

		public GetViewerSessionInfoResult()
		{
			_additionalStudyInstanceUids = new List<string>();
		}

		[DataMember(IsRequired = true)]
		public List<string> AdditionalStudyInstanceUids
		{
			get { return _additionalStudyInstanceUids; }
			set { _additionalStudyInstanceUids = value; }
		}
	
		//TODO: later, could add layout information, visible display sets, etc.
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenStudiesResult
	{
		private ViewerSession _viewerSession;

		public OpenStudiesResult(ViewerSession viewerSession)
		{
			_viewerSession = viewerSession;
		}

		public OpenStudiesResult()
		{
		}

		[DataMember(IsRequired = true)]
		public ViewerSession ViewerSession
		{
			get { return _viewerSession; }
			set { _viewerSession = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenStudyInfo
	{
		private string _studyInstanceUid;
		private string _sourceAETitle;

		public OpenStudyInfo(StudyIdentifier studyIdentifier)
			: this(studyIdentifier.StudyInstanceUid, studyIdentifier.RetrieveAeTitle)
		{
		}

		public OpenStudyInfo(string studyInstanceUid, string sourceAE)
		{
			_studyInstanceUid = studyInstanceUid;
			_sourceAETitle = sourceAE;
		}

		[DataMember(IsRequired = true)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		[DataMember(IsRequired = false)]
		public string SourceAETitle
		{
			get { return _sourceAETitle; }
			set { _sourceAETitle = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenStudiesRequest
	{
		private List<OpenStudyInfo> _studiesToOpen;
		private bool? _activateIfAlreadyOpen;

		public OpenStudiesRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public List<OpenStudyInfo> StudiesToOpen
		{
			get { return _studiesToOpen; }
			set { _studiesToOpen = value; }
		}

		[DataMember(IsRequired = false)]
		public bool? ActivateIfAlreadyOpen
		{
			get { return _activateIfAlreadyOpen; }
			set { _activateIfAlreadyOpen = value; }
		}

		//TODO: add study source(s), viewer layout, hanging protocols.
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class CloseViewerSessionRequest
	{
		private ViewerSession _viewerSession;

		public CloseViewerSessionRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public ViewerSession ViewerSession
		{
			get { return _viewerSession; }
			set { _viewerSession = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class ActivateViewerSessionRequest
	{
		private ViewerSession _viewerSession;

		public ActivateViewerSessionRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public ViewerSession ViewerSession
		{
			get { return _viewerSession; }
			set { _viewerSession = value; }
		}
	}
}