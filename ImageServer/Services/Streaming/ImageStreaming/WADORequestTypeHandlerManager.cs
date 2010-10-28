#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
