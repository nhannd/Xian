using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NHibernate;
using NHibernate.Collection;
using NHibernate.Expression;
using Iesi.Collections;

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

        public ReadOnlyQueryResultCollection StudyQuery(QueryKey queryKey)
        {
            // TODO
            if (null == queryKey)
				throw new System.ArgumentNullException("queryKey", SR.ExceptionStudyQueryNullKey);

			if (queryKey.ContainsTag(DicomTag.ModalitiesInStudy))
			{
				// Dicom *does* support wildcards on this tag, but right now the local datastore
				// does not.  This is because the modalities are post-filtered in the code 
				// and I couldn't find a good wildcard RegEx pattern to use.  This should get
				// sorted out when C-FIND is implemented.
				if (ContainsWildCards(queryKey[DicomTag.ModalitiesInStudy]))
					throw new NotSupportedException(SR.ExceptionModalitiesInStudyWildcardsNotSupported);
			}

			//
            // prepare the HQL query string
            //
            StringBuilder selectCommandString = new StringBuilder(1024);
            selectCommandString.AppendFormat("FROM Study ");

			bool whereClauseAdded = false;

            foreach (DicomTag tag in queryKey.DicomTags)
            {
                if (queryKey[tag].Length > 0)
                {
                    Path path = new Path(tag.ToString());
                    DictionaryEntry column = DataAccessLayer.GetIDicomDictionary().GetColumn(path);

					if (column.IsComputed)
						continue;

					StringBuilder nextCriteria = new StringBuilder();
					AppendQuery(queryKey[tag], column, nextCriteria);

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
			List<string> modalitiesInStudyFilterValues = new List<string>();
			bool processModalitiesInStudy = queryKey.ContainsTag(DicomTag.ModalitiesInStudy);
			if (processModalitiesInStudy)
			{
				string modalitiesInStudyFilter = queryKey[DicomTag.ModalitiesInStudy].ToString();
				if (!String.IsNullOrEmpty(modalitiesInStudyFilter))
				{
					string[] splitValues = modalitiesInStudyFilter.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
					if (splitValues != null && splitValues.Length > 0)
						modalitiesInStudyFilterValues.AddRange(splitValues);
				}
			}

			// 
			// compile the query results
			//
			QueryResultList results = new QueryResultList();
			
			foreach (object element in studiesFound)
            {
                Study study = element as Study;
                QueryResult result = new QueryResult();

                foreach (PropertyInfo pi in study.GetType().GetProperties())
                {
                    string fieldName = pi.Name;
                    if (DataAccessLayer.GetIDicomDictionary().Contains(new TagName(fieldName)))
                    {
                        // ensure that property actually has a value
                        object fieldValue = pi.GetValue(study, null);
                        if (null == fieldValue)
                            continue;

                        DictionaryEntry col = DataAccessLayer.GetIDicomDictionary().GetColumn(new TagName(fieldName));
                        DicomTag tag = new DicomTag(col.Path.GetLastPathElementAsInt32());
                        result.Add(tag, fieldValue.ToString());
                    }
                }

                // special tags
                if (processModalitiesInStudy)
                {
                    Dictionary<string, string> set = new Dictionary<string, string>();
                    foreach (Series series in study.Series)
                    {
						if (String.IsNullOrEmpty(series.Modality))
							continue;

                        if (!set.ContainsKey(series.Modality))
                            set.Add(series.Modality, series.Modality);
                    }

					if (modalitiesInStudyFilterValues.Count > 0 && set.Count > 0)
					{
						//when filter values have been specified for the ModalitiesInStudyTag, if any of the filter values is a match
						//for any of the modalities contained in the study, then the study is returned.  Otherwise, the study is
						//filtered out, hence we continue right away, skipping the addition of the study to the results.
						if (!modalitiesInStudyFilterValues.Exists(delegate(string filterValue) { return set.ContainsKey(filterValue); }))
							continue;
					}
										
					string modalities = "";
                    foreach (string modality in set.Keys)
                    {
                        modalities += modality + @"\";
                    }

                    // get rid of trailing slash
					if (modalities != "")
						modalities = modalities.Remove(modalities.Length - 1);

                    result.Add(DicomTag.ModalitiesInStudy, modalities);
                }

                results.Add(result);
            }
            return new ReadOnlyQueryResultCollection(results); 
        }

		private void AppendQuery(string queryKeyString, DictionaryEntry column, StringBuilder returnBuilder)
		{ 
			string newQueryKeyString = StandardizeQueryKey(queryKeyString);

			if (column.ValueRepresentation == "DA")
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

		private void AppendDateQuery(string dateQueryString, string columnName, StringBuilder returnBuilder)
		{
			string fromDate, toDate;
			bool isRange;

			try
			{
				DateRangeParser.Parse(dateQueryString, out fromDate, out toDate, out isRange);
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
					dateRangeQueryBuilder.AppendFormat("( CONVERT(datetime, {0}) = CONVERT(datetime, '{1}') )", columnName + "_", fromDate.ToString());
				}
				else
				{
					dateRangeQueryBuilder.AppendFormat("( CONVERT(datetime, {0}) IS NOT NULL AND CONVERT(datetime, {0}) >= CONVERT(datetime, '{1}') )", columnName + "_", fromDate);
				}
			}

			if (toDate != "")
			{
				if (fromDate == "")
					dateRangeQueryBuilder.AppendFormat("( CONVERT(datetime, {0}) IS NULL OR ", columnName + "_");
				else
					dateRangeQueryBuilder.AppendFormat(" AND (");

				dateRangeQueryBuilder.AppendFormat("CONVERT(datetime, {0}) <= CONVERT(datetime, '{1}') )", columnName + "_", toDate);
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
