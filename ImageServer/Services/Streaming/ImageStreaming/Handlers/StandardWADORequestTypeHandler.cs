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
using System.Collections.Generic;
using System.Net;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    /// <summary>
    /// Represents handler that handles requests with RequestType of "WADO"
    /// </summary>
    [ExtensionOf(typeof(WADORequestTypeExtensionPoint))]
    public class StandardWADORequestTypeHandler : IWADORequestTypeHandler
    {
        #region Private Members
        private readonly Dictionary<string, List<IWADOMimeTypeHandler>> _mimeTypeHandlers = new Dictionary<string, List<IWADOMimeTypeHandler>>();
        #endregion


        #region Constructors
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Looks up the internal mapping for a handler to process the specified request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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

            // No match. Let's throw exception.
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

        /// <summary>
        /// Returns the <see cref="IWADOMimeTypeHandler"/> that can process the specified request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private IWADOMimeTypeHandler GetMimeTypeHandler(HttpListenerRequest request)
        {
            return ResolveMimeTypeHandler(request);
        }

        #endregion

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
