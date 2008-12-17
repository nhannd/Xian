using System;
using System.Collections.Generic;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	/// <summary>
	/// Bridge interface for <see cref="IStudyRootQuery"/>.
	/// </summary>
	/// <remarks>
	/// The bridge design pattern allows the public interface (<see cref="IStudyRootQueryBridge"/>) and it's
	/// underlying implementation <see cref="IStudyRootQuery"/> to vary independently.
	/// </remarks>
	public interface IStudyRootQueryBridge : IStudyRootQuery, IDisposable
	{
		/// <summary>
		/// Comparer used to sort the results returned from <see cref="IStudyRootQuery.StudyQuery"/>.
		/// </summary>
		IComparer<StudyRootStudyIdentifier> StudyComparer { get; set; }
		/// <summary>
		/// Comparer used to sort the results returned from <see cref="IStudyRootQuery.SeriesQuery"/>.
		/// </summary>
		IComparer<SeriesIdentifier> SeriesComparer { get; set; }
		/// <summary>
		/// Comparer used to sort the results returned from <see cref="IStudyRootQuery.ImageQuery"/>.
		/// </summary>
		IComparer<ImageIdentifier> ImageComparer { get; set; }

		/// <summary>
		/// Performs a STUDY query for the given <b>exact</b> Accession Number.
		/// </summary>
		IList<StudyRootStudyIdentifier> QueryByAccessionNumber(string accessionNumber);

		/// <summary>
		/// Performs a STUDY query for the given <b>exact</b> Patient Id.
		/// </summary>
		IList<StudyRootStudyIdentifier> QueryByPatientId(string patientId);

		/// <summary>
		/// Performs a STUDY query for the given Study Instance Uid.
		/// </summary>
		IList<StudyRootStudyIdentifier> QueryByStudyInstanceUid(string studyInstanceUid);

		/// <summary>
		/// Performs a STUDY query for the given Study Instance Uids.
		/// </summary>
		IList<StudyRootStudyIdentifier> QueryByStudyInstanceUid(IEnumerable<string> studyInstanceUids);

		/// <summary>
		/// Performs a SERIES query for the given Study Instance Uid.
		/// </summary>
		IList<SeriesIdentifier> SeriesQuery(string studyInstanceUid);

		/// <summary>
		/// Performs an IMAGE query for the given Study and Series Instance Uid.
		/// </summary>
		IList<ImageIdentifier> ImageQuery(string studyInstanceUid, string seriesInstanceUid);

		/// <summary>
		/// Performs the appropriate query given the input <see cref="DicomAttributeCollection"/>, based
		/// on the value of the QueryRetrieveLevel attribute.
		/// </summary>
		IList<DicomAttributeCollection> Query(DicomAttributeCollection queryCriteria);
	}
}
