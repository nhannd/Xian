using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;

namespace ClearCanvas.Dicom.DataStore
{
    public class DataAccessLayer
    {

        #region Handcoded Members
        #region Private Members
        private static Dictionary<Thread, ISessionFactory> _sessionFactories;
        private static Configuration _hibernateConfiguration;
        private static Dictionary<ISessionFactory, IDataStoreReader> _dataStoreReaders;
        private static Dictionary<ISessionFactory, IDataStoreWriter> _dataStoreWriters;
		private static Dictionary<string, IDicomDictionary> _dicomDictionaries;

        private static Dictionary<ISessionFactory, IDataStoreWriter> DataStoreWriters
        {
            get { return _dataStoreWriters; }
            set { _dataStoreWriters = value; }
        }
	
        private static Dictionary<ISessionFactory, IDataStoreReader> DataStoreReaders
        {
            get { return _dataStoreReaders; }
            set { _dataStoreReaders = value; }
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
                foreach (KeyValuePair<Thread, ISessionFactory> iterator in DataAccessLayer.SessionFactories)
                {
                    if (!iterator.Key.IsAlive)
                        toRemoveArray.Add(iterator);
                }
                foreach (KeyValuePair<Thread, ISessionFactory> iterator in toRemoveArray)
                {
                    // get rid of session-dependent objects that we remember about
                    DataAccessLayer.DataStoreReaders.Remove(iterator.Value);
                    DataAccessLayer.DataStoreWriters.Remove(iterator.Value);

                    // close the session and get rid of the session that we remember about
                    DataAccessLayer.SessionFactories.Remove(iterator.Key);
                }
                toRemoveArray.Clear();

                Thread currentThread = Thread.CurrentThread;
                if (DataAccessLayer.SessionFactories.ContainsKey(currentThread))
                {
                    return DataAccessLayer.SessionFactories[currentThread];
                }
                else
                {
                    ISessionFactory newSessionFactory = DataAccessLayer.HibernateConfiguration.BuildSessionFactory();
                    DataAccessLayer.SessionFactories.Add(currentThread, newSessionFactory);
                    return newSessionFactory;
                }
            }
        }

        #endregion
        #endregion

        static DataAccessLayer()
        {
            DataAccessLayer.HibernateConfiguration = new Configuration();
            string assemblyName = MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Name;
            DataAccessLayer.HibernateConfiguration.Configure(assemblyName + ".cfg.xml");
            DataAccessLayer.HibernateConfiguration.AddAssembly(assemblyName);

            _sessionFactories = new Dictionary<Thread, ISessionFactory>();
            _dataStoreReaders = new Dictionary<ISessionFactory, IDataStoreReader>();
            _dataStoreWriters = new Dictionary<ISessionFactory, IDataStoreWriter>();
			_dicomDictionaries = new Dictionary<string, IDicomDictionary>();
        }

        public static IDataStoreReader GetIDataStoreReader()
        {
            ISessionFactory sessionFactory = DataAccessLayer.CurrentFactory;
            if (DataAccessLayer.DataStoreReaders.ContainsKey(sessionFactory))
            {
                return DataAccessLayer.DataStoreReaders[sessionFactory];
            }
            else
            {
                IDataStoreReader dataStoreReader = new DataStoreReader(sessionFactory);
                DataAccessLayer.DataStoreReaders.Add(sessionFactory, dataStoreReader);
                return dataStoreReader;
            }
        }

        public static IDataStoreWriter GetIDataStoreWriter()
        {
            ISessionFactory sessionFactory = DataAccessLayer.CurrentFactory;
            if (DataAccessLayer.DataStoreWriters.ContainsKey(sessionFactory))
            {
                return DataAccessLayer.DataStoreWriters[sessionFactory];
            }
            else
            {
                IDataStoreWriter dataStoreWriter = new DataStoreWriter(sessionFactory);
                DataAccessLayer.DataStoreWriters.Add(sessionFactory, dataStoreWriter);
                return dataStoreWriter;
            }
        }

		public static IDataStoreCleaner GetIDataStoreCleaner()
		{
			return (IDataStoreCleaner)GetIDataStoreWriter();
		}

		internal static IDicomDictionary GetIDicomDictionary()
		{
			return GetIDicomDictionary(DicomDictionary.DefaultDictionaryName);
		}

		internal static IDicomDictionary GetIDicomDictionary(string dictionaryName)
		{
			if (!_dicomDictionaries.ContainsKey(dictionaryName))
				_dicomDictionaries[dictionaryName] = new DicomDictionary(DataAccessLayer.CurrentFactory.OpenSession(), dictionaryName);

			return _dicomDictionaries[dictionaryName];
		}
	}
}
