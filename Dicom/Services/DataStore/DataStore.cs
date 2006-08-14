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
    public sealed partial class DataStore : IDataStore
    {
        #region Handcoded Members
        private DataStore()
        {
            
        }

        public DataStore(ISession session) : this()
        {
            _session = session;
            _dicomDictionary = new DicomDictionary(_session);
        }

        #region Private Members
        private ISession _session;
        private ISession Session
        {
            get { return _session; }
        }

        static private DicomDictionary _dicomDictionary;
        static private DicomDictionary DicomDictionary
        {
            get { return _dicomDictionary; }
        }
        #endregion
        #endregion

        #region IDataStore Members

        public bool StudyExists(Uid referencedUid)
        {
            try
            {
                if (!this.Session.IsConnected)
                    this.Session.Reconnect();

                IList studies = this.Session.CreateCriteria(typeof(Study))
                    .Add(Expression.Eq("StudyInstanceUid", referencedUid.ToString()))
                    .List();

                return studies.Count > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public bool SeriesExists(Uid referencedUid)
        {
            try
            {
                if (!this.Session.IsConnected)
                    this.Session.Reconnect();

                IList seriesList = this.Session.CreateCriteria(typeof(Series))
                    .Add(Expression.Eq("SeriesInstanceUid", referencedUid.ToString()))
                    .List();

                return seriesList.Count > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public bool SopInstanceExists(Uid referencedUid)
        {
            try
            {
                if (!this.Session.IsConnected)
                    this.Session.Reconnect();

                IList sops = this.Session.CreateCriteria(typeof(SopInstance))
                    .Add(Expression.Eq("SopInstanceUid", referencedUid.ToString()))
                    .List();

                return sops.Count > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public ISopInstance GetSopInstance(Uid referencedUid)
        {
            ISopInstance sop = null;
            try
            {
                if (!this.Session.IsConnected)
                    this.Session.Reconnect();

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
            ISeries series = null;
            try
            {
                if (!this.Session.IsConnected)
                    this.Session.Reconnect();

                IList listOfSeries = this.Session.CreateCriteria(typeof(Series))
                    .Add(Expression.Eq("SeriesInstanceUid", referenceUid.ToString()))
                    .SetFetchMode("SopInstances", FetchMode.Eager)
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
            IStudy studyFound = null;
            try
            {
                if (!this.Session.IsConnected)
                    this.Session.Reconnect();


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
                DictionaryEntry column = DataStore.DicomDictionary.GetColumn(path);

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
                if (!this.Session.IsConnected)
                    this.Session.Reconnect();

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
                    if (DataStore.DicomDictionary.Contains(new TagName(fieldName)))
                    {
                        // ensure that property actually has a value
                        object fieldValue = pi.GetValue(study, null);
                        if (null == fieldValue)
                            continue;

                        DictionaryEntry col = DataStore.DicomDictionary.GetColumn(new TagName(fieldName));
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
