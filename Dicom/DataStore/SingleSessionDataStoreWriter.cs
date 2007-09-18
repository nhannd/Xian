using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Expression;

namespace ClearCanvas.Dicom.DataStore
{
    internal class SingleSessionDataStoreWriter : IDataStoreWriter
    {
        #region Handcoded Members
        public SingleSessionDataStoreWriter(ISession session)
        {
            _session = session;
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

		public void StoreSopInstances(IEnumerable<ISopInstance> sops)
		{
			ITransaction tx = null; 
			try
			{
				tx = this.Session.BeginTransaction();

				foreach (ISopInstance sop in sops)
					this.Session.SaveOrUpdate(sop);
					
				tx.Commit();
			}
			catch
			{
				if (null != tx)
					tx.Rollback();

				throw ;
			}
		}
		
		public void StoreSopInstance(ISopInstance sop)
        {
            ITransaction tx = null;
            try
            {
                tx = this.Session.BeginTransaction();
                this.Session.SaveOrUpdate(sop);
                tx.Commit();
            }
            catch
            {
                if (null != tx)
                    tx.Rollback();

                throw;
            }
        }

		public void RemoveSopInstances(IEnumerable<ISopInstance> sops)
		{
			ITransaction tx = null;
			try
			{
				tx = this.Session.BeginTransaction();

				foreach (ISopInstance sop in sops)
				{
					this.Session.Delete(sop);
				}

				tx.Commit();
			}
			catch
			{
				if (null != tx)
					tx.Rollback();

				throw;
			}
		}

    	public void RemoveSopInstance(ISopInstance sopToRemove)
        {
            ITransaction tx = null;
            try
            {
                tx = this.Session.BeginTransaction();

                this.Session.Delete(sopToRemove);

                tx.Commit();
            }
            catch
            {
                if (null != tx)
                    tx.Rollback();

                throw;
            }
        }

        public void StoreSeries(ISeries seriesToStore)
        {
            ITransaction tx = null;
            try
            {
                tx = this.Session.BeginTransaction();
                this.Session.SaveOrUpdate(seriesToStore);
                tx.Commit();
            }
            catch
            {
                if (null != tx)
                    tx.Rollback();

                throw;
            }
        }

        public void RemoveSeries(ISeries seriesToRemove)
        {
            ITransaction tx = null;
            try
            {
                tx = this.Session.BeginTransaction();
                this.Session.Delete(seriesToRemove);
                tx.Commit();
            }
            catch
            {
                if (null != tx)
                    tx.Rollback();

                throw;
            }
        }

        public void StoreStudy(IStudy study)
        {
            ITransaction tx = null;
            try
            {
                tx = this.Session.BeginTransaction();
                this.Session.SaveOrUpdate(study);
                tx.Commit();
            }
            catch
            {
                if (null != tx)
                    tx.Rollback();
                throw;
            }
        }

        public void RemoveStudy(IStudy studyToRemove)
        {
            ITransaction tx = null;
            try
            {
                tx = this.Session.BeginTransaction();
                this.Session.Delete(studyToRemove);
                tx.Commit();
            }
            catch
            {
                if (null != tx)
                    tx.Rollback();

                throw;
            }
        }

    	#endregion
	}
}
