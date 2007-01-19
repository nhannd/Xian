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

        public bool Start()
        {
            lock (_lockRunningState)
            {
                if (RunningState.Running == _runningState || RunningState.Transition == _runningState)
                    return (RunningState.Running == _runningState);

                _runningState = RunningState.Transition;
            }

            _thread = new Thread(new ParameterizedThreadStart(StartupShred));
            _thread.Start(this);

            lock (_lockRunningState)
            {
                _runningState = RunningState.Running;
            }

            return (RunningState.Running == _runningState);
        }


        public bool Stop()
        {
            lock (_lockRunningState)
            {
                if (RunningState.Stopped == _runningState || RunningState.Transition == _runningState)
                    return (RunningState.Running == _runningState);

                _runningState = RunningState.Transition;
            }

            _shredObject.Stop();
            _thread.Join();

            lock (_lockRunningState)
            {
                _runningState = RunningState.Stopped;
            }

            return (RunningState.Running == _runningState);
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

            lock (_lockShredId)
            {
                servicePort = _servicePort;
                _servicePort++;
            }

            return servicePort;
        }

        #region Private fields
        private static int _servicePort = 49153;
        private static object _lockShredId = new object();
        private static object _lockRunningState = new object();
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
