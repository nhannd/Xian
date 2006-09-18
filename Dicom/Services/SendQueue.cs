using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using NHibernate;

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
        public void Add(IParcel aParcel)
        {
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

        public void Remove(IParcel aParcel)
        {
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

        public IParcel CreateNewParcel(ApplicationEntity sourceAE, ApplicationEntity destinationAE, string parcelDescription)
        {
            return new Parcel(sourceAE, destinationAE, parcelDescription);
        }

        public IEnumerable<IParcel> GetParcels()
        {
            IList listOfParcels;
            List<IParcel> returningParcels = new List<IParcel>();
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

            foreach (IParcel parcel in listOfParcels)
            {
                returningParcels.Add(parcel);
            }
            
            return returningParcels;
        }

        public IEnumerable<IParcel> GetSendIncompleteParcels()
        {
            IList listOfParcels;
            List<IParcel> returningParcels = new List<IParcel>();
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

                foreach (IParcel parcel in listOfParcels)
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

        public void UpdateParcel(IParcel aParcel)
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
