using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
    internal class ShredController : MarshalByRefObject
    {
        public ShredController(AppDomain shredDomain, IShred shred, ShredStartupInfo startupInfo)
        {
            Platform.CheckForNullReference(shred, "shred");
            Platform.CheckForNullReference(shredDomain, "shredDomain");
            Platform.CheckForNullReference(startupInfo, "startupInfo");

            _domain = shredDomain;
            _shredObject = shred;
            _startupInfo = startupInfo;
            _id = ShredController.NextId;
            _thread = new Thread(new ParameterizedThreadStart(StartupShred));
            _runningState = RunningState.Stopped;
        }

        static ShredController()
        {
            _nextId = 101;
        }

        public void Start()
        {
            if (RunningState.Stopped == _runningState)
            {
                _thread.Start(this);
                _runningState = RunningState.Running;
            }
        }


        public void Stop()
        {
            if (RunningState.Running == _runningState)
            {
                _shredObject.Stop();
                _thread.Join();
                _runningState = RunningState.Stopped;
            }
        }

        private void StartupShred(object threadData)
        {
            ShredController shredController = threadData as ShredController;
            IWcfShred wcfShred = shredController.Shred as IWcfShred;
            if (null != wcfShred)
            {
                wcfShred.ServicePort = GetNextAvailableServicePort();
            }

            shredController.Shred.Start();
        }

        private static int GetNextAvailableServicePort()
        {
            int servicePort;

            lock (_lockObject)
            {
                servicePort = _servicePort;
                _servicePort++;
            }

            return servicePort;
        }

        private enum RunningState { Stopped, Running };

        #region Private fields
        private static int _servicePort = 21000;
        private static object _lockObject = new object();
        private RunningState _runningState;
        #endregion

        #region Properties
        private Thread _thread;
        private IShred _shredObject;
        private AppDomain _domain;
        private ShredStartupInfo _startupInfo;

        public ShredStartupInfo StartupInfo
        {
            get { return _startupInfo; }
        }
	
        private int _id;
        private static int _nextId;
        protected static int NextId
        {
            get { return _nextId++; }
        }

        public int Id
        {
            get { return _id; }
        }
	
        public AppDomain ShredDomain
        {
            get { return _domain; }
        }
	
        public IShred Shred
        {
            get { return _shredObject; }
        }
	
        public WcfDataShred WcfDataShred
        {
            get
            {
                return new WcfDataShred(this.Id, this.Shred.GetDisplayName(), this.Shred.GetDescription(), (RunningState.Running == _runningState) ? true : false);
            }
        }
	
        #endregion
    }
}
