using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    [ExtensionPoint()]
    public class WADOHandlerManagerExtensionPoint : ExtensionPoint<IWADORequestTypeHandler>
    {

    }


    public class WADORequestTypeHandlerManager
    {
        private Dictionary<string, IWADORequestTypeHandler> _handlers = new Dictionary<string, IWADORequestTypeHandler>();

        public WADORequestTypeHandlerManager()
        {
            LoadHandlers();
        }

        public IWADORequestTypeHandler GetHandler(string requestType)
        {
            if (_handlers.ContainsKey(requestType))
                return _handlers[requestType];
            else
                throw new WADOException((int)HttpStatusCode.BadRequest, String.Format("Unsupported RequestType {0}", requestType));
        
        }

        

        private void LoadHandlers()
        {
            WADOHandlerManagerExtensionPoint xp = new WADOHandlerManagerExtensionPoint();
            object[] plugins = xp.CreateExtensions();
            foreach (object plugin in plugins)
            {
                if (plugin is IWADORequestTypeHandler)
                {
                    IWADORequestTypeHandler typeHandler = plugin as IWADORequestTypeHandler;
                    _handlers.Add(typeHandler.RequestType, typeHandler);
                }
            }
        }

    }
}
