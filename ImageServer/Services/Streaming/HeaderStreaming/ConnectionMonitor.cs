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
using System.ServiceModel;
using System.ServiceModel.Description;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming
{
    class ConnectionMonitor
    {
        #region Private Members
        private readonly List<InstanceContext> _contexts = new List<InstanceContext>();

        private readonly ServiceHostBase _host;

        private readonly int _maxConnections;
        private static readonly Dictionary<ServiceHostBase, ConnectionMonitor> _hostList = new Dictionary<ServiceHostBase, ConnectionMonitor>();
            
        #endregion


        #region Public Static Methods
        /// <summary>
        /// Gets an instance of <see cref="ConnectionMonitor"/> for the specified service host.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static ConnectionMonitor GetMonitor(ServiceHostBase host)
        {
            lock (_hostList)
            {
                if (_hostList.ContainsKey(host))
                {
                    return _hostList[host];
                }
                else
                {

                    _hostList.Add(host, new ConnectionMonitor(host));
                    return _hostList[host];
                }
            }
            
        }
        #endregion

        #region Constructors
        /// <summary>
        /// ****** Internal use only. Use <see cref="GetMonitor"/> instead.
        /// </summary>
        /// <param name="host"></param>
        private ConnectionMonitor(ServiceHostBase host)
        {
            _host = host;

            ServiceThrottlingBehavior behaviour;
            if (_host.Description.Behaviors.Contains(typeof(ServiceThrottlingBehavior)))
            {
                behaviour = (ServiceThrottlingBehavior)_host.Description.Behaviors[typeof(ServiceThrottlingBehavior)];
            }
            else
            {
                behaviour = new ServiceThrottlingBehavior();
            }

            // Assume we are using PerCall for InstanceContext mode,
            // each time a client requests for study header, a new InstanceContext is created.
            // Max connections = min (MaxConcurrentCalls, MaxConcurrentInstances)
            // MaxConcurrentSessions has no effect
            _maxConnections = Math.Min(behaviour.MaxConcurrentCalls, behaviour.MaxConcurrentInstances);
            
        }
        #endregion

        #region Public Methods
        
        /// <summary>
        /// Adds the specified <see cref="OperationContext"/>
        /// </summary>
        /// <param name="context"></param>
        public void AddContext(OperationContext context)
        {
            lock (_contexts)
            {
                InstanceContext ctx = context.InstanceContext;
                if (!_contexts.Contains(ctx))
                {
                    ctx.Closed += Context_Closed;
                    ctx.Faulted += Context_Faulted;
                    _contexts.Add(ctx);
                }

                if (_contexts.Count == _maxConnections)
                {
                    Platform.Log(LogLevel.Warn, "Max concurrency reached: {0}. Max={1}", _contexts.Count, _maxConnections); 

                    ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Warning, "Header Streaming",
                                         AlertTypeCodes.LowResources, null, TimeSpan.FromSeconds(15),
                                         SR.AlertHeaderMaxConnectionsReached, _maxConnections);
                }
            }
        }

        #endregion

        #region Private Methods


        private void Context_Faulted(object sender, EventArgs e)
        {
            lock (_contexts)
            {
                InstanceContext ctx = sender as InstanceContext;
                if (ctx != null)
                {
                    ctx.Faulted -= Context_Faulted;
                    if (_contexts.Contains(ctx))
                    {
                        _contexts.Remove(ctx);
                    }
                }

            }
        }

        private void Context_Closed(object sender, EventArgs e)
        {
            lock (_contexts)
            {
                InstanceContext ctx = sender as InstanceContext;
                if (ctx!=null)
                {
                    ctx.Closed -= Context_Closed;
                    if (_contexts.Contains(ctx))
                    {
                        _contexts.Remove(ctx);
                    }
                }

                Platform.Log(LogLevel.Debug, "Context closed detected: # concurrent connections: {0}", _contexts.Count);
            }
        }
        #endregion

    }
}