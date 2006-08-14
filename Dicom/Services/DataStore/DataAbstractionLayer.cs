using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.Attributes;

namespace ClearCanvas.Dicom.DataStore
{
    public sealed partial class DataAbstractionLayer
    {
        #region Handcoded Members
        private static ISession _currentSession = null;
        private static readonly ISessionFactory _sessionFactory;

        static DataAbstractionLayer()
        {
            Configuration cfg = new Configuration();
            string assemblyName = MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Name;
            cfg.Configure(assemblyName + ".cfg.xml");
            cfg.AddAssembly(assemblyName);
            _sessionFactory = cfg.BuildSessionFactory();
            _currentSession = _sessionFactory.OpenSession();
        }

        public static ISession GetCurrentSession()
        {
            if (null == _currentSession)
                _currentSession = _sessionFactory.OpenSession();

            return _currentSession;
        }

        public static void CloseSession()
        {
            if (null == _currentSession)
                // No current session
                return;

            _currentSession.Close();
            _currentSession = null;
        }

        public static void CloseSessionFactory()
        {
            if (_sessionFactory != null)
                _sessionFactory.Close();
        }

        private static ISessionFactory GetSessionFactory()
        {
            return _sessionFactory;
        }

        public static IDataStore GetIDataStore()
        {
            return new DataStore(GetCurrentSession());
        }

        public static IDataStoreWriteAccessor GetIDataStoreWriteAccessor()
        {
            return new DataStoreWriteAccessor(GetCurrentSession());
        }

        public static IDicomDictionary GetIDicomDictionary()
        {
            return new DicomDictionary(GetCurrentSession());
        }
        #endregion

    }
}
