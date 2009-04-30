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

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Services.Automation
{
	/// <summary>
	/// The namespace for all the automation data and service contracts.
	/// </summary>
	public static class AutomationNamespace
	{
		/// <summary>
		/// The namespace for all the automation data and service contracts.
		/// </summary>
		public const string Value = "http://www.clearcanvas.ca/imageViewer/automation";
	}

	#region Viewer Automation

	/// <summary>
	/// Data contract for fault when there are no active viewers.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class NoActiveViewersFault
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public NoActiveViewersFault()
		{}
	}

	/// <summary>
	/// Data contract for fault when the supplied <see cref="Viewer"/> no longer exists.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class ViewerNotFoundFault
	{
		private string _failureDescription;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ViewerNotFoundFault()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ViewerNotFoundFault(string failureDescription)
		{
			_failureDescription = failureDescription;
		}

		/// <summary>
		/// Textual description of the failure.
		/// </summary>
		[DataMember(IsRequired = false)]
		public string FailureDescription
		{
			get { return _failureDescription; }
			set { _failureDescription = value; }
		}

	}

	/// <summary>
	/// Data contract for when a failure occurs opening the requested study (or studies).
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenStudiesFault
	{
		private string _failureDescription;

		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudiesFault()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudiesFault(string failureDescription)
		{
			_failureDescription = failureDescription;
		}

		/// <summary>
		/// Textual description of the failure.
		/// </summary>
		[DataMember(IsRequired = false)]
		public string FailureDescription
		{
			get { return _failureDescription; }
			set { _failureDescription = value; }
		}
	}

	/// <summary>
	/// Data contract representing a viewer component or workspace.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class Viewer
	{
		private Guid _identifier;
		private string _primaryStudyInstanceUid;

		/// <summary>
		/// Constructor.
		/// </summary>
		public Viewer(Guid identifier, string primaryStudyInstanceUid)
		{
			_identifier = identifier;
			_primaryStudyInstanceUid = primaryStudyInstanceUid;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public Viewer(Guid viewerId)
			: this(viewerId, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public Viewer()
		{
		}

		/// <summary>
		/// Gets or sets the unique identifier of this <see cref="Viewer"/>.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Guid Identifier
		{
			get { return _identifier; }
			set { _identifier = value; }
		}

		/// <summary>
		/// Gets or sets the study instance uid of the primary study, or study of interest.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string PrimaryStudyInstanceUid
		{
			get { return _primaryStudyInstanceUid; }
			set { _primaryStudyInstanceUid = value; }
		}

		public override bool Equals(object obj)
		{
			if (obj is Viewer)
				return (obj as Viewer).Identifier == this.Identifier;
			else 
				return false;
		}

		public override int GetHashCode()
		{
			return this.Identifier.GetHashCode();
		}

		public override string ToString()
		{
			return this.Identifier.ToString();
		}

		public static bool operator == (Viewer viewer1, Viewer viewer2)
		{
			return Object.Equals(viewer1, viewer2);
		}

		public static bool operator !=(Viewer viewer1, Viewer viewer2)
		{
			return !Object.Equals(viewer1, viewer2);
		}
	}

	/// <summary>
	/// Data contract for results returned from <see cref="IViewerAutomation.GetActiveViewers"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class GetActiveViewersResult
	{
		private List<Viewer> _activeViewers;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GetActiveViewersResult()
		{
			_activeViewers = new List<Viewer>();
		}

		/// <summary>
		/// The currently active <see cref="Viewer"/>s.
		/// </summary>
		[DataMember(IsRequired = true)]
		public List<Viewer> ActiveViewers
		{
			get { return _activeViewers; }
			set { _activeViewers = value; }
		}
	}

	/// <summary>
	/// Data contract for requests via <see cref="IViewerAutomation.GetViewerInfo"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class GetViewerInfoRequest
	{
		private Viewer _viewer;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GetViewerInfoRequest()
		{
		}

		/// <summary>
		/// Gets or sets the viewer whose info is to be retrieved.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Viewer Viewer
		{
			get { return _viewer; }
			set { _viewer = value; }
		}
	}

	/// <summary>
	/// Data contract for results returned from <see cref="IViewerAutomation.GetViewerInfo"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class GetViewerInfoResult
	{
		private List<string> _additionalStudyInstanceUids;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GetViewerInfoResult()
		{
			_additionalStudyInstanceUids = new List<string>();
		}

		/// <summary>
		/// Gets or sets the study instance uids contained within the <see cref="GetViewerInfoRequest.Viewer"/>,
		/// not including the <see cref="Viewer.PrimaryStudyInstanceUid"/>, or study of interest.
		/// </summary>
		[DataMember(IsRequired = true)]
		public List<string> AdditionalStudyInstanceUids
		{
			get { return _additionalStudyInstanceUids; }
			set { _additionalStudyInstanceUids = value; }
		}
	
		//TODO: later, could add layout information, visible display sets, etc.
	}

	/// <summary>
	/// Data contracts for results returned from <see cref="IViewerAutomation.OpenStudies"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenStudiesResult
	{
		private Viewer _viewer;

		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudiesResult(Viewer viewer)
		{
			_viewer = viewer;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudiesResult()
		{
		}

		/// <summary>
		/// Gets or sets the <see cref="Viewer"/> in which the studies were opened.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Viewer Viewer
		{
			get { return _viewer; }
			set { _viewer = value; }
		}
	}

	/// <summary>
	/// Data contract for defining studies to be opened via <see cref="IViewerAutomation.OpenStudies"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenStudyInfo
	{
		private string _studyInstanceUid;
		private string _sourceAETitle;

		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudyInfo(StudyIdentifier studyIdentifier)
			: this(studyIdentifier.StudyInstanceUid, studyIdentifier.RetrieveAeTitle)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudyInfo(string studyInstanceUid, string sourceAE)
		{
			_studyInstanceUid = studyInstanceUid;
			_sourceAETitle = sourceAE;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudyInfo(string studyInstanceUid)
			: this(studyInstanceUid, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudyInfo()
		{
		}

		/// <summary>
		/// The Study Instance Uid of the study to be opened.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		/// <summary>
		/// The AE Title where the study is known to reside.
		/// </summary>
		[DataMember(IsRequired = false)]
		public string SourceAETitle
		{
			get { return _sourceAETitle; }
			set { _sourceAETitle = value; }
		}
	}

	/// <summary>
	/// Data contract for open studies requests via <see cref="IViewerAutomation.OpenStudies"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenStudiesRequest
	{
		private List<OpenStudyInfo> _studiesToOpen;
		private bool? _activateIfAlreadyOpen;

		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudiesRequest()
		{
		}

		/// <summary>
		/// Gets or sets the list of studies to open; the first study in the list
		/// will be taken to be the primary study or study of interest (<see cref="Viewer.PrimaryStudyInstanceUid"/>).
		/// </summary>
		[DataMember(IsRequired = true)]
		public List<OpenStudyInfo> StudiesToOpen
		{
			get { return _studiesToOpen; }
			set { _studiesToOpen = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not to simply
		/// activate the viewer if the requested primary study is already
		/// the primary study in an existing <see cref="Viewer"/>.
		/// </summary>
		/// <remarks>
		/// When this value is false, a new <see cref="Viewer"/> will always be opened
		/// whether or not the primary study is already the primary study in other <see cref="Viewer"/>s.
		/// </remarks>
		[DataMember(IsRequired = false)]
		public bool? ActivateIfAlreadyOpen
		{
			get { return _activateIfAlreadyOpen; }
			set { _activateIfAlreadyOpen = value; }
		}

		//TODO: add study source(s), viewer layout, hanging protocols.
	}

	/// <summary>
	/// Data contract for a request to close a <see cref="Viewer"/> via <see cref="IViewerAutomation.CloseViewer"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class CloseViewerRequest
	{
		private Viewer _viewer;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CloseViewerRequest()
		{
		}

		/// <summary>
		/// Gets or sets the <see cref="Viewer"/> to be closed.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Viewer Viewer
		{
			get { return _viewer; }
			set { _viewer = value; }
		}
	}

	/// <summary>
	/// Data contract for a request to activate a <see cref="Viewer"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class ActivateViewerRequest
	{
		private Viewer _viewer;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ActivateViewerRequest()
		{
		}

		/// <summary>
		/// Gets or sets the <see cref="Viewer"/> to activate.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Viewer Viewer
		{
			get { return _viewer; }
			set { _viewer = value; }
		}
	}

	#endregion

	#region Dicom Explorer Automation

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class DicomExplorerNotFoundFault
	{
		public DicomExplorerNotFoundFault()
		{
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class ServerNotFoundFault
	{
		public ServerNotFoundFault()
		{
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class NoLocalStoreFault
	{
		public NoLocalStoreFault()
		{
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class DicomExplorerSearchCriteria
	{
		private string _patientId;
		private string _patientsName;
		private string _accessionNumber;
		private string _studyDescription;
		private DateTime? _studyDateFrom;
		private DateTime? _studyDateTo;
		private List<string> _modalities = new List<string>();

		public DicomExplorerSearchCriteria()
		{
		}

		[DataMember(IsRequired = true)]
		public DateTime? StudyDateFrom
		{
			get { return _studyDateFrom; }
			set { _studyDateFrom = value; }
		}

		[DataMember(IsRequired = true)]
		public DateTime? StudyDateTo
		{
			get { return _studyDateTo; }
			set { _studyDateTo = value; }
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
		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set { _accessionNumber = value; }
		}

		[DataMember(IsRequired = true)]
		public string StudyDescription
		{
			get { return _studyDescription; }
			set { _studyDescription = value; }
		}

		[DataMember(IsRequired = true)]
		public List<string> Modalities
		{
			get { return _modalities; }
			set { _modalities = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public abstract class SearchStudiesRequest
	{
		private DicomExplorerSearchCriteria _searchCriteria;

		public SearchStudiesRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public DicomExplorerSearchCriteria SearchCriteria
		{
			get { return _searchCriteria; }
			set { _searchCriteria = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class SearchLocalStudiesRequest : SearchStudiesRequest
	{
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class SearchLocalStudiesResult
	{
		public SearchLocalStudiesResult()
		{
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class SearchRemoteStudiesRequest : SearchStudiesRequest
	{
		private string _aeTitle;

		public SearchRemoteStudiesRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public string AETitle
		{
			get { return _aeTitle; }
			set { _aeTitle = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class SearchRemoteStudiesResult
	{
		public SearchRemoteStudiesResult()
		{
		}
	}

	#endregion
}
