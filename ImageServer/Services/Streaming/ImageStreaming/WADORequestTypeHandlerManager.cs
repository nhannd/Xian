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
using System.Net;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    /// <summary>
    /// Manages life-time of request type plugins.
    /// </summary>
    /// <remarks>
    /// <see cref="WADORequestTypeHandlerManager"/> instantiates and cleans up resources held by plugins that implements <see cref="IWADORequestTypeHandler"/>.
    /// When <see cref="WADORequestTypeHandlerManager"/> is <see cref="Dispose">disposed</see> the plugin instances held are also disposed of. 
    /// </remarks>
    class WADORequestTypeHandlerManager : IDisposable
    {
        #region Private Members
        private Dictionary<string, IWADORequestTypeHandler> _handlers = new Dictionary<string, IWADORequestTypeHandler>();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="WADORequestTypeHandlerManager"/>
        /// </summary>
        public WADORequestTypeHandlerManager()
        {
            LoadHandlers();
        }
        #endregion

        #region Public Methods

        public IWADORequestTypeHandler GetHandler(string requestType)
        {
            String type = requestType.ToUpper();
            if (_handlers.ContainsKey(type))
                return _handlers[type];
            else
                throw new WADOException(HttpStatusCode.BadRequest, String.Format("Unsupported RequestType {0}", requestType));

        }

        #endregion

        #region Private Methods

        private void LoadHandlers()
        {
            WADORequestTypeExtensionPoint xp = new WADORequestTypeExtensionPoint();
            object[] plugins = xp.CreateExtensions();
            foreach (object plugin in plugins)
            {
                if (plugin is IWADORequestTypeHandler)
                {
                    IWADORequestTypeHandler typeHandler = plugin as IWADORequestTypeHandler;
                    _handlers.Add(typeHandler.RequestType.ToUpper(), typeHandler);
                }
            }
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            foreach (IWADORequestTypeHandler plugin in _handlers.Values)
            {
                plugin.Dispose();
            }
        }

        #endregion
    }
}
