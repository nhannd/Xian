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
                    series = listOfSeries[0] as ISeries;
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
            if (null != associatedCollection)
            {
                if (associatedCollection.WasInitialized)
                    return;
            }

            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                session.Lock(primaryObject, LockMode.Read);
                NHibernateUtil.Initialize(associatedObject);
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
                    studyFound = listOfStudies[0] as IStudy;
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
                throw new System.ArgumentNullException("Bad null arguments");

            //
            // prepare the HQL query string
            //
            StringBuilder selectCommandString = new StringBuilder(1024);
            selectCommandString.AppendFormat("FROM Study ");

            int index = 1;
            foreach (DicomTag tag in queryKey.DicomTags)
            {
                Path path = new Path(tag.ToString());
                DictionaryEntry column = SingleSessionDataAccessLayer.GetIDicomDictionary().GetColumn(path);

                if (queryKey[tag].Length > 0)
                {
                    if (index > 1)
                        selectCommandString.AppendFormat(" AND ");
                    else
                        selectCommandString.AppendFormat("WHERE ");

                    // don't forget to append the trailing underscore for column names
                    selectCommandString.AppendFormat("{0} LIKE '{1}'", column.TagName.ToString() + "_", queryKey[tag]);
                    ++index;
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
                    if (SingleSessionDataAccessLayer.GetIDicomDictionary().Contains(new TagName(fieldName)))
                    {
                        // ensure that property actually has a value
                        object fieldValue = pi.GetValue(study, null);
                        if (null == fieldValue)
                            continue;

                        DictionaryEntry col = SingleSessionDataAccessLayer.GetIDicomDictionary().GetColumn(new TagName(fieldName));
                        DicomTag tag = new DicomTag(col.Path.GetLastPathElementAsInt32());
                        result.Add(tag, fieldValue.ToString());
                    }
                }

                results.Add(result);
            }
            return new ReadOnlyQueryResultCollection(results); 
        }

        #endregion
    }
}
