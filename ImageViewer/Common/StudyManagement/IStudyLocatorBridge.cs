#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
	public interface IStudyLocatorBridge : IStudyLocator, IDisposable
	{
		/// <summary>
		/// Comparer used to sort the results returned from <see cref="IStudyLocator.LocateStudies"/>.
		/// </summary>
		IComparer<StudyRootStudyIdentifier> StudyComparer { get; set; }

		/// <summary>
		/// Comparer used to sort the results returned from <see cref="IStudyLocator.LocateSeries"/>.
		/// </summary>
		IComparer<SeriesIdentifier> SeriesComparer { get; set; }

		/// <summary>
		/// Comparer used to sort the results returned from <see cref="IStudyLocator.LocateImages"/>.
		/// </summary>
		IComparer<ImageIdentifier> ImageComparer { get; set; }

		IList<StudyRootStudyIdentifier> LocateStudyByAccessionNumber(string accessionNumber);
		IList<StudyRootStudyIdentifier> LocateStudyByAccessionNumber(string accessionNumber, out LocateFailureInfo[] failures);
		IList<StudyRootStudyIdentifier> LocateStudyByPatientId(string patientId);
		IList<StudyRootStudyIdentifier> LocateStudyByPatientId(string patientId, out LocateFailureInfo[] failures);
		IList<StudyRootStudyIdentifier> LocateStudyByInstanceUid(string studyInstanceUid);
		IList<StudyRootStudyIdentifier> LocateStudyByInstanceUid(string studyInstanceUid, out LocateFailureInfo[] failures);
		IList<StudyRootStudyIdentifier> LocateStudyByInstanceUid(IEnumerable<string> studyInstanceUids);
		IList<StudyRootStudyIdentifier> LocateStudyByInstanceUid(IEnumerable<string> studyInstanceUids, out LocateFailureInfo[] failures);
		IList<SeriesIdentifier> LocateSeriesByStudy(string studyInstanceUid);
		IList<SeriesIdentifier> LocateSeriesByStudy(string studyInstanceUid, out LocateFailureInfo[] failures);
		IList<ImageIdentifier> LocateImagesBySeries(string studyInstanceUid, string seriesInstanceUid);
		IList<ImageIdentifier> LocateImagesBySeries(string studyInstanceUid, string seriesInstanceUid, out LocateFailureInfo[] failures);
	}
}