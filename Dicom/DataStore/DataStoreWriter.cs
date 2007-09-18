using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;

namespace ClearCanvas.Dicom.DataStore
{
    internal class DataStoreWriter : IDataStoreWriter, IDataStoreCleaner
    {
        #region Handcoded Members

        public DataStoreWriter(ISessionFactory sessionFactory)
        {
            this.SessionFactory = sessionFactory;
        }
        #region Private Members
        private DataStoreWriter() { }
        private ISessionFactory _sessionFactory;

        private ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
            set { _sessionFactory = value; }
        }
	
        #endregion
        #endregion

        #region IDataStoreWriter Members

        public ISopInstance NewImageSopInstance()
        {
            return new ImageSopInstance();
        }

        public void StoreSopInstance(ISopInstance sop)
        {
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();
                session.SaveOrUpdate(sop);
                transaction.Commit();
            }
            catch
            {
                if (null != transaction)
                    transaction.Rollback();

                throw;
            }
            finally
            {
                if (null != session)
                    session.Close();
            }
        }

        public void StoreSeries(ISeries seriesToStore)
        {
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();
                session.SaveOrUpdate(seriesToStore);
                transaction.Commit();
            }
            catch
            {
                if (null != transaction)
                    transaction.Rollback();

                throw;
            }
            finally
            {
                if (null != session)
                    session.Close();
            }
        }

        public void RemoveSeries(ISeries seriesToRemove)
        {
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();
                session.Delete(seriesToRemove);
                transaction.Commit();
            }
            catch
            {
                if (null != transaction)
                    transaction.Rollback();

                throw;
            }
            finally
            {
                if (null != session)
                    session.Close();
            }
        }

		public void RemoveSopInstances(IEnumerable<ISopInstance> sops)
		{
			ISession session = null;
			ITransaction transaction = null;
			try
			{
				session = this.SessionFactory.OpenSession();
				transaction = session.BeginTransaction();

				foreach (ISopInstance sop in sops)
				{
					session.Delete(sop);
				}

				transaction.Commit();
			}
			catch
			{
				if (null != transaction)
					transaction.Rollback();

				throw;
			}
			finally
			{
				if (null != session)
					session.Close();
			}
		}
		
		public void RemoveSopInstance(ISopInstance sopToRemove)
        {
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                session.Delete(sopToRemove);

                transaction.Commit();
            }
            catch
            {
                if (null != transaction)
                    transaction.Rollback();

                throw;
            }
            finally
            {
                if (null!=session)
                    session.Close();
            }
        }

        public void StoreStudy(IStudy study)
        {
            ITransaction transaction = null;
            ISession session = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();
                session.SaveOrUpdate(study);
                transaction.Commit();
            }
            catch
            {
                if (null != transaction)
                    transaction.Rollback();
                throw;
            }
            finally
            {
                if (null != session)
                    session.Close();
            }
        }

        public void RemoveStudy(IStudy studyToRemove)
        {
            ITransaction transaction = null;
            ISession session = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();
                session.Delete(studyToRemove);
                transaction.Commit();
            }
            catch
            {
                if (null != transaction)
                    transaction.Rollback();

                throw;
            }
            finally
            {
                if (null != session)
                    session.Close();
            }
        }

 		public void StoreSopInstances(IEnumerable<ISopInstance> sops)
		{
			ISession session = null;
			ITransaction transaction = null;
			try
			{
				session = this.SessionFactory.OpenSession();
				transaction = session.BeginTransaction();
				
				foreach (ISopInstance sop in sops)
					session.SaveOrUpdate(sop);

				transaction.Commit();
			}
			catch
			{
				if (null != transaction)
					transaction.Rollback();

				throw;
			}
			finally
			{
				if (null != session)
					session.Close();
			}
		}

    	#endregion

		#region IDataStoreCleaner Members

		public void ClearAllStudies()
		{
			ITransaction transaction = null;
			ISession session = null;

			try
			{
				session = this.SessionFactory.OpenSession();
				transaction = session.BeginTransaction();
				session.Delete("from Study");
				transaction.Commit();
			}
			catch
			{
				if (transaction != null)
					transaction.Rollback();

				throw;
			}
			finally
			{
				if (session != null)
					session.Close();
			}
		}

		#endregion
	}
}
