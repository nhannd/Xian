#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageServer.Services.Shreds.ServerInfo
{
    /// <summary>
    /// Abstract class for shreds that run periodically
    /// </summary>
    public abstract class TimerShred :  Shred
    {
        #region Private Members
        private readonly TimeSpan _delay;
        private readonly TimeSpan _interval;
        private Timer _heartBeat = null;
        private readonly CallbackDelegate _callback;
        #endregion

        #region Delegate
        /// <summary>
        /// Delegate method called after each interval.
        /// </summary>
        public delegate void CallbackDelegate();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="TimerShred"/> with the specified delay, interval and callback method.
        /// </summary>
        /// <param name="delay">Delay before the callback is first executed</param>
        /// <param name="interval">Interval between callback executions</param>
        /// <param name="callback">Method to callback on each period.</param>
        public TimerShred(TimeSpan delay, TimeSpan interval, CallbackDelegate callback)
        {
            Platform.CheckForNullReference(interval, "interval");
            Platform.CheckForNullReference(callback, "callback");
            _delay = delay;
            _interval = interval;
            _callback = callback;
        }
        #endregion
        public override void Start()
        {
            try
            {
                _heartBeat = new Timer(delegate
                                                {
                                                    try
                                                    {
                                                        _callback();
                                                    }
                                                    catch(Exception e)
                                                    {
                                                        Platform.Log(LogLevel.Error, e, "Error occurred in {0}",GetDisplayName());
                                                    }
                                                }, null, _delay, _interval);
                
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Fatal, e, "Unexpected exception starting {0}", GetDisplayName());
                throw;
            }
        }
      

        public override void Stop()
        {
            if (_heartBeat != null)
                _heartBeat.Dispose();
        }

       
    }

    /// <summary>
    /// Shred that periodically updates the server information in the database.
    /// </summary>
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class ServerInformationShred : TimerShred
    {
        public ServerInformationShred()
            :base(TimeSpan.Zero, TimeSpan.FromMinutes(5), UpdateDB)
        {
            
        }

        private static void UpdateDB()
        {
            
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();

            using (IUpdateContext context = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IServerInformationEntityBroker broker = context.GetBroker<IServerInformationEntityBroker>();
                ServerInformationUpdateColumns columns = new ServerInformationUpdateColumns();

                columns.LastKnownTime = Platform.Time;

                broker.Update(ServiceTools.localServerInformation.GetKey(), columns);

                context.Commit();
            }
        }

        public override string GetDisplayName()
        {
            return "ServerInfo Shred";
        }

        public override string GetDescription()
        {
            return "Updates server information";
        }
    }
}