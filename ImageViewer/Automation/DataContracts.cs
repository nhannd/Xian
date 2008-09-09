using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Automation
{
	#region Faults

	[DataContract(Namespace = "http://www.clearcanvas.ca/imageViewer/automation/contracts")]
	public class NoActiveViewerSessionsFault
	{
		public NoActiveViewerSessionsFault()
		{}
	}

	[DataContract(Namespace = "http://www.clearcanvas.ca/imageViewer/automation/contracts")]
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

	[DataContract(Namespace = "http://www.clearcanvas.ca/imageViewer/automation/contracts")]
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

	#endregion

	#region Viewer Session

	[DataContract(Namespace = "http://www.clearcanvas.ca/imageViewer/automation/contracts")]
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

	[DataContract(Namespace = "http://www.clearcanvas.ca/imageViewer/automation/contracts")]
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

	#region Info

	[DataContract(Namespace = "http://www.clearcanvas.ca/imageViewer/automation/contracts")]
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

	[DataContract(Namespace = "http://www.clearcanvas.ca/imageViewer/automation/contracts")]
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

	#endregion
	#endregion

	#region Open Studies

	//Note: this class is not a data contract right now, but could be returned in the OpenStudiesResult later, if necessary.
	internal class OpenStudyResult
	{
		private string _studyInstanceUid;
		private int _numberOfImagesLoaded;
		private int _numberOfImagesFailed;

		public OpenStudyResult()
		{
		}

		public OpenStudyResult(string studyInstanceUid)
			: this(studyInstanceUid, 0, 0)
		{
		}

		public OpenStudyResult(string studyInstanceUid, int loaded, int failed)
		{
			_studyInstanceUid = studyInstanceUid;
			_numberOfImagesLoaded = loaded;
			_numberOfImagesFailed = failed;
		}

		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		public int NumberOfImagesFailed
		{
			get { return _numberOfImagesFailed; }
			set { _numberOfImagesFailed = value; }
		}

		public int NumberOfImagesLoaded
		{
			get { return _numberOfImagesLoaded; }
			set { _numberOfImagesLoaded = value; }
		}
	}

	[DataContract(Namespace = "http://www.clearcanvas.ca/imageViewer/automation/contracts")]
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

	[DataContract(Namespace = "http://www.clearcanvas.ca/imageViewer/automation/contracts")]
	public class OpenStudiesRequest
	{
		private List<string> _studyInstanceUids;
		private WindowBehaviour? _windowBehaviour;
		private bool? _activateIfAlreadyOpen;

		public OpenStudiesRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public List<string> StudyInstanceUids
		{
			get { return _studyInstanceUids; }
			set { _studyInstanceUids = value; }
		}

		[DataMember(IsRequired = false)]
		public WindowBehaviour? WindowBehaviour
		{
			get { return _windowBehaviour; }
			set { _windowBehaviour = value; }
		}

		[DataMember(IsRequired = false)]
		public bool? ActivateIfAlreadyOpen
		{
			get { return _activateIfAlreadyOpen; }
			set { _activateIfAlreadyOpen = value; }
		}

		//TODO: add study source(s), viewer layout, hanging protocols.
	}

	#endregion

	#region Close Viewer Session

	[DataContract(Namespace = "http://www.clearcanvas.ca/imageViewer/automation/contracts")]
	public class CloseViewerSessionRequest
	{
		private ViewerSession _viewerSession;

		CloseViewerSessionRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public ViewerSession ViewerSession
		{
			get { return _viewerSession; }
			set { _viewerSession = value; }
		}
	}

	#endregion

	#region Activate Viewer Session

	[DataContract(Namespace = "http://www.clearcanvas.ca/imageViewer/automation/contracts")]
	public class ActivateViewerSessionRequest
	{
		private ViewerSession _viewerSession;

		ActivateViewerSessionRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public ViewerSession ViewerSession
		{
			get { return _viewerSession; }
			set { _viewerSession = value; }
		}
	}

	#endregion
}

