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
using System.ServiceModel;
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

		/// <summary>
		/// Constructor.
		/// </summary>
		public ViewerAutomationBridge(IViewerAutomation viewerAutomationClient, IStudyRootQuery studyRootQueryClient)
			: this(viewerAutomationClient, new StudyRootQueryBridge(studyRootQueryClient))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ViewerAutomationBridge(IViewerAutomation viewerAutomationClient, IStudyRootQueryBridge studyRootQueryBridge)
		{
			Platform.CheckForNullReference(viewerAutomationClient, "viewerAutomationClient");
			Platform.CheckForNullReference(studyRootQueryBridge, "studyRootQueryBridge");

			_viewerAutomationClient = viewerAutomationClient;
			_studyRootQueryBridge = studyRootQueryBridge;
			_openStudiesBehaviour = OpenStudiesBehaviour.ActivateExistingViewer;

			_studyComparer = new StudyDateTimeComparer();
		}

		/// <summary>
		/// Gets the underlying <see cref="IViewerAutomation"/> client.
		/// </summary>
		protected IViewerAutomation ViewerAutomationClient
		{
			get { return _viewerAutomationClient; }	
		}

		/// <summary>
		/// Gets the underlying <see cref="IStudyRootQueryBridge"/>.
		/// </summary>
		protected IStudyRootQueryBridge StudyRootQueryBridge
		{
			get { return _studyRootQueryBridge; }
		}

		/// <summary>
		/// Comparer used to sort results from <see cref="IStudyRootQuery.StudyQuery"/>.
		/// </summary>
		public IComparer<StudyRootStudyIdentifier> StudyComparer
		{
			get { return _studyComparer; }
			set { _studyComparer = value; }
		}

		#region IViewerAutomationBridge Members

		/// <summary>
		/// Specifies what the behaviour should be if the primary study is already the primary study in an existing viewer.
		/// </summary>
		public OpenStudiesBehaviour OpenStudiesBehaviour
		{
			get { return _openStudiesBehaviour; }
			set { _openStudiesBehaviour = value; }
		}

		/// <summary>
		/// Gets all the active <see cref="Viewer"/>s.
		/// </summary>
		/// <returns></returns>
		public IList<Viewer> GetViewers()
		{
			return _viewerAutomationClient.GetActiveViewers().ActiveViewers;
		}

		/// <summary>
		/// Gets all the active viewers having the given primary study instance uid.
		/// </summary>
		public IList<Viewer> GetViewers(string primaryStudyInstanceUid)
		{
			Platform.CheckForEmptyString(primaryStudyInstanceUid, "primaryStudyInstanceUid");

			return CollectionUtils.Select(GetViewers(),
						delegate(Viewer viewer)
						{
							return viewer.PrimaryStudyInstanceUid == primaryStudyInstanceUid;
						});
		}

		/// <summary>
		/// Gets all the active viewers where the primary study has the given accession number.
		/// </summary>
		public IList<Viewer> GetViewersByAccessionNumber(string accessionNumber)
		{
			IList<StudyRootStudyIdentifier> studies = _studyRootQueryBridge.QueryByAccessionNumber(accessionNumber);
			CheckAtLeastOneStudy(studies);
			return GetViewers(studies);
		}

		/// <summary>
		/// Gets all the active viewers where the primary study has the given patient id.
		/// </summary>
		public IList<Viewer> GetViewersByPatientId(string patientId)
		{
			IList<StudyRootStudyIdentifier> studies = _studyRootQueryBridge.QueryByPatientId(patientId);
			CheckAtLeastOneStudy(studies);
			return GetViewers(studies);
		}

		/// <summary>
		/// Opens the given study.
		/// </summary>
		public Viewer OpenStudy(string studyInstanceUid)
		{
			return OpenStudies(new string[] { studyInstanceUid });
		}

		/// <summary>
		/// Opens the given studies.
		/// </summary>
		public Viewer OpenStudies(IEnumerable<string> studyInstanceUids)
		{
			IList<StudyRootStudyIdentifier> studies = _studyRootQueryBridge.QueryByStudyInstanceUid(studyInstanceUids);
			return OpenStudies(studies);
		}

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> accession number.
		/// </summary>
		public Viewer OpenStudiesByAccessionNumber(string accessionNumber)
		{
			return OpenStudiesByAccessionNumber(new string[] {accessionNumber });
		}

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> accession numbers.
		/// </summary>
		public Viewer OpenStudiesByAccessionNumber(IEnumerable<string> accessionNumbers)
		{
			List<StudyRootStudyIdentifier> studies = new List<StudyRootStudyIdentifier>();

			foreach (string accessionNumber in accessionNumbers)
				studies.AddRange(_studyRootQueryBridge.QueryByAccessionNumber(accessionNumber));

			return OpenStudies(studies);
		}

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> patient id.
		/// </summary>
		public Viewer OpenStudiesByPatientId(string patientId)
		{
			return OpenStudiesByPatientId(new string[] { patientId });
		}

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> patient ids.
		/// </summary>
		public Viewer OpenStudiesByPatientId(IEnumerable<string> patientIds)
		{
			List<StudyRootStudyIdentifier> studies = new List<StudyRootStudyIdentifier>();

			foreach (string patientId in patientIds)
				studies.AddRange(_studyRootQueryBridge.QueryByPatientId(patientId));

			return OpenStudies(studies);
		}

		/// <summary>
		/// Opens the given study.
		/// </summary>
		public Viewer OpenStudies(List<OpenStudyInfo> studiesToOpen)
		{
			OpenStudiesRequest request = new OpenStudiesRequest();
			request.ActivateIfAlreadyOpen = (_openStudiesBehaviour == OpenStudiesBehaviour.ActivateExistingViewer);
			request.StudiesToOpen = studiesToOpen;

			return _viewerAutomationClient.OpenStudies(request).Viewer;
		}

		/// <summary>
		/// Activates the given <see cref="Viewer"/>.
		/// </summary>
		public void ActivateViewer(Viewer viewer)
		{
			ActivateViewerRequest request = new ActivateViewerRequest();
			request.Viewer = viewer;
			_viewerAutomationClient.ActivateViewer(request);
		}

		/// <summary>
		/// Closes the given <see cref="Viewer"/>.
		/// </summary>
		/// <param name="viewer"></param>
		public void CloseViewer(Viewer viewer)
		{
			CloseViewerRequest request = new CloseViewerRequest();
			request.Viewer = viewer;
			_viewerAutomationClient.CloseViewer(request);
		}

		/// <summary>
		/// Gets additional studies, not including the primary one, for the given <see cref="Viewer"/>.
		/// </summary>
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

		/// <summary>
		/// Implementation of the Dispose pattern.
		/// </summary>
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

		/// <summary>
		/// Disposes this instance.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (CommunicationException)
			{
				//connection already closed.
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e);
			}
		}

		#endregion

	}
}
