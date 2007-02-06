using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using NHibernate;
using NHibernate.Collection;

namespace ClearCanvas.Dicom.Services
{
    public class SendQueue : ISendQueue
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
        public void Add(ISendParcel aParcel)
        {
            if (aParcel == null)
                return;

            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();

                transaction = session.BeginTransaction();
                session.SaveOrUpdate(aParcel);
                transaction.Commit();
                session.Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            finally
            {
                session.Close();
            }
        }

        public void Remove(ISendParcel aParcel)
        {
            if (aParcel == null)
                return;

            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();

                transaction = session.BeginTransaction();
                session.Delete(aParcel);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            finally
            {
                session.Close();
            }
        }

        public ISendParcel CreateNewParcel(ApplicationEntity sourceAE, ApplicationEntity destinationAE, string parcelDescription)
        {
            SendParcel aNewParcel = new SendParcel(sourceAE, destinationAE, parcelDescription);
            aNewParcel.InitializeAssociatedCollection += InitializeAssociatedObject;

            return aNewParcel;
        }

        public void InitializeAssociatedObject(object parcel, PersistentCollection associatedCollection)
        {
            if (null == associatedCollection || associatedCollection.WasInitialized)
                return;

            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                session.Lock(parcel, LockMode.Read);
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

        public IEnumerable<ISendParcel> GetParcels()
        {
            IList listOfParcels;
            List<ISendParcel> returningParcels = new List<ISendParcel>();
            ISession session = null;

            ITransaction transaction = null;

            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();
                listOfParcels = session.Find("from Parcel");
                if (listOfParcels.Count <= 0)
                    return null;
            }
            catch 
            {
                transaction.Rollback();
                throw; 
            }
            finally { session.Close(); }

            foreach (ISendParcel parcel in listOfParcels)
            {
                returningParcels.Add(parcel);
            }
            
            return returningParcels;
        }

        public IEnumerable<ISendParcel> GetSendIncompleteParcels()
        {
            IList listOfParcels;
            List<ISendParcel> returningParcels = new List<ISendParcel>();
            ISession session = null;
            ITransaction transaction = null;

            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();
                listOfParcels = session.Find("from Parcel as parcel where parcel.ParcelTransferState != ?",
                    ParcelTransferState.Completed,
                    NHibernateUtil.Int16);

                if (listOfParcels.Count <= 0)
                    return null;

                foreach (ISendParcel parcel in listOfParcels)
                {
                    // workaround: fetching isn't being done automatically even though 'lazy' is set to false
                    // so here, we access the list of sop instance filenames as a means of indirectly causing
                    // a fetch to occur
                    IEnumerable<string> listOfFileNames = parcel.GetReferencedSopInstanceFileNames();
                    returningParcels.Add(parcel);
                }
            }
            catch 
            {
                transaction.Rollback();
                throw; 
            }
            finally { session.Close(); }

            return returningParcels;
        }

        public void UpdateParcel(ISendParcel aParcel)
        {
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();
                session.Update(aParcel);
                transaction.Commit();
            }
            catch 
            {
                transaction.Rollback();
                throw; 
            }
            finally { session.Close(); }
        }

        #endregion
    }
}
