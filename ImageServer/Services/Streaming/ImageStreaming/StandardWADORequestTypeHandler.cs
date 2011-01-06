#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    /// <summary>
    /// Represents handler that handles requests with RequestType of "WADO"
    /// </summary>
    [ExtensionOf(typeof(WADORequestTypeExtensionPoint))]
    class StandardWADORequestTypeHandler : IWADORequestTypeHandler
    {

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
                throw new WADOException(HttpStatusCode.BadRequest, String.Format("studyUID parameter is required"));
            }

            if (String.IsNullOrEmpty(seriesUid))
            {
                throw new WADOException(HttpStatusCode.BadRequest, String.Format("seriesUid parameter is required"));
                
            }

            if (String.IsNullOrEmpty(objectUid))
            {
                throw new WADOException(HttpStatusCode.BadRequest, String.Format("objectUid parameter is required"));
            }
        }
        
        #endregion

        #region IWADORequestTypeHandler Members


        #endregion

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion

        #region IWADORequestTypeHandler Members


        public WADOResponse Process(WADORequestTypeHandlerContext context)
        {
            //Validate(context.HttpContext.Request);

            ObjectStreamingHandlerFactory factory = new ObjectStreamingHandlerFactory();
            IObjectStreamingHandler handler = factory.CreateHandler(context.HttpContext.Request);
            return handler.Process(context);
        }

        #endregion
    }
}
