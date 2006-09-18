using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using Iesi.Collections;
using System.Reflection;

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
                    studyFound = listOfStudies[0] as IStudy;
            }
            catch (Exception ex)
            {
                throw ex;
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
            try
            {
                IQuery query = this.Session.CreateQuery(selectCommandString.ToString());
                studiesFound = query.List();
            }
            catch (Exception ex)
            {
                throw ex;
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

        public void InitializeAssociatedObject(object parentObject, object childObject)
        {
            // TODO: specific cases of check for whether the child object
            // has already been initialized, and skipping if it has. If 
            // we are using v1.2 of NHibernate or later, there should be
            // a WasInitialized property that can be checked

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

        public ISeries GetSeriesAndSopInstances(Uid referenceUid)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IStudy GetStudyAndSeries(Uid referenceUid)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IStudy GetStudyAndAllObjects(Uid referenceUid)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
