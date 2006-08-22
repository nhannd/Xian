using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Expression;

namespace ClearCanvas.Dicom.DataStore
{
    internal class DataStoreWriteAccessor : IDataStoreWriteAccessor
    {
        #region Handcoded Members
        public DataStoreWriteAccessor(ISession session)
        {
            _session = session;
        }

        public void StoreDictionary(DicomDictionaryContainer container)
        {
            ITransaction tx = null;
            try
            {
                if (!this.Session.IsConnected)
                    this.Session.Reconnect();

                tx = this.Session.BeginTransaction();
                this.Session.SaveOrUpdate(container);
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }
        }

        #region Private Members
        private ISession Session
        {
            get { return _session; }
        }

        private readonly ISession _session;
        #endregion
        #endregion

        #region IDataStoreWriteAccessor Members

        public ISopInstance NewImageSopInstance()
        {
            return new ImageSopInstance();
        }

        public void StoreSopInstance(ISopInstance sop)
        {
            ITransaction tx = null;
            try
            {
                if (!this.Session.IsConnected)
                    this.Session.Reconnect();

                tx = this.Session.BeginTransaction();
                this.Session.SaveOrUpdate(sop);
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }
        }

        public void RemoveSopInstance(ISopInstance sopToRemove)
        {
            ITransaction tx = null;
            try
            {
                if (!this.Session.IsConnected)
                    this.Session.Reconnect();

                tx = this.Session.BeginTransaction();

                // remove the child sop instance from the parent series' collection
                ISeries series = sopToRemove.GetParentSeries();
                if (null != series)
                    series.RemoveSopInstance(sopToRemove);

                this.Session.Delete(sopToRemove);

                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }
        }

        public void StoreSeries(ISeries seriesToStore)
        {
            ITransaction tx = null;
            try
            {
                if (!this.Session.IsConnected)
                    this.Session.Reconnect();

                tx = this.Session.BeginTransaction();
                this.Session.SaveOrUpdate(seriesToStore);
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }
        }

        public void RemoveSeries(ISeries seriesToRemove)
        {
            ITransaction tx = null;
            try
            {
                if (!this.Session.IsConnected)
                    this.Session.Reconnect();

                tx = this.Session.BeginTransaction();
                this.Session.Delete(seriesToRemove);
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }
        }

        public void StoreStudy(IStudy study)
        {
            ITransaction tx = null;
            try
            {
                if (!this.Session.IsConnected)
                    this.Session.Reconnect();

                tx = this.Session.BeginTransaction();
                this.Session.SaveOrUpdate(study);
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }                
        }

        public void RemoveStudy(IStudy studyToRemove)
        {
            ITransaction tx = null;
            try
            {
                if (!this.Session.IsConnected)
                    this.Session.Reconnect();

                tx = this.Session.BeginTransaction();
                this.Session.Delete(studyToRemove);
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }
        }

        #endregion
    }

}
