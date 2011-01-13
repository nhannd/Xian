#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageServer.Services.Common.Shreds
{
    /// <summary>
    /// Plugin to host ImageServer-specific web services.
    /// </summary>
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class RemoteServicesServer : WcfShred
    {

        #region Private Members

        private readonly string _className;
        private ServiceMount _serviceMount;

        #endregion

        #region Constructors

        public RemoteServicesServer()
        {
            _className = GetType().ToString();
        }

        #endregion

        #region IShred Implementation Shred Override

        public override void Start()
        {
            Platform.Log(LogLevel.Debug, "{0}[{1}]: Start invoked", _className, AppDomain.CurrentDomain.FriendlyName);

            try
            {
                MountWebServices();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Fatal, e, "Unexpected exception starting Web Services Server Shred");
            }
        }

        private void MountWebServices()
        {
            _serviceMount = new ServiceMount(new Uri(WebServicesSettings.Default.BaseUri), typeof(ServerWsHttpConfiguration).AssemblyQualifiedName);
            _serviceMount.AddServices(new ApplicationServiceExtensionPoint());
            _serviceMount.OpenServices();
        }

        public override void Stop()
        {
            Platform.Log(LogLevel.Info, "{0}[{1}]: Stop invoked", _className, AppDomain.CurrentDomain.FriendlyName);
            if (_serviceMount!=null)
                _serviceMount.CloseServices();
        }

        public override string GetDisplayName()
        {
            return "Remote Services Server";
        }

        public override string GetDescription()
        {
            return "Provide remote services to clients.";
        }

        #endregion
    }
}
