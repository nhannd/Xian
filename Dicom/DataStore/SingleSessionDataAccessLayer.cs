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
    public partial class SingleSessionDataAccessLayer : IDisposable
    {
        #region Handcoded Members
        #region Private Members

        private static readonly ISessionFactory _sessionFactory;
        private static Dictionary<Thread, ISession> _sessions;
        private static Dictionary<ISession, IDataStoreReader> _dataStores;
        private static Dictionary<ISession, IDataStoreWriter> _dataStoreWriteAccessors;
        private static IDicomDictionary _dicomDictionary;

        private static void CloseSessionFactory()
        {          
            if (_sessionFactory != null)
                _sessionFactory.Close();
        }

        private static ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }

        private static Dictionary<ISession, IDataStoreReader> DataStores
        {
            get { return _dataStores; }
        }

        private static Dictionary<Thread, ISession> Sessions
        {
            get { return _sessions; }
        }

        private static Dictionary<ISession, IDataStoreWriter> DataStoreWriteAccessors
        {
            get { return _dataStoreWriteAccessors; }
        }

        private static ISession CurrentSession
        {
            get 
            {
                // look for stale sessions and get rid of them
                List<KeyValuePair<Thread,ISession>> toRemoveArray = new List<KeyValuePair<Thread,ISession>>();
                foreach (KeyValuePair<Thread, ISession> iterator in SingleSessionDataAccessLayer.Sessions)
                {
                    if (!iterator.Key.IsAlive || !iterator.Value.IsOpen)
                        toRemoveArray.Add(iterator);
                }
                foreach (KeyValuePair<Thread, ISession> iterator in toRemoveArray)
                {
                    // get rid of session-dependent objects that we remember about
                    SingleSessionDataAccessLayer.DataStores.Remove(iterator.Value);
                    SingleSessionDataAccessLayer.DataStoreWriteAccessors.Remove(iterator.Value);

                    // close the session and get rid of the session that we remember about
                    iterator.Value.Close();
                    SingleSessionDataAccessLayer.Sessions.Remove(iterator.Key);
                }
                toRemoveArray.Clear();

                Thread currentThread = Thread.CurrentThread;
                if (SingleSessionDataAccessLayer.Sessions.ContainsKey(currentThread))
                {
                    return SingleSessionDataAccessLayer.Sessions[currentThread];
                }
                else
                {
                    ISession newSession = SingleSessionDataAccessLayer.SessionFactory.OpenSession();
                    SingleSessionDataAccessLayer.Sessions.Add(currentThread, newSession);
                    return newSession;
                }
            }
        }

        #endregion

        static SingleSessionDataAccessLayer()
        {
            Configuration cfg = new Configuration();
            string assemblyName = MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Name;
            cfg.Configure(assemblyName + ".cfg.xml");
            cfg.AddAssembly(assemblyName);
            _sessionFactory = cfg.BuildSessionFactory();

            _sessions = new Dictionary<Thread, ISession>();
            _dataStores = new Dictionary<ISession, IDataStoreReader>();
            _dataStoreWriteAccessors = new Dictionary<ISession, IDataStoreWriter>();
        }

        public static void CloseCurrentSession()
        {
            SingleSessionDataAccessLayer.CurrentSession.Close();
        }

        public static void ClearCurrentSession()
        {
            SingleSessionDataAccessLayer.CurrentSession.Clear();
        }

        public static void ReconnectCurrentSession()
        {
            if (!SingleSessionDataAccessLayer.CurrentSession.IsConnected)
                SingleSessionDataAccessLayer.CurrentSession.Reconnect();
        }

        public static void DisconnectCurrentSession()
        {
            SingleSessionDataAccessLayer.CurrentSession.Disconnect();
        }

        public static IDataStoreReader GetIDataStoreReader()
        {
            ISession session = SingleSessionDataAccessLayer.CurrentSession;
            if (SingleSessionDataAccessLayer.DataStores.ContainsKey(session))
            {
                return SingleSessionDataAccessLayer.DataStores[session];
            }
            else
            {
                IDataStoreReader dataStore = new SingleSessionDataStoreReader(session);
                SingleSessionDataAccessLayer.DataStores.Add(session, dataStore);
                return dataStore;
            }
        }

        public static IDataStoreWriter GetIDataStoreWriter()
        {
            ISession session = SingleSessionDataAccessLayer.CurrentSession;
            if (SingleSessionDataAccessLayer.DataStoreWriteAccessors.ContainsKey(session))
            {
                return SingleSessionDataAccessLayer.DataStoreWriteAccessors[session];
            }
            else
            {
                IDataStoreWriter dataStoreWriteAccessor = new SingleSessionDataStoreWriter(session);
                SingleSessionDataAccessLayer.DataStoreWriteAccessors.Add(session, dataStoreWriteAccessor);
                return dataStoreWriteAccessor;
            }
        }

        public static IDicomDictionary GetIDicomDictionary()
        {
            if (null == _dicomDictionary)
                _dicomDictionary = new DicomDictionary(SingleSessionDataAccessLayer.SessionFactory.OpenSession());

            return _dicomDictionary;
        }

        public static IDicomPersistentStore GetIDicomPersistentStore()
        {
            return new SingleSessionDicomImageStore();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            SingleSessionDataAccessLayer.ClearCurrentSession();
            SingleSessionDataAccessLayer.CloseCurrentSession();
            SingleSessionDataAccessLayer.CloseSessionFactory();
        }

        #endregion
    }
}
