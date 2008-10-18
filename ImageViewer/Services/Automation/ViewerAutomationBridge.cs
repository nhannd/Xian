using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Services.Automation
{
	/// <summary>
	/// Default implementation of <see cref="IViewerAutomationBridge"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The bridge design pattern allows the public interface (<see cref="IViewerAutomationBridge"/>) and it's
	/// underlying implementation <see cref="IViewerAutomation"/> to vary independently.
	/// </para>
	/// <para>
	/// This is the default implementation of <see cref="IViewerAutomationBridge"/>, which uses both the
	/// <see cref="IStudyRootQuery"/> and <see cref="IViewerAutomation"/> interfaces to drive automation.
	/// The bridge pattern allows the <see cref="IViewerAutomation"/> (and in this case, also the <see cref="IStudyRootQuery"/>)
	/// implementor to be provided by the client through the constructors.
	/// </para>
	/// </remarks>
	public class ViewerAutomationBridge : IViewerAutomationBridge
	{
		private IViewerAutomation _viewerAutomationClient;
		private IStudyRootQueryBridge _studyRootQueryBridge;
		
		private OpenStudiesBehaviour _openStudiesBehaviour;
		private IComparer<StudyRootStudyIdentifier> _studyComparer;

		public ViewerAutomationBridge(IViewerAutomation viewerAutomationClient, IStudyRootQuery studyRootQueryClient)
			: this(viewerAutomationClient, new StudyRootQueryBridge(studyRootQueryClient))
		{
		}

		public ViewerAutomationBridge(IViewerAutomation viewerAutomationClient, IStudyRootQueryBridge studyRootQueryBridge)
		{
			Platform.CheckForNullReference(viewerAutomationClient, "viewerAutomationClient");
			Platform.CheckForNullReference(studyRootQueryBridge, "studyRootQueryBridge");

			_viewerAutomationClient = viewerAutomationClient;
			_studyRootQueryBridge = studyRootQueryBridge;
			_openStudiesBehaviour = OpenStudiesBehaviour.ActivateExistingViewer;

			_studyComparer = new StudyDateTimeComparer();
		}

		protected IViewerAutomation ViewerAutomationClient
		{
			get { return _viewerAutomationClient; }	
		}

		protected IStudyRootQueryBridge StudyRootQueryBridge
		{
			get { return _studyRootQueryBridge; }
		}

		public IComparer<StudyRootStudyIdentifier> StudyComparer
		{
			get { return _studyComparer; }
			set { _studyComparer = value; }
		}

		#region IViewerAutomationBridge Members

		public OpenStudiesBehaviour OpenStudiesBehaviour
		{
			get { return _openStudiesBehaviour; }
			set { _openStudiesBehaviour = value; }
		}

		public IList<Viewer> GetViewers()
		{
			return _viewerAutomationClient.GetActiveViewers().ActiveViewers;
		}

		public IList<Viewer> GetViewers(string primaryStudyInstanceUid)
		{
			Platform.CheckForEmptyString(primaryStudyInstanceUid, "primaryStudyInstanceUid");

			return CollectionUtils.Select(GetViewers(),
						delegate(Viewer viewer)
						{
							return viewer.PrimaryStudyInstanceUid == primaryStudyInstanceUid;
						});
		}

		public IList<Viewer> GetViewersByAccessionNumber(string accessionNumber)
		{
			IList<StudyRootStudyIdentifier> studies = _studyRootQueryBridge.QueryByAccessionNumber(accessionNumber);
			CheckAtLeastOneStudy(studies);
			return GetViewers(studies);
		}

		public IList<Viewer> GetViewersByPatientId(string patientId)
		{
			IList<StudyRootStudyIdentifier> studies = _studyRootQueryBridge.QueryByPatientId(patientId);
			CheckAtLeastOneStudy(studies);
			return GetViewers(studies);
		}

		public Viewer OpenStudy(string studyInstanceUid)
		{
			return OpenStudies(new string[] { studyInstanceUid });
		}

		public Viewer OpenStudies(IEnumerable<string> studyInstanceUids)
		{
			IList<StudyRootStudyIdentifier> studies = _studyRootQueryBridge.QueryByStudyInstanceUid(studyInstanceUids);
			return OpenStudies(studies);
		}

		public Viewer OpenStudiesByAccessionNumber(string accessionNumber)
		{
			return OpenStudiesByAccessionNumber(new string[] {accessionNumber });
		}

		public Viewer OpenStudiesByAccessionNumber(IEnumerable<string> accessionNumbers)
		{
			List<StudyRootStudyIdentifier> studies = new List<StudyRootStudyIdentifier>();

			foreach (string accessionNumber in accessionNumbers)
				studies.AddRange(_studyRootQueryBridge.QueryByAccessionNumber(accessionNumber));

			return OpenStudies(studies);
		}

		public Viewer OpenStudiesByPatientId(string patientId)
		{
			return OpenStudiesByPatientId(new string[] { patientId });
		}

		public Viewer OpenStudiesByPatientId(IEnumerable<string> patientIds)
		{
			List<StudyRootStudyIdentifier> studies = new List<StudyRootStudyIdentifier>();

			foreach (string patientId in patientIds)
				studies.AddRange(_studyRootQueryBridge.QueryByPatientId(patientId));

			return OpenStudies(studies);
		}

		public Viewer OpenStudies(List<OpenStudyInfo> studiesToOpen)
		{
			OpenStudiesRequest request = new OpenStudiesRequest();
			request.ActivateIfAlreadyOpen = (_openStudiesBehaviour == OpenStudiesBehaviour.ActivateExistingViewer);
			request.StudiesToOpen = studiesToOpen;

			return _viewerAutomationClient.OpenStudies(request).Viewer;
		}

		public void ActivateViewer(Viewer viewer)
		{
			ActivateViewerRequest request = new ActivateViewerRequest();
			request.Viewer = viewer;
			_viewerAutomationClient.ActivateViewer(request);
		}

		public void CloseViewer(Viewer viewer)
		{
			CloseViewerRequest request = new CloseViewerRequest();
			request.Viewer = viewer;
			_viewerAutomationClient.CloseViewer(request);
		}

		public IList<string> GetViewerAdditionalStudies(Viewer viewer)
		{
			GetViewerInfoRequest request = new GetViewerInfoRequest();
			return _viewerAutomationClient.GetViewerInfo(request).AdditionalStudyInstanceUids;
		}

		#endregion

		private Viewer OpenStudies(IList<StudyRootStudyIdentifier> studiesToOpen)
		{
			CheckAtLeastOneStudy(studiesToOpen);

			if (_studyComparer != null)
				studiesToOpen = CollectionUtils.Sort(studiesToOpen, _studyComparer.Compare);

			return OpenStudies(Convert(studiesToOpen));
		}

		private static void CheckAtLeastOneStudy(IList<StudyRootStudyIdentifier> studies)
		{
			if (studies.Count == 0)
				throw new QueryFailedException("No studies were found matching the given criteria.");
		}

		private IList<Viewer> GetViewers(IEnumerable<StudyRootStudyIdentifier> studies)
		{
			List<string> studyInstanceUids = CollectionUtils.Map<StudyRootStudyIdentifier, string>(studies,
				delegate(StudyRootStudyIdentifier identifier)
				{
					return identifier.StudyInstanceUid;
				});

			return CollectionUtils.Select(GetViewers(),
				delegate(Viewer viewer)
				{
					return studyInstanceUids.Contains(viewer.PrimaryStudyInstanceUid);
				});
		}

		private static List<OpenStudyInfo> Convert(IEnumerable<StudyRootStudyIdentifier> studyIdentifiers)
		{
			return CollectionUtils.Map<StudyRootStudyIdentifier, OpenStudyInfo>(studyIdentifiers,
															delegate(StudyRootStudyIdentifier studyIdentifier)
															{
																return new OpenStudyInfo(studyIdentifier);
															});
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_viewerAutomationClient != null && _viewerAutomationClient is IDisposable)
				{
					((IDisposable)_viewerAutomationClient).Dispose();
					_viewerAutomationClient = null;
				}

				if (_studyRootQueryBridge != null)
				{
					_studyRootQueryBridge.Dispose();
					_studyRootQueryBridge = null;
				}
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

	}
}
