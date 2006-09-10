using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.Dicom.Services
{
    public class DicomServicesLayer 
    {
        #region Handcoded Members
        private static ISession _currentSession = null;
        private static readonly ISessionFactory _sessionFactory;

        static DicomServicesLayer()
        {
            Configuration cfg = new Configuration();
            string assemblyName = MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Name;
            cfg.Configure(assemblyName + ".cfg.xml");
            cfg.AddAssembly(assemblyName);
            cfg.AddAssembly("ClearCanvas.Dicom.DataStore");
            _sessionFactory = cfg.BuildSessionFactory();
        }

        private static ISession GetCurrentSession()
        {
            if (null == _currentSession)
                _currentSession = _sessionFactory.OpenSession();

            return _currentSession;
        }

        private static void CloseSession()
        {
            if (null == _currentSession)
                // No current session
                return;

            _currentSession.Close();
            _currentSession = null;
        }

        private static void CloseSessionFactory()
        {
            if (_sessionFactory != null)
                _sessionFactory.Close();
        }

        private static ISessionFactory GetSessionFactory()
        {
            return _sessionFactory;
        }

        public static ISendQueueService GetISendQueueService()
        {
            if (null == _sendQueue)
                _sendQueue = new SendQueue(GetSessionFactory());

            return _sendQueue;
        }

        public static ISendService GetISendService(ApplicationEntity senderAE)
        {
            if (null == _sendService)
                _sendService = new Sender(senderAE);

            return _sendService;
        }

        public static IDicomSender GetIDicomSender()
        {
            return new DicomSender();
        }
        #region Private Members
        private static ISendQueueService _sendQueue;
        private static ISendService _sendService;
        #endregion
        #endregion
    }
}
