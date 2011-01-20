#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Web;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Common.Modules
{
    class HttpContextDataManagementModule: IHttpModule
    {
        private HttpApplication _application;

        #region IHttpModule Members

        public void Dispose()
        {
            if (_application!=null)
            {
                try
                {
                    _application.EndRequest -= context_EndRequest;
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error,e);
                }
            }
        }

        public void Init(HttpApplication context)
        {
            _application = context;
            context.EndRequest += new EventHandler(context_EndRequest);
            
        }

        static void context_EndRequest(object sender, EventArgs e)
        {
            if (HttpContextData.Current!=null)
            {
                HttpContextData.Current.Dispose();
            } 
        }

        #endregion
    }
}
