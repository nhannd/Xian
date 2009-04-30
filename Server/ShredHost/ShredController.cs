#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;

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
            _thread.Name = String.Format("{0}", _shredCacheObject.GetDisplayName());
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
            try
            {
                if (wcfShred != null)
                {
                    wcfShred.SharedHttpPort = ShredHostServiceSettings.Instance.SharedHttpPort;
                    wcfShred.SharedTcpPort = ShredHostServiceSettings.Instance.SharedTcpPort;
                }
                shredController.Shred.Start();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when starting up Shred {0}",
                             shredController.Shred.GetDescription());
            }
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
