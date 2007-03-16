using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
    internal class ShredController : MarshalByRefObject
    {
        public ShredController(ShredStartupInfo startupInfo)
        {
            Platform.CheckForNullReference(startupInfo, "startupInfo");

            _startupInfo = startupInfo;
            _id = ShredController.NextId;
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

            // create the AppDomain that the shred object will be instantiated in
            _domain = AppDomain.CreateDomain(_startupInfo.ShredTypeName);
            _shredObject = (IShred)_domain.CreateInstanceFromAndUnwrap(_startupInfo.AssemblyPath.LocalPath, _startupInfo.ShredTypeName);
            
            // cache the shred's details so that even if the shred is stopped and unloaded
            // we still have it's display name 
            _shredCacheObject = new ShredCacheObject(_shredObject.GetDisplayName(), _shredObject.GetDescription());
            
            // create the thread and start it
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
            AppDomain.Unload(_domain);
            _domain = null;         // need to explicity set to null, otherwise any references to it in the future will throw an exception
            _shredObject = null;
            _thread = null;

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
			wcfShred.HttpPort = 8088;
			wcfShred.TcpPort = 4044;
            shredController.Shred.Start();
        }

        private class ShredCacheObject : IShred
        {
            public ShredCacheObject(string displayName, string description)
            {
                _displayName = displayName;
                _description = description;
            }

            #region Private fields
            private string _displayName;
            private string _description;
            #endregion

            #region IShred Members

            public void Start()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void Stop()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public string GetDisplayName()
            {
                return _displayName;
            }

            public string GetDescription()
            {
                return _description;
            }

            #endregion
        }

        #region Private fields
        private static object _lockShredId = new object();
        private static object _lockRunningState = new object();
        private RunningState _runningState;
        #endregion

        #region Properties
        private Thread _thread;
        private IShred _shredObject;
        private ShredCacheObject _shredCacheObject;
        private AppDomain _domain;
        private ShredStartupInfo _startupInfo;
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

        public ShredStartupInfo StartupInfo
        {
            get { return _startupInfo; }
        }

        public IShred Shred
        {
            get 
            {
                if (null == _shredObject)
                    return _shredCacheObject;
                else
                    return _shredObject; 
            }
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
