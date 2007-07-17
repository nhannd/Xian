using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NHibernate;
using NHibernate.Collection;
using NHibernate.Expression;
using Iesi.Collections;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.DataStore
{
    public class DataStoreReader : IDataStoreReader
    {
        #region Handcoded Members
        public DataStoreReader(ISessionFactory sessionFactory)
        {
            this.SessionFactory = sessionFactory;
        }

        #region Private Members
        private DataStoreReader()
        {
        }

        private ISessionFactory _sessionFactory;

        private ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
            set { _sessionFactory = value; }
        }
	
        #endregion

        #endregion

        #region IDataStore Members

        public bool StudyExists(Uid referencedUid)
        {
            int count = 0;
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                IEnumerable results = session.Enumerable("select count(study) from Study study " +
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
                if (null!=transaction)
                    transaction.Rollback();
                throw ex;
            }
            finally
            {
                if (null != session)
                    session.Close();
            }

            return count > 0;
        }

        public bool SeriesExists(Uid referencedUid)
        {
            int count = 0;
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();
                IEnumerable results = session.Enumerable("select count(series) from Series series " +
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
                if (null!=transaction)
                    transaction.Rollback();
                throw ex;
            }
            finally
            {
                if (null != session)
                    session.Close();
            }

            return count > 0;
        }

        public bool SopInstanceExists(Uid referencedUid)
        {
            int count = 0;
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                IEnumerable results = session.Enumerable("select count(sop) from SopInstance sop " +
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
                if (null!=transaction)
                    transaction.Rollback();
                throw ex;
            }
            finally
            {
                if (null != session)
                    session.Close();
            }

            return count > 0;
        }

        public ISopInstance GetSopInstance(Uid referencedUid)
        {
            if (!SopInstanceExists(referencedUid))
                return null;

            ISopInstance sop = null;
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                IList listOfSops = session.CreateCriteria(typeof(SopInstance))
                     .Add(Expression.Eq("SopInstanceUid", referencedUid.ToString()))
                     .SetFetchMode("WindowValues_", FetchMode.Eager)
                     .List();

                if (null != listOfSops && listOfSops.Count > 0)
                    sop = listOfSops[0] as ISopInstance;
            }
            catch (Exception ex)
            {
                if (null!=transaction)
                    transaction.Rollback();
                throw ex;
            }
            finally
            {
                if (null != session)
                    session.Close();
            }

            return sop;
        }

        public ISeries GetSeries(Uid referenceUid)
        {
            if (!SeriesExists(referenceUid))
                return null;

            ISeries series = null;
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                IList listOfSeries = session.CreateCriteria(typeof(Series))
                    .Add(Expression.Eq("SeriesInstanceUid", referenceUid.ToString()))
                    .SetFetchMode("SopInstances", FetchMode.Lazy)
                    .List();

                if (null != listOfSeries && listOfSeries.Count > 0)
                {
                    (listOfSeries[0] as Series).InitializeAssociatedCollection += InitializeAssociatedObject;
                    series = listOfSeries[0] as ISeries;
                }
            }
            catch (Exception ex)
            {
                if (null!=transaction)
                    transaction.Rollback();
                throw ex;
            }
            finally
            {
                if (null != session)
                    session.Close();
            }

            return series;
        }

        public void InitializeAssociatedObject(object primaryObject, object associatedObject)
        {
            // TODO: specific cases of check for whether the child object
            // has already been initialized, and skipping if it has. If 
            // we are using v1.2 of NHibernate or later, there should be
            // a WasInitialized property that can be checked
            PersistentCollection associatedCollection = associatedObject as PersistentCollection;
            if (null == associatedCollection || associatedCollection.WasInitialized)
                return;

            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                session.Lock(primaryObject, LockMode.Read);
                NHibernateUtil.Initialize(associatedCollection);
            }
            catch (Exception ex)
            {
                if (null != transaction)
                    transaction.Rollback();
                throw ex;
            }
            finally
            {
                if (null != session)
                    session.Close();
            }
        }

        public IStudy GetStudy(Uid referenceUid)
        {
            if (!StudyExists(referenceUid))
                return null;

            IStudy studyFound = null;
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                IList listOfStudies = session.CreateCriteria(typeof(Study))
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
                if (null!=transaction)
                    transaction.Rollback();
                throw ex;
            }
            finally
            {
                if (null != session)
                    session.Close();
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
			ISession session = null;
			ITransaction transaction = null;
			try
			{
				session = this.SessionFactory.OpenSession();
				transaction = session.BeginTransaction();

				IQuery query = session.CreateQuery(selectCommandString.ToString());
				studiesFound = query.List();
			}
			catch (Exception ex)
			{
				if (null != transaction)
					transaction.Rollback();
				throw ex;
			}
			finally
			{
				if (null != session)
					session.Close();
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

			foreach (DicomTagPath path in queryKey.DicomTagPathCollection)
            {
                if (queryKey[path].Length > 0)
                {
					DictionaryEntry column = queryDictionary.GetColumn(path);

					if (column.IsComputed)
						continue;

					StringBuilder nextCriteria = new StringBuilder();
                    if (path.Equals(DicomTags.ModalitiesinStudy))
					{
						//special case for modalities in study since it's not actually in the study table.
						AppendModalitiesInStudyQuery(queryKey[DicomTags.ModalitiesinStudy], nextCriteria);
					}
					else
					{
						AppendQuery(queryKey[path], column, nextCriteria);
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
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

				IQuery query = session.CreateQuery(selectCommandString.ToString());
                studiesFound = query.List();
            }
            catch (Exception ex)
            {
                if (null!=transaction)
                    transaction.Rollback();
                throw ex;
            }
            finally
            {
                if (null != session)
                    session.Close();
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
						result.Add(col.Path, fieldValue.ToString());
					}
				}

				Dictionary<string, string> setModalities = new Dictionary<string, string>();
				foreach (Series series in study.Series)
					setModalities[series.Modality] = series.Modality;

				string modalities = VMStringConverter.ToDicomStringArray<string>(setModalities.Keys);
				result.Add(DicomTags.ModalitiesinStudy, modalities);

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
