using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Services.Automation
{
	/// <summary>
	/// Enum specifying how to treat the study request when the primary study is the
	/// primary study in an existing viewer.
	/// </summary>
	public enum OpenStudiesBehaviour
	{
		/// <summary>
		/// Specifies that the existing viewer should be activated (Default).
		/// </summary>
		ActivateExistingViewer = 0,
		/// <summary>
		/// Specifies that a new viewer should always be opened.
		/// </summary>
		AlwaysOpenNewViewer
	}

	/// <summary>
	/// Exception thrown when a query fails via <see cref="IStudyRootQuery"/>.
	/// </summary>
	public class QueryFailedException : Exception
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public QueryFailedException(string message)
			: base(message)
		{
		}
	}

	/// <summary>
	/// Interface for bridge to <see cref="IViewerAutomation"/>.
	/// </summary>
	/// <remarks>
	/// The bridge design pattern allows the public interface (<see cref="IViewerAutomationBridge"/>) and it's
	/// underlying implementation <see cref="IViewerAutomation"/> to vary independently.
	/// </remarks>
	public interface IViewerAutomationBridge : IDisposable
	{
		/// <summary>
		/// Specifies what the behaviour should be if the primary study is already the primary study in an existing viewer.
		/// </summary>
		OpenStudiesBehaviour OpenStudiesBehaviour { get; set; }

		/// <summary>
		/// Comparer used to sort results from <see cref="IStudyRootQuery.StudyQuery"/>.
		/// </summary>
		IComparer<StudyRootStudyIdentifier> StudyComparer { get; set; }

		/// <summary>
		/// Gets all the active <see cref="Viewer"/>s.
		/// </summary>
		/// <returns></returns>
		IList<Viewer> GetViewers();

		/// <summary>
		/// Gets all the active viewers having the given primary study instance uid.
		/// </summary>
		IList<Viewer> GetViewers(string primaryStudyInstanceUid);

		/// <summary>
		/// Gets all the active viewers where the primary study has the given accession number.
		/// </summary>
		IList<Viewer> GetViewersByAccessionNumber(string accessionNumber);

		/// <summary>
		/// Gets all the active viewers where the primary study has the given patient id.
		/// </summary>
		IList<Viewer> GetViewersByPatientId(string patientId);

		/// <summary>
		/// Opens the given study.
		/// </summary>
		Viewer OpenStudy(string studyInstanceUid);

		/// <summary>
		/// Opens the given studies.
		/// </summary>
		Viewer OpenStudies(IEnumerable<string> studyInstanceUids);

		/// <summary>
		/// Opens the given study.
		/// </summary>
		Viewer OpenStudies(List<OpenStudyInfo> studiesToOpen);
				
		/// <summary>
		/// Opens all studies matching the given <b>exact</b> accession number.
		/// </summary>
		Viewer OpenStudiesByAccessionNumber(string accessionNumber);

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> accession numbers.
		/// </summary>
		Viewer OpenStudiesByAccessionNumber(IEnumerable<string> accessionNumbers);

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> patient id.
		/// </summary>
		Viewer OpenStudiesByPatientId(string patientId);

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> patient ids.
		/// </summary>
		Viewer OpenStudiesByPatientId(IEnumerable<string> patientIds);

		/// <summary>
		/// Activates the given <see cref="Viewer"/>.
		/// </summary>
		void ActivateViewer(Viewer viewer);

		/// <summary>
		/// Closes the given <see cref="Viewer"/>.
		/// </summary>
		/// <param name="viewer"></param>
		void CloseViewer(Viewer viewer);

		/// <summary>
		/// Gets additional studies, not including the primary one, for the given <see cref="Viewer"/>.
		/// </summary>
		IList<string> GetViewerAdditionalStudies(Viewer viewer);
	}
}