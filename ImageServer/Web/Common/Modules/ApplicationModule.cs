#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
    public class ApplicationModule : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
            
        }

        public void Init(HttpApplication context)
        {
            Platform.PluginManager.PluginLoaded += new EventHandler<PluginLoadedEventArgs>(PluginManager_PluginLoaded);
        }

        #endregion

        void PluginManager_PluginLoaded(object sender, PluginLoadedEventArgs e)
        {
            Platform.Log(LogLevel.Info, e.Message);
        }
    }
}
