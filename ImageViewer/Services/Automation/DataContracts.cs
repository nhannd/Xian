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
	public class NoActiveViewersFault
	{
		public NoActiveViewersFault()
		{}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class ViewerNotFoundFault
	{
		private string _failureDescription;

		public ViewerNotFoundFault()
		{
		}

		public ViewerNotFoundFault(string failureDescription)
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
	public class Viewer
	{
		private Guid _identifier;
		private string _primaryStudyInstanceUid;

		public Viewer(Guid identifier, string primaryStudyInstanceUid)
		{
			_identifier = identifier;
			_primaryStudyInstanceUid = primaryStudyInstanceUid;
		}

		public Viewer(Guid viewerId)
			: this(viewerId, null)
		{
		}

		public Viewer()
		{
		}

		[DataMember(IsRequired = true)]
		public Guid Identifier
		{
			get { return _identifier; }
			set { _identifier = value; }
		}

		[DataMember(IsRequired = true)]
		public string PrimaryStudyInstanceUid
		{
			get { return _primaryStudyInstanceUid; }
			set { _primaryStudyInstanceUid = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class GetActiveViewersResult
	{
		private List<Viewer> _activeViewers;

		public GetActiveViewersResult()
		{
			_activeViewers = new List<Viewer>();
		}

		[DataMember(IsRequired = true)]
		public List<Viewer> ActiveViewers
		{
			get { return _activeViewers; }
			set { _activeViewers = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class GetViewerInfoRequest
	{
		private Viewer _viewer;

		public GetViewerInfoRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public Viewer Viewer
		{
			get { return _viewer; }
			set { _viewer = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class GetViewerInfoResult
	{
		private List<string> _additionalStudyInstanceUids;

		public GetViewerInfoResult()
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
		private Viewer _viewer;

		public OpenStudiesResult(Viewer viewer)
		{
			_viewer = viewer;
		}

		public OpenStudiesResult()
		{
		}

		[DataMember(IsRequired = true)]
		public Viewer Viewer
		{
			get { return _viewer; }
			set { _viewer = value; }
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
	public class CloseViewerRequest
	{
		private Viewer _viewer;

		public CloseViewerRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public Viewer Viewer
		{
			get { return _viewer; }
			set { _viewer = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class ActivateViewerRequest
	{
		private Viewer _viewer;

		public ActivateViewerRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public Viewer Viewer
		{
			get { return _viewer; }
			set { _viewer = value; }
		}
	}
}
