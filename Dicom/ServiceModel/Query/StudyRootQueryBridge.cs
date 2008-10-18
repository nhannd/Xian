using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	/// <summary>
	/// Default implementation of <see cref="IStudyRootQueryBridge"/>.
	/// </summary>
	public class StudyRootQueryBridge : IStudyRootQueryBridge
	{
		private IStudyRootQuery _client;

		private IComparer<StudyRootStudyIdentifier> _studyComparer;
		private IComparer<SeriesIdentifier> _seriesComparer;
		private IComparer<ImageIdentifier> _imageComparer;

		public StudyRootQueryBridge(IStudyRootQuery client)
		{
			Platform.CheckForNullReference(client, "client");
			_client = client;
			_studyComparer = new StudyDateTimeComparer();
		}

		public IComparer<StudyRootStudyIdentifier> StudyComparer
		{
			get { return _studyComparer; }
			set { _studyComparer = value; }
		}

		public IComparer<SeriesIdentifier> SeriesComparer
		{
			get { return _seriesComparer; }
			set { _seriesComparer = value; }
		}

		public IComparer<ImageIdentifier> ImageComparer
		{
			get { return _imageComparer; }
			set { _imageComparer = value; }
		}

		public IList<StudyRootStudyIdentifier> QueryByAccessionNumber(string accessionNumber)
		{
			Platform.CheckForEmptyString(accessionNumber, "accessionNumber");
			if (accessionNumber.Contains("*") || accessionNumber.Contains("?"))
				throw new ArgumentException("Accession Number cannot contain wildcard characters.");

			StudyRootStudyIdentifier criteria = new StudyRootStudyIdentifier();
			criteria.AccessionNumber = accessionNumber;
			return StudyQuery(criteria);
		}

		public IList<StudyRootStudyIdentifier> QueryByPatientId(string patientId)
		{
			Platform.CheckForEmptyString(patientId, "patientId");
			if (patientId.Contains("*") || patientId.Contains("?"))
				throw new ArgumentException("Patient Id cannot contain wildcard characters.");

			StudyRootStudyIdentifier criteria = new StudyRootStudyIdentifier();
			criteria.PatientId = patientId;
			return StudyQuery(criteria);
		}

		public IList<StudyRootStudyIdentifier> QueryByStudyInstanceUid(string studyInstanceUid)
		{
			return QueryByStudyInstanceUid(new string[] {studyInstanceUid});
		}

		public IList<StudyRootStudyIdentifier> QueryByStudyInstanceUid(IEnumerable<string> studyInstanceUids)
		{
			foreach (string studyInstanceUid in studyInstanceUids)
			{
				Platform.CheckForEmptyString(studyInstanceUid, "studyInstanceUid");

				if (studyInstanceUid.Contains("*") || studyInstanceUid.Contains("?"))
					throw new ArgumentException("Study Instance Uid cannot contain wildcard characters.");
			}

			StudyRootStudyIdentifier criteria = new StudyRootStudyIdentifier();
			criteria.StudyInstanceUid = DicomStringHelper.GetDicomStringArray(studyInstanceUids);
			return StudyQuery(criteria);
		}

		public IList<SeriesIdentifier> SeriesQuery(string studyInstanceUid)
		{
			Platform.CheckForEmptyString(studyInstanceUid, "studyInstanceUid");

			SeriesIdentifier criteria = new SeriesIdentifier();
			criteria.StudyInstanceUid = studyInstanceUid;
			return SeriesQuery(studyInstanceUid);
		}

		public IList<ImageIdentifier> ImageQuery(string studyInstanceUid, string seriesInstanceUid)
		{
			Platform.CheckForEmptyString(studyInstanceUid, "studyInstanceUid");
			Platform.CheckForEmptyString(seriesInstanceUid, "seriesInstanceUid");

			ImageIdentifier criteria = new ImageIdentifier();
			criteria.StudyInstanceUid = studyInstanceUid;
			criteria.SeriesInstanceUid = seriesInstanceUid;
			return ImageQuery(criteria);
		}

		#region IStudyRootQuery Members

		public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
		{
			IList<StudyRootStudyIdentifier> results = _client.StudyQuery(queryCriteria);
			if (_studyComparer != null)
				results = CollectionUtils.Sort(results, _studyComparer.Compare);

			return results;
		}

		public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
		{
			IList<SeriesIdentifier> results = _client.SeriesQuery(queryCriteria);
			if (_seriesComparer != null)
				results = CollectionUtils.Sort(results, _seriesComparer.Compare);

			return results;
		}

		public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
		{
			IList<ImageIdentifier> results = _client.ImageQuery(queryCriteria);
			if (_imageComparer != null)
				results = CollectionUtils.Sort(results, _imageComparer.Compare);

			return results;
		}

		#endregion

		public IList<DicomAttributeCollection> Query(DicomAttributeCollection queryCriteria)
		{
			Platform.CheckForNullReference(queryCriteria, "queryCriteria");

			string level = queryCriteria[DicomTags.QueryRetrieveLevel];
			Platform.CheckForEmptyString(level, "level");

			if (level == "STUDY")
				return Convert(_client.StudyQuery(new StudyRootStudyIdentifier(queryCriteria)));
			else if (level == "SERIES")
				return Convert(_client.SeriesQuery(new SeriesIdentifier(queryCriteria)));
			else if (level == "IMAGE")
				return Convert(_client.ImageQuery(new ImageIdentifier(queryCriteria)));

			throw new ArgumentException(String.Format("Query/Retrieve level '{0}' is not supported.", level));
		}

		private static IList<DicomAttributeCollection> Convert<T>(IList<T> identifiers) where T : Identifier, new()
		{
			return CollectionUtils.Map<T, DicomAttributeCollection>(identifiers,
				delegate(T id) { return id.ToDicomAttributeCollection(); });
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_client != null && _client is IDisposable)
				{
					((IDisposable)_client).Dispose();
					_client = null;
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
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
	}
}
