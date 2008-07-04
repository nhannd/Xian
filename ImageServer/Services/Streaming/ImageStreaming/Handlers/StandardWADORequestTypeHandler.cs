using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{

    [ExtensionOf(typeof(WADOHandlerManagerExtensionPoint))]
    public class StandardWADORequestTypeHandler : IWADORequestTypeHandler
    {

        private Dictionary<string, List<IWADOMimeTypeHandler>> _mimeTypeHandlers =
            new Dictionary<string, List<IWADOMimeTypeHandler>>();

        public StandardWADORequestTypeHandler()
        {
            WADOMimeTypeExtensionPoint xp = new WADOMimeTypeExtensionPoint();
            object[] plugins = xp.CreateExtensions();
            foreach (object plugin in plugins)
            {
                if (plugin is IWADOMimeTypeHandler)
                {
                    IWADOMimeTypeHandler handler = plugin as IWADOMimeTypeHandler;
                    foreach(string mime in handler.SupportedRequestedMimeTypes)
                    {
                        if (!_mimeTypeHandlers.ContainsKey(mime))
                        {
                            _mimeTypeHandlers.Add(mime, new List<IWADOMimeTypeHandler>());
                        }
                        _mimeTypeHandlers[mime].Add(handler);
                    }
                    
                }
            }
        }

        private IWADOMimeTypeHandler ResolveMimeTypeHandler(HttpListenerRequest request)
        {
            string[] requestMimeTypes = request.AcceptTypes;

            if (requestMimeTypes == null)
                requestMimeTypes = new string[] {"*/*"};

            // match the mime type
            foreach(string rawMime in requestMimeTypes)
            {
                // ignore the q-value
                string mime = rawMime.IndexOf(";")>=0? rawMime.Substring(0, rawMime.IndexOf(';')) : rawMime;
                if (_mimeTypeHandlers.ContainsKey(mime))
                {
                    // return first one in the list
                    return _mimeTypeHandlers[mime][0];
                }
            }

            StringBuilder mimes = new StringBuilder();
            foreach (string mime in requestMimeTypes)
            {
                if (mimes.Length > 0)
                    mimes.Append(",");
                mimes.Append(mime);
            }

            throw new WADOException((int)HttpStatusCode.UnsupportedMediaType,
                    String.Format("Unsupported mime types: {0}", mimes));
        }

        private IWADOMimeTypeHandler GetMimeTypeHandler(HttpListenerRequest request)
        {
            return ResolveMimeTypeHandler(request);
        }

        #region IWADORequestTypeHandler Members

        public string RequestType
        {
            get { return "WADO"; }
        }

        public void Validate(HttpListenerRequest request)
        {
            string studyUid = request.QueryString["studyUID"];
            string seriesUid = request.QueryString["seriesUid"];
            string objectUid = request.QueryString["objectUid"];

            if (String.IsNullOrEmpty(studyUid))
            {
                throw new WADOException((int)HttpStatusCode.BadRequest, String.Format("studyUID is not specified"));
            }

            if (String.IsNullOrEmpty(seriesUid))
            {
                throw new WADOException((int)HttpStatusCode.BadRequest, String.Format("seriesUid is not specified"));
                
            }

            if (String.IsNullOrEmpty(objectUid))
            {
                throw new WADOException((int)HttpStatusCode.BadRequest, String.Format("objectUid is not specified"));
            }

            IWADOMimeTypeHandler handler = GetMimeTypeHandler(request);
        }
        
        #endregion

        #region IWADORequestTypeHandler Members

        public WADOResponse Process(HttpListenerRequest request)
        {
            Validate(request);
            IWADOMimeTypeHandler handler = GetMimeTypeHandler(request);
            return handler.GetResponseContent(request);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion
    }
}
