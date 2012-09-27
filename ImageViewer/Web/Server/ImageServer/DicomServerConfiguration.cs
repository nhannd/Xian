#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Web.Server.ImageServer
{
     
    [ExtensionOf(typeof(ServiceProviderExtensionPoint), Enabled = true)]
    internal class DicomServerConfigurationServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof(IDicomServerConfiguration))
                return null;

            return new DicomServerConfiguration();
        }

        #endregion
    }

    internal class DicomServerConfiguration : IDicomServerConfiguration
    {
        #region IDicomServerConfiguration Members

        public GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request)
        {
            return new GetDicomServerConfigurationResult
                       {
                           Configuration = new Common.DicomServer.DicomServerConfiguration
                                               {
                                                   //TODO (CR Sep 2012): Shouldn't we be getting a partition here?
                                                   //Maybe we can set the "main" partition for the application based
                                                   //on the aetitle passed in to start the viewer app?
                                                   AETitle = WebViewerServices.Default.AETitle,
                                                   HostName = WebViewerServices.Default.ArchiveServerHostname,
                                                   Port = WebViewerServices.Default.ArchiveServerPort
                                               }
                       };
        }

        public UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request)
        {
            return new UpdateDicomServerConfigurationResult();
        }

        public GetDicomServerExtendedConfigurationResult GetExtendedConfiguration(GetDicomServerExtendedConfigurationRequest request)
        {
            return new GetDicomServerExtendedConfigurationResult
                       {
                           ExtendedConfiguration = new DicomServerExtendedConfiguration()
                                                       {
                                                           AllowUnknownCaller = true
                                                       }
                       };
        }

        public UpdateDicomServerExtendedConfigurationResult UpdateExtendedConfiguration(UpdateDicomServerExtendedConfigurationRequest request)
        {            
            return new UpdateDicomServerExtendedConfigurationResult();
        }

        #endregion
    }
}
