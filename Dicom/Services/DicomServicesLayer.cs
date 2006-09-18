using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NHibernate;
using NHibernate.Cfg;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.Dicom.Services
{
    public class DicomServicesLayer 
    {
        #region Handcoded Members
        #region Private Members
        private static Dictionary<Thread, ISessionFactory> _sessionFactories;
        private static Configuration _hibernateConfiguration;
        private static Dictionary<ISessionFactory, ISendQueue> _sendQueues;
        private static Dictionary<ISessionFactory, ISender> _senders;

        public static Dictionary<ISessionFactory, ISendQueue> SendQueues
        {
            get { return _sendQueues; }
            set { _sendQueues = value; }
        }
	
        private static Dictionary<ISessionFactory, ISender> Senders
        {
            get { return _senders; }
            set { _senders = value; }
        }
	
        private static Configuration HibernateConfiguration
        {
            get { return _hibernateConfiguration; }
            set { _hibernateConfiguration = value; }
        }

        private static Dictionary<Thread, ISessionFactory> SessionFactories
        {
            get { return _sessionFactories; }
        }

        private static ISessionFactory CurrentFactory
        {
            get
            {
                // look for stale sessions and get rid of them
                List<KeyValuePair<Thread, ISessionFactory>> toRemoveArray = new List<KeyValuePair<Thread, ISessionFactory>>();
                foreach (KeyValuePair<Thread, ISessionFactory> iterator in DicomServicesLayer.SessionFactories)
                {
                    if (!iterator.Key.IsAlive)
                        toRemoveArray.Add(iterator);
                }
                foreach (KeyValuePair<Thread, ISessionFactory> iterator in toRemoveArray)
                {
                    // get rid of session-dependent objects that we remember about
                    DicomServicesLayer.SendQueues.Remove(iterator.Value);
                    DicomServicesLayer.Senders.Remove(iterator.Value);

                    // close the session and get rid of the session that we remember about
                    DicomServicesLayer.SessionFactories.Remove(iterator.Key);
                }
                toRemoveArray.Clear();

                Thread currentThread = Thread.CurrentThread;
                if (DicomServicesLayer.SessionFactories.ContainsKey(currentThread))
                {
                    return DicomServicesLayer.SessionFactories[currentThread];
                }
                else
                {
                    ISessionFactory newSessionFactory = DicomServicesLayer.HibernateConfiguration.BuildSessionFactory();
                    DicomServicesLayer.SessionFactories.Add(currentThread, newSessionFactory);
                    return newSessionFactory;
                }
            }
        }

        #endregion
        #endregion

        static DicomServicesLayer()
        {
            DicomServicesLayer.HibernateConfiguration = new Configuration();
            string assemblyName = MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Name;
            DicomServicesLayer.HibernateConfiguration.Configure(assemblyName + ".cfg.xml");
            DicomServicesLayer.HibernateConfiguration.AddAssembly(assemblyName);
            DicomServicesLayer.HibernateConfiguration.AddAssembly("ClearCanvas.Dicom.DataStore");

            _sessionFactories = new Dictionary<Thread, ISessionFactory>();
            _sendQueues = new Dictionary<ISessionFactory, ISendQueue>();
            _senders = new Dictionary<ISessionFactory, ISender>();
        }

        public static ISendQueue GetISendQueue()
        {
            ISessionFactory sessionFactory = DicomServicesLayer.CurrentFactory;
            if (DicomServicesLayer.SendQueues.ContainsKey(sessionFactory))
            {
                return DicomServicesLayer.SendQueues[sessionFactory];
            }
            else
            {
                ISendQueue sendQueue = new SendQueue(sessionFactory);
                DicomServicesLayer.SendQueues.Add(sessionFactory, sendQueue);
                return sendQueue;
            }
        }

        public static ISender GetISender(ApplicationEntity senderAE)
        {
            ISessionFactory sessionFactory = DicomServicesLayer.CurrentFactory;
            if (DicomServicesLayer.Senders.ContainsKey(sessionFactory))
            {
                return DicomServicesLayer.Senders[sessionFactory];
            }
            else
            {
                ISender sender = new Sender(senderAE);
                DicomServicesLayer.Senders.Add(sessionFactory, sender);
                return sender;
            }
        }

        public static IDicomSender GetIDicomSender()
        {
            return new DicomSender();
        }
    }
}
