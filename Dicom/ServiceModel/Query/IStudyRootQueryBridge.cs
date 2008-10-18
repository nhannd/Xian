using System;
using System.Collections.Generic;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	// Later, can add more complex things like methods that accept criteria objects to
	// define the query criteria.
	public interface IStudyRootQueryBridge : IStudyRootQuery, IDisposable
	{
		IComparer<StudyRootStudyIdentifier> StudyComparer { get; set; }
		IComparer<SeriesIdentifier> SeriesComparer { get; set; }
		IComparer<ImageIdentifier> ImageComparer { get; set; }

		IList<StudyRootStudyIdentifier> QueryByAccessionNumber(string accessionNumber);
		IList<StudyRootStudyIdentifier> QueryByPatientId(string patientId);
		IList<StudyRootStudyIdentifier> QueryByStudyInstanceUid(string studyInstanceUid);
		IList<StudyRootStudyIdentifier> QueryByStudyInstanceUid(IEnumerable<string> studyInstanceUids);

		IList<SeriesIdentifier> SeriesQuery(string studyInstanceUid);
		IList<ImageIdentifier> ImageQuery(string studyInstanceUid, string seriesInstanceUid);

		IList<DicomAttributeCollection> Query(DicomAttributeCollection queryCriteria);
	}
}
