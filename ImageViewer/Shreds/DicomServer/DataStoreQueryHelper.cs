using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.DataStore;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal class DataStoreQueryHelper
	{
		public static IStudy GetStudy(string studyInstanceUid)
		{
			List<string> studyUids = new List<string>();
			studyUids.Add(studyInstanceUid);

			foreach (IStudy study in GetStudies(studyUids))
				return study;

			//this can't actually happen because GetStudies will throw if the study isn't found
			return null;
		}

		public static IEnumerable<IStudy> GetStudies(IEnumerable<string> studyInstanceUids)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				foreach (string studyInstanceUid in studyInstanceUids)
				{
					IStudy study = reader.GetStudy(studyInstanceUid);
					if (study == null)
					{
						string message =
							String.Format("The specified study does not exist in the data store (uid = {0}).", studyInstanceUid);
						throw new ArgumentException(message);
					}

					yield return study;
				}
			}
		}

		public static IEnumerable<ISeries> GetSeries(string studyInstanceUid, IEnumerable<string> seriesInstanceUids)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				IStudy study = reader.GetStudy(studyInstanceUid);
				if (study == null)
				{
					string message = String.Format("The specified study does not exist in the data store (uid = {0}).", studyInstanceUid);
					throw new ArgumentException(message);
				}

				foreach (string seriesInstanceUid in seriesInstanceUids)
				{
					ISeries series = CollectionUtils.SelectFirst(study.GetSeries(),
										delegate(ISeries test) { return test.SeriesInstanceUid == seriesInstanceUid; });

					if (series == null)
					{
						string message = String.Format("The specified series does not exist in the data store (study = {0}, series = {1}).", studyInstanceUid, seriesInstanceUid);
						throw new ArgumentException(message);
					}

					yield return series;
				}
			}
		}

		public static IEnumerable<ISopInstance> GetStudySopInstances(IEnumerable<string> studyInstanceUids)
		{
			foreach (IStudy study in GetStudies(studyInstanceUids))
			{
				foreach (ISopInstance sop in study.GetSopInstances())
					yield return sop;
			}
		}

		public static IEnumerable<ISopInstance> GetSeriesSopInstances(string studyInstanceUid, IEnumerable<string> seriesInstanceUids)
		{
			foreach (ISeries series in GetSeries(studyInstanceUid, seriesInstanceUids))
			{
				foreach (ISopInstance sop in series.GetSopInstances())
					yield return sop;
			}
		}

		public static IEnumerable<ISopInstance> GetSopInstances(string studyInstanceUid, string seriesInstanceUid, IEnumerable<string> sopInstanceUids)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				IStudy study = reader.GetStudy(studyInstanceUid);
				if (study == null)
				{
					string message = String.Format("The specified study does not exist in the data store (uid = {0}).", studyInstanceUid);
					throw new ArgumentException(message);
				}

				ISeries series = CollectionUtils.SelectFirst(study.GetSeries(),
									delegate(ISeries test) { return test.SeriesInstanceUid == seriesInstanceUid; });

				if (series == null)
				{
					string message = String.Format("The specified series does not exist in the data store (study = {0}, series = {1}).", studyInstanceUid, seriesInstanceUid);
					throw new ArgumentException(message);
				}

				foreach (string sopInstanceUid in sopInstanceUids)
				{
					ISopInstance sop = CollectionUtils.SelectFirst(series.GetSopInstances(),
										delegate(ISopInstance test) { return test.SopInstanceUid == sopInstanceUid; });

					if (sop == null)
					{
						string message = String.Format("The specified sop instance does not exist in the data store (study = {0}, series = {1}, sop = {2}).", studyInstanceUid, seriesInstanceUid, sopInstanceUid);
						throw new ArgumentException(message);
					}

					yield return sop;
				}
			}
		}
	}
}
