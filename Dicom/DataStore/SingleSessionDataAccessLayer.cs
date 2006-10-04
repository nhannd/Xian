using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.Attributes;

namespace ClearCanvas.Dicom.DataStore
{
    public class SingleSessionDataAccessLayer
    {
        #region Handcoded Members
        #region Private Members

        private static Dictionary<Thread, ISessionFactory> _sessionFactories;
        private static Dictionary<ISessionFactory, ISession> _sessions;
        private static Dictionary<ISession, IDataStoreReader> _dataStores;
        private static Dictionary<ISession, IDataStoreWriter> _dataStoreWriteAccessors;
        private static IDicomDictionary _dicomDictionary;
        private static Configuration _hibernateConfiguration;

        private static Dictionary<Thread, ISessionFactory> SessionFactories
        {
            get { return _sessionFactories; }
        }

        private static Dictionary<ISession, IDataStoreReader> DataStoreReaders
        {
            get { return _dataStores; }
        }

        private static Dictionary<ISessionFactory, ISession> Sessions
        {
            get { return _sessions; }
        }

        private static Dictionary<ISession, IDataStoreWriter> DataStoreWriters
        {
            get { return _dataStoreWriteAccessors; }
        }

        private static Configuration HibernateConfiguration
        {
            get { return _hibernateConfiguration; }
        }

        private static ISessionFactory CurrentFactory
        {
            get
            {
                // look for stale sessions and get rid of them
                List<KeyValuePair<Thread, ISessionFactory>> toRemoveArray = new List<KeyValuePair<Thread, ISessionFactory>>();
                foreach (KeyValuePair<Thread, ISessionFactory> iterator in SingleSessionDataAccessLayer.SessionFactories)
                {
                    if (!iterator.Key.IsAlive)
                        toRemoveArray.Add(iterator);
                }
                foreach (KeyValuePair<Thread, ISessionFactory> iterator in toRemoveArray)
                {
                    // get rid of session-dependent objects that we remember about, if the session
                    // hasn't already been removed due to the Sqlite workaround.
                    if (SingleSessionDataAccessLayer.Sessions.ContainsKey(iterator.Value))
                    {
                        ISession sessionToRemove = SingleSessionDataAccessLayer.Sessions[iterator.Value];
                        SingleSessionDataAccessLayer.DataStoreReaders.Remove(sessionToRemove);
                        SingleSessionDataAccessLayer.DataStoreWriters.Remove(sessionToRemove);
                        SingleSessionDataAccessLayer.Sessions.Remove(iterator.Value);
                    }

                    // close the session and get rid of the session that we remember about
                    SingleSessionDataAccessLayer.SessionFactories.Remove(iterator.Key);
                }
                toRemoveArray.Clear();

                Thread currentThread = Thread.CurrentThread;
                if (SingleSessionDataAccessLayer.SessionFactories.ContainsKey(currentThread))
                {
                    return SingleSessionDataAccessLayer.SessionFactories[currentThread];
                }
                else
                {
                    ISessionFactory newSessionFactory = SingleSessionDataAccessLayer.HibernateConfiguration.BuildSessionFactory();
                    SingleSessionDataAccessLayer.SessionFactories.Add(currentThread, newSessionFactory);
                    return newSessionFactory;
                }
            }
        }

        private static ISession CurrentSession
        {
            get 
            {
                if (SingleSessionDataAccessLayer.Sessions.ContainsKey(SingleSessionDataAccessLayer.CurrentFactory))
                {
                    return SingleSessionDataAccessLayer.Sessions[SingleSessionDataAccessLayer.CurrentFactory];
                }
                else
                {
                    ISession newSession = SingleSessionDataAccessLayer.CurrentFactory.OpenSession();
                    SingleSessionDataAccessLayer.Sessions.Add(SingleSessionDataAccessLayer.CurrentFactory, newSession);
                    return newSession;
                }
            }
        }

        #endregion

        static SingleSessionDataAccessLayer()
        {
            _hibernateConfiguration = new Configuration();
            string assemblyName = MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Name;
            _hibernateConfiguration.Configure(assemblyName + ".cfg.xml");
            _hibernateConfiguration.AddAssembly(assemblyName);
            _sessionFactories = new Dictionary<Thread, ISessionFactory>();
            _sessionFactories.Add(Thread.CurrentThread, _hibernateConfiguration.BuildSessionFactory());
            _sessions = new Dictionary<ISessionFactory, ISession>();
            _dataStores = new Dictionary<ISession, IDataStoreReader>();
            _dataStoreWriteAccessors = new Dictionary<ISession, IDataStoreWriter>();
        }

        public static void SqliteWorkaround()
        {
            SingleSessionDataAccessLayer.CurrentSession.Disconnect();
            SingleSessionDataAccessLayer.CurrentSession.Clear();
            SingleSessionDataAccessLayer.CurrentSession.Close();
            SingleSessionDataAccessLayer.RemoveCurrentSession();
        }

        private static void RemoveCurrentSession()
        {
            SingleSessionDataAccessLayer.DataStoreReaders.Remove(SingleSessionDataAccessLayer.CurrentSession);
            SingleSessionDataAccessLayer.DataStoreWriters.Remove(SingleSessionDataAccessLayer.CurrentSession);
            SingleSessionDataAccessLayer.Sessions.Remove(SingleSessionDataAccessLayer.CurrentFactory);
        }

        //public static void CloseCurrentSession()
        //{
        //    SingleSessionDataAccessLayer.CurrentSession.Close();
        //}

        //public static void ClearCurrentSession()
        //{
        //    SingleSessionDataAccessLayer.CurrentSession.Clear();
        //}

        //public static void ReconnectCurrentSession()
        //{
        //    if (!SingleSessionDataAccessLayer.CurrentSession.IsConnected)
        //        SingleSessionDataAccessLayer.CurrentSession.Reconnect();
        //}

        //public static void DisconnectCurrentSession()
        //{
        //    SingleSessionDataAccessLayer.CurrentSession.Disconnect();
        //}

        public static IDataStoreReader GetIDataStoreReader()
        {
            ISession session = SingleSessionDataAccessLayer.CurrentSession;
            if (SingleSessionDataAccessLayer.DataStoreReaders.ContainsKey(session))
            {
                return SingleSessionDataAccessLayer.DataStoreReaders[session];
            }
            else
            {
                IDataStoreReader dataStore = new SingleSessionDataStoreReader(session);
                SingleSessionDataAccessLayer.DataStoreReaders.Add(session, dataStore);
                return dataStore;
            }
        }

        public static IDataStoreWriter GetIDataStoreWriter()
        {
            ISession session = SingleSessionDataAccessLayer.CurrentSession;
            if (SingleSessionDataAccessLayer.DataStoreWriters.ContainsKey(session))
            {
                return SingleSessionDataAccessLayer.DataStoreWriters[session];
            }
            else
            {
                IDataStoreWriter dataStoreWriteAccessor = new SingleSessionDataStoreWriter(session);
                SingleSessionDataAccessLayer.DataStoreWriters.Add(session, dataStoreWriteAccessor);
                return dataStoreWriteAccessor;
            }
        }

        public static IDicomDictionary GetIDicomDictionary()
        {
            if (null == _dicomDictionary)
                _dicomDictionary = new DicomDictionary(SingleSessionDataAccessLayer.CurrentFactory.OpenSession());

            return _dicomDictionary;
        }

        public static IDicomPersistentStore GetIDicomPersistentStore()
        {
            return new SingleSessionDicomImageStore();
        }

        #endregion
    }
}
