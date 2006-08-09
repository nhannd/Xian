using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using Iesi.Collections;

namespace ClearCanvas.Dicom.DataStore
{
    public sealed partial class DataStore : IDataStore
    {
        #region Handcoded Members
        public DataStore()
        {

        }

        public DataStore(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        #region Private Members
        private ISessionFactory _sessionFactory;
        private ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }
        #endregion
        #endregion

        #region IDataStore Members

        public bool StudyExists(Uid referencedUid)
        {
            ISession session = null;
            try
            {
                session = SessionFactory.OpenSession();

                if (!session.IsConnected)
                    session.Reconnect();

                IList studies = session.CreateCriteria(typeof(Study))
                    .Add(Expression.Eq("StudyInstanceUid", referencedUid.ToString()))
                    .List();

                session.Disconnect();
                session.Close();

                return studies.Count > 0;
            }
            catch (Exception ex)
            {
                session.Disconnect();
                session.Close();
                throw ex;
            }            
        }

        public bool SeriesExists(Uid referencedUid)
        {
            ISession session = null;
            try
            {
                session = SessionFactory.OpenSession();

                if (!session.IsConnected)
                    session.Reconnect();

                IList seriesList = session.CreateCriteria(typeof(Series))
                    .Add(Expression.Eq("SeriesInstanceUid", referencedUid.ToString()))
                    .List();

                session.Disconnect();
                session.Close();

                return seriesList.Count > 0;
            }
            catch (Exception ex)
            {
                session.Disconnect();
                session.Close();
                throw ex;
            }            
        }

        public bool SopInstanceExists(Uid referencedUid)
        {
            ISession session = null;
            try
            {
                session = SessionFactory.OpenSession();

                if (!session.IsConnected)
                    session.Reconnect();

                IList sops = session.CreateCriteria(typeof(SopInstance))
                    .Add(Expression.Eq("SopInstanceUid", referencedUid.ToString()))
                    .List();

                session.Disconnect();
                session.Close();

                return sops.Count > 0;
            }
            catch (Exception ex)
            {
                session.Disconnect();
                session.Close();
                throw ex;
            }            
        }

        public ISopInstance GetSopInstance(Uid referencedUid)
        {
            ISession session = null;
            ISopInstance sop = null;
            try
            {
                session = SessionFactory.OpenSession();

                if (!session.IsConnected)
                    session.Reconnect();

                IList listOfSops = session.CreateCriteria(typeof(SopInstance))
                    .Add(Expression.Eq("SopInstanceUid", referencedUid.ToString()))
                    .SetFetchMode("WindowValues_", FetchMode.Eager)
                    .List();

                if (null != listOfSops && listOfSops.Count > 0)
                    sop = listOfSops[0] as ISopInstance;

                session.Disconnect();
                session.Close();
            }
            catch (Exception ex)
            {
                session.Disconnect();
                session.Close();
                throw ex;
            }

            return sop;
        }

        public ISeries GetSeries(Uid referenceUid)
        {
            return GetSeries(referenceUid, true);
        }

        public ISeries GetSeries(Uid referenceUid, bool willRetrievalAllRelatedInstances)
        {
            ISession session = null;
            ISeries series = null;
            try
            {
                session = SessionFactory.OpenSession();

                if (!session.IsConnected)
                    session.Reconnect();

                IList listOfSeries = session.CreateCriteria(typeof(Series))
                    .Add(Expression.Eq("SeriesInstanceUid", referenceUid.ToString()))
                    .SetFetchMode("SopInstance_", FetchMode.Eager)
                    .List();

                if (null != listOfSeries && listOfSeries.Count > 0)
                    series = listOfSeries[0] as ISeries;

                session.Disconnect();
                session.Close();
            }
            catch (Exception ex)
            {
                session.Disconnect();
                session.Close();
                throw ex;
            }

            return series;
        }

        public IStudy GetStudy(Uid referenceUid)
        {
            return GetStudy(referenceUid, true);
        }

        public IStudy GetStudy(Uid referenceUid, bool willRetrieveAllRelatedSeries)
        {
            ISession session = null;
            IStudy studyFound = null;
            try
            {
                session = SessionFactory.OpenSession();

                if (!session.IsConnected)
                    session.Reconnect();


                IList listOfStudies = session.CreateCriteria(typeof(Study))
                    .Add(Expression.Eq("StudyInstanceUid", referenceUid.ToString()))
                    .SetFetchMode("Series_", FetchMode.Eager)
                    .List();

                if (null != listOfStudies && listOfStudies.Count > 0)
                    studyFound = listOfStudies[0] as IStudy;

                session.Disconnect();
                session.Close();
            }
            catch (Exception ex)
            {
                session.Disconnect();
                session.Close();
                throw ex;
            }

            return studyFound;
        }
        #endregion
    }
}
