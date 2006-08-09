using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using NHibernate;

namespace ClearCanvas.Dicom.Services
{
    public class SendQueue : ISendQueueService
    {
        #region Handcoded Members
        public SendQueue(ISessionFactory sessionFactory)
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

        #region ISendQueueService Members
        public void Add(IParcel aParcel)
        {
            ISession session = null;
            ITransaction tx = null;
            try
            {
                session = this.SessionFactory.OpenSession();

                if (!session.IsConnected)
                    session.Reconnect();

                tx = session.BeginTransaction();
                session.SaveOrUpdate(aParcel);
                tx.Commit();

                session.Flush();
                session.Disconnect();
                session.Close();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                session.Close();
                throw ex;
            }
        }

        public void Remove(IParcel aParcel)
        {
            ISession session = null;
            ITransaction tx = null;
            try
            {
                session = this.SessionFactory.OpenSession();

                if (!session.IsConnected)
                    session.Reconnect();

                tx = session.BeginTransaction();
                session.Delete(aParcel);
                tx.Commit();

                session.Flush();
                session.Disconnect();
                session.Close();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                session.Close();
                throw ex;
            }
        }

        public IParcel CreateNewParcel(ApplicationEntity sourceAE, ApplicationEntity destinationAE)
        {
            return new Parcel(sourceAE, destinationAE);
        }

        #endregion
    }
}
