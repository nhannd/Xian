using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using NHibernate.Collection;
using Iesi.Collections;
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.DataStore
{
    internal sealed partial class SingleSessionDataStoreReader : IDataStoreReader
    {
        #region Handcoded Members
        private SingleSessionDataStoreReader()
        {
            
        }

        public SingleSessionDataStoreReader(ISession session) : this()
        {
            _session = session;
        }

        #region Private Members
        private ISession _session;
        private ISession Session
        {
            get { return _session; }
        }

        #endregion
        #endregion

        #region IDataStore Members

        public bool StudyExists(Uid referencedUid)
        {
            int count = 0;
            try
            {
                IEnumerable results = this.Session.Enumerable("select count(study) from Study study " +
                    "where study.StudyInstanceUid = ?",
                    referencedUid.ToString(),
                    NHibernateUtil.String);

                foreach (int number in results)
                {
                    count = number;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return count > 0;
        }

        public bool SeriesExists(Uid referencedUid)
        {
            int count = 0;
            try
            {
                IEnumerable results = this.Session.Enumerable("select count(series) from Series series " +
                    "where series.SeriesInstanceUid = ?",
                    referencedUid.ToString(),
                    NHibernateUtil.String);

                foreach (int number in results)
                {
                    count = number;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return count > 0;
        }

        public bool SopInstanceExists(Uid referencedUid)
        {
            int count = 0;
            try
            {
                IEnumerable results = this.Session.Enumerable("select count(sop) from SopInstance sop " +
                    "where sop.SopInstanceUid = ?",
                    referencedUid.ToString(),
                    NHibernateUtil.String);

                foreach (int number in results)
                {
                    count = number;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return count > 0;
        }

        public ISopInstance GetSopInstance(Uid referencedUid)
        {
            if (!SopInstanceExists(referencedUid))
                return null;

            ISopInstance sop = null;
            try
            {
               IList listOfSops = this.Session.CreateCriteria(typeof(SopInstance))
                    .Add(Expression.Eq("SopInstanceUid", referencedUid.ToString()))
                    .SetFetchMode("WindowValues_", FetchMode.Eager)
                    .List();

                if (null != listOfSops && listOfSops.Count > 0)
                    sop = listOfSops[0] as ISopInstance;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return sop;
        }

        public ISeries GetSeries(Uid referenceUid)
        {
            if (!SeriesExists(referenceUid))
                return null;

            ISeries series = null;
            try
            {
                IList listOfSeries = this.Session.CreateCriteria(typeof(Series))
                    .Add(Expression.Eq("SeriesInstanceUid", referenceUid.ToString()))
                    .SetFetchMode("SopInstances", FetchMode.Lazy)
                    .List();

                if (null != listOfSeries && listOfSeries.Count > 0)
                    series = listOfSeries[0] as ISeries;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return series;
        }

		public void InitializeAssociatedObject(object parentObject, object childObject)
		{
			// TODO: specific cases of check for whether the child object
			// has already been initialized, and skipping if it has. If 
			// we are using v1.2 of NHibernate or later, there should be
			// a WasInitialized property that can be checked
			PersistentCollection associatedCollection = childObject as PersistentCollection;
			if (null == associatedCollection || associatedCollection.WasInitialized)
				return;

			try
			{
				this.Session.Lock(parentObject, LockMode.Read);
				NHibernateUtil.Initialize(childObject);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

        public IStudy GetStudy(Uid referenceUid)
        {
            if (!StudyExists(referenceUid))
                return null;

            IStudy studyFound = null;
            try
            {
                IList listOfStudies = this.Session.CreateCriteria(typeof(Study))
                    .Add(Expression.Eq("StudyInstanceUid", referenceUid.ToString()))
                    .SetFetchMode("Series", FetchMode.Eager)
                    .List();

                if (null != listOfStudies && listOfStudies.Count > 0)
                {
                    (listOfStudies[0] as Study).InitializeAssociatedCollection += InitializeAssociatedObject;
                    foreach (Series series in (listOfStudies[0] as Study).Series)
                    {
                        series.InitializeAssociatedCollection += InitializeAssociatedObject;
                    }
                    studyFound = listOfStudies[0] as IStudy;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return studyFound;
        }

		public IList<IStudy> GetStudies()
		{
			IList<IStudy> studiesList = new List<IStudy>();

			//
			// prepare the HQL query string
			//
			StringBuilder selectCommandString = new StringBuilder(1024);
			selectCommandString.AppendFormat("FROM Study ORDER BY StoreTime_ ");

			//
			// submit the HQL query
			//
			IList studiesFound = null;
			ITransaction transaction = null;
			try
			{
				transaction = this.Session.BeginTransaction();

				IQuery query = this.Session.CreateQuery(selectCommandString.ToString());
				studiesFound = query.List();
			}
			catch (Exception ex)
			{
				if (null != transaction)
					transaction.Rollback();
				throw ex;
			}

			foreach (object element in studiesFound)
			{
				Study study = element as Study;
				studiesList.Add(study);
			}

			return studiesList;
		}

		public ReadOnlyQueryResultCollection StudyQuery(QueryKey queryKey)
		{
			if (null == queryKey)
				throw new System.ArgumentNullException("queryKey", SR.ExceptionStudyQueryNullKey);

			// prepare the HQL query string
			//
			StringBuilder selectCommandString = new StringBuilder(1024);
			selectCommandString.AppendFormat(" FROM Study as study ");

			bool whereClauseAdded = false;

			IDicomDictionary queryDictionary = DataAccessLayer.GetIDicomDictionary(DicomDictionary.DefaultQueryDictionaryName);

			foreach (DicomTag tag in queryKey.DicomTags)
			{
				if (queryKey[tag].Length > 0)
				{
					Path path = new Path(tag.ToString());
					DictionaryEntry column = queryDictionary.GetColumn(path);

					if (column.IsComputed)
						continue;

					StringBuilder nextCriteria = new StringBuilder();
					if (tag.Equals(DicomTag.ModalitiesInStudy))
					{
						//special case for modalities in study since it's not actually in the study table.
						AppendModalitiesInStudyQuery(queryKey[DicomTag.ModalitiesInStudy], nextCriteria);
					}
					else
					{
						AppendQuery(queryKey[tag], column, nextCriteria);
					}

					if (nextCriteria.Length == 0)
						continue;

					if (whereClauseAdded)
					{
						selectCommandString.AppendFormat(" AND ");
					}
					else
					{
						selectCommandString.AppendFormat("WHERE ");
						whereClauseAdded = true;
					}

					selectCommandString.Append(nextCriteria.ToString());
				}
			}

			//
			// submit the HQL query
			//
			IList studiesFound = null;
			ITransaction transaction = null;
			try
			{
				transaction = this.Session.BeginTransaction();

				IQuery query = this.Session.CreateQuery(selectCommandString.ToString());
				studiesFound = query.List();
			}
			catch (Exception ex)
			{
				if (null != transaction)
					transaction.Rollback();
				throw ex;
			}

			return CompileResults(queryKey, studiesFound);
		}

		private ReadOnlyQueryResultCollection CompileResults(QueryKey queryKey, IList studiesFound)
		{
			// 
			// compile the query results
			//
			QueryResultList results = new QueryResultList();

			foreach (object element in studiesFound)
			{
				Study study = element as Study;
				QueryResult result = new QueryResult();

				IDicomDictionary resultsDictionary = DataAccessLayer.GetIDicomDictionary(DicomDictionary.DefaultResultsDictionaryName);

				foreach (PropertyInfo pi in study.GetType().GetProperties())
				{
					string fieldName = pi.Name;
					if (resultsDictionary.Contains(new TagName(fieldName)))
					{
						// ensure that property actually has a value
						object fieldValue = pi.GetValue(study, null);
						if (null == fieldValue)
							continue;

						DictionaryEntry col = resultsDictionary.GetColumn(new TagName(fieldName));
						DicomTag tag = new DicomTag(col.Path.GetLastPathElementAsInt32());
						result.Add(tag, fieldValue.ToString());
					}
				}

				Dictionary<string, string> setModalities = new Dictionary<string, string>();
				foreach (Series series in study.Series)
					setModalities[series.Modality] = series.Modality;

				string modalities = VMStringConverter.ToDicomStringArray<string>(setModalities.Keys);
				result.Add(DicomTag.ModalitiesInStudy, modalities);

				results.Add(result);
			}

			return new ReadOnlyQueryResultCollection(results);
		}

		private void AppendQuery(string queryKeyString, DictionaryEntry column, StringBuilder returnBuilder)
		{
			string newQueryKeyString = StandardizeQueryKey(queryKeyString);

			if (column.ValueRepresentation == "UI")
			{
				AppendListOfUidQuery(newQueryKeyString, column.TagName.ToString(), returnBuilder);
			}
			else if (column.ValueRepresentation == "DA")
			{
				AppendDateQuery(newQueryKeyString, column.TagName.ToString(), returnBuilder);
			}
			else if (column.ValueRepresentation == "TM")
			{
				// Unsupported at this time.
				//AppendTimeQuery(...)
			}
			else if (column.ValueRepresentation == "DT")
			{
				// Unsupported at this time.
				//AppendDateTimeQuery(...)
			}
			else if (IsWildCardQuery(newQueryKeyString, column))
			{
				AppendWildCardQuery(newQueryKeyString, column.TagName.ToString(), returnBuilder);
			}
			else
			{
				AppendSingleValueQuery(newQueryKeyString, column.TagName.ToString(), returnBuilder);
			}
		}

		private void AppendModalitiesInStudyQuery(string modalitiesInStudyQuery, StringBuilder returnBuilder)
		{
			if (String.IsNullOrEmpty(modalitiesInStudyQuery))
				return;

			if (ContainsWildCards(modalitiesInStudyQuery))
			{
				returnBuilder.AppendFormat("exists(from study.InternalSeries as series where series.Modality like '{0}')", modalitiesInStudyQuery);
			}
			else
			{
				List<string> modalities = new List<string>(VMStringConverter.ToStringArray(modalitiesInStudyQuery));
				string modalitiesList = StringUtilities.Combine<string>(modalities, ", ", delegate(string value) { return String.Format("'{0}'", value); });
				returnBuilder.AppendFormat("exists(from study.InternalSeries as series where series.Modality in ( {0} ))", modalitiesList);
			}
		}

		private void AppendListOfUidQuery(string listOfUidQueryString, string columnName, StringBuilder returnBuilder)
		{
			if (String.IsNullOrEmpty(listOfUidQueryString))
				return;

			List<string> uids = new List<string>(VMStringConverter.ToStringArray(listOfUidQueryString));
			string uidList = StringUtilities.Combine<string>(uids, ", ", delegate(string value) { return String.Format("'{0}'", value); });
			returnBuilder.AppendFormat("{0} in ( {1} )", columnName + "_", uidList);
		}

		private void AppendDateQuery(string dateQueryString, string columnName, StringBuilder returnBuilder)
		{
			string fromDate, toDate;
			bool isRange;

			try
			{
				DateRangeHelper.Parse(dateQueryString, out fromDate, out toDate, out isRange);
			}
			catch (Exception e)
			{
				throw;
			}

			StringBuilder dateRangeQueryBuilder = new StringBuilder();

			if (fromDate != "")
			{
				//When a dicom date is specified with no '-', it is to be taken as an exact date.
				if (!isRange)
				{
					dateRangeQueryBuilder.AppendFormat("( {0} = '{1}' )", columnName + "_", fromDate.ToString());
				}
				else
				{
					dateRangeQueryBuilder.AppendFormat("( {0} IS NOT NULL AND {0} >= '{1}' )", columnName + "_", fromDate);
				}
			}

			if (toDate != "")
			{
				if (fromDate == "")
					dateRangeQueryBuilder.AppendFormat("( {0} IS NULL OR ", columnName + "_");
				else
					dateRangeQueryBuilder.AppendFormat(" AND (");

				dateRangeQueryBuilder.AppendFormat("{0} <= '{1}' )", columnName + "_", toDate);
			}

			returnBuilder.AppendFormat("({0})", dateRangeQueryBuilder.ToString());
		}

		private void AppendWildCardQuery(string wildCardQuery, string columnName, StringBuilder returnBuilder)
		{
			string newWildCardQuery = wildCardQuery.Replace("*", "%");

			// don't forget to append the trailing underscore for column names
			returnBuilder.AppendFormat("{0} LIKE '{1}'", columnName + "_", newWildCardQuery);
		}

		private void AppendSingleValueQuery(string exactQueryString, string columnName, StringBuilder returnBuilder)
		{
			returnBuilder.AppendFormat("{0} = '{1}'", columnName + "_", exactQueryString);
		}

		private bool IsWildCardQuery(string queryStringCandidate, DictionaryEntry column)
		{
			string[] excludeVRs = { "DA", "TM", "DT", "SL", "SS", "US", "UL", "FL", "FD", "OB", "OW", "UN", "AT", "DS", "IS", "AS", "UI" };
			foreach (string excludeVR in excludeVRs)
			{
				if (0 == String.Compare(excludeVR, column.ValueRepresentation, true))
					return false;
			}

			return ContainsWildCards(queryStringCandidate);
		}

		private bool ContainsWildCards(string queryString)
		{
			string wildcardChars = "?*";

			if (queryString.IndexOfAny(wildcardChars.ToCharArray()) < 0)
				return false;

			return true;
		}

		private string StandardizeQueryKey(string inputQueryKeyString)
		{
			return inputQueryKeyString.Replace("'", "''");
		}

		#endregion
	}
}
