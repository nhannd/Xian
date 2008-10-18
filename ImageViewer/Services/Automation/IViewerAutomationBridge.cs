using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Services.Automation
{
	public enum OpenStudiesBehaviour
	{
		ActivateExistingViewer = 0,
		AlwaysOpenNewViewer
	}

	public class QueryFailedException : Exception
	{
		public QueryFailedException(string message)
			: base(message)
		{
		}
	}

	public interface IViewerAutomationBridge : IDisposable
	{
		OpenStudiesBehaviour OpenStudiesBehaviour { get; set; }
		IComparer<StudyRootStudyIdentifier> StudyComparer { get; set; }

		IList<Viewer> GetViewers();
		IList<Viewer> GetViewers(string primaryStudyInstanceUid);
		IList<Viewer> GetViewersByAccessionNumber(string accessionNumber);
		IList<Viewer> GetViewersByPatientId(string patientId);

		Viewer OpenStudy(string studyInstanceUid);
		Viewer OpenStudies(IEnumerable<string> studyInstanceUids);
		Viewer OpenStudies(List<OpenStudyInfo> studiesToOpen);
				
		Viewer OpenStudiesByAccessionNumber(string accessionNumber);
		Viewer OpenStudiesByAccessionNumber(IEnumerable<string> accessionNumbers);

		Viewer OpenStudiesByPatientId(string patientId);
		Viewer OpenStudiesByPatientId(IEnumerable<string> patientIds);

		void ActivateViewer(Viewer viewer);
		void CloseViewer(Viewer viewer);

		IList<string> GetViewerAdditionalStudies(Viewer viewer);
	}
}