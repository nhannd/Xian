using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;

namespace ClearCanvas.Dicom.DataStore
{
    public sealed partial class DataAbstractionLayer
    {
        public class DataStoreObjectCreator : IDataStoreObjectCreator
        {
            #region Handcoded Members
            public DataStoreObjectCreator(ISessionFactory sessionFactory)
            {
                _sessionFactory = sessionFactory;
            }

            #region Private Members
            private ISessionFactory SessionFactory
            {
                get { return _sessionFactory; }
            }

            private readonly ISessionFactory _sessionFactory;
            #endregion
            #endregion

            #region IDataStoreObjectCreation Members

            public ISopInstance NewImageSopInstance()
            {
                return new ImageSopInstance();
            }

            public void StoreImageSopInstance(ISopInstance sop)
            {
                ISession session = null;
                ITransaction tx = null;
                try
                {
                    session = SessionFactory.OpenSession();
                    tx = session.BeginTransaction();

                    session.Save(sop);
                    tx.Commit();

                    session.Close();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    session.Close();
                    throw ex;
                }
            }

            #endregion
        }
    }
}
