#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.ServiceModel;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;
using DicomServerConfigurationContract = ClearCanvas.ImageViewer.Common.DicomServer.DicomServerConfiguration;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class DicomServerConfigurationServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof (IDicomServerConfiguration))
                return null;

            return new ProxyGenerator().CreateInterfaceProxyWithTargetInterface(
                typeof (IDicomServerConfiguration), new DicomServerConfiguration()
                , new IInterceptor[] {new BasicFaultInterceptor()});
        }

        #endregion
    }

    internal class DicomServerConfiguration : IDicomServerConfiguration
    {
        private static readonly string _configurationKey = typeof(DicomServerConfigurationContract).FullName;

        //Note: the installer is supposed to set these defaults. These are the bottom of the barrel, last-ditch defaults.
        #region Defaults

        private string DefaultAE
        {
            get { return "AETITLE"; }
        }

        private string DefaultHostname
        {
            get { return "localhost"; }
        }

        private string DefaultFileStoreLocation
        {
            get { return Path.Combine(Platform.ApplicationDataDirectory, "filestore"); }
        }

        private int DefaultPort
        {
            get { return 104; }
        }

        #endregion

        #region Implementation of IDicomServerConfiguration

        public GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request)
        {
            DicomServerConfigurationContract configuration;
            using (var context = new DataAccessContext())
            {
                var broker = context.GetConfigurationBroker();
                configuration = broker.GetDataContractValue<DicomServerConfigurationContract>(_configurationKey)
                                ?? new DicomServerConfigurationContract { Port = DefaultPort };
            }

            if (String.IsNullOrEmpty(configuration.AETitle))
                configuration.AETitle = DefaultAE;
            if (String.IsNullOrEmpty(configuration.HostName))
                configuration.HostName = DefaultHostname;
            if (String.IsNullOrEmpty(configuration.FileStoreDirectory))
                configuration.FileStoreDirectory = DefaultFileStoreLocation;

            return new GetDicomServerConfigurationResult { Configuration = configuration };
        }

        public UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request)
        {
            Platform.CheckForEmptyString(request.Configuration.AETitle, "AETitle");
            Platform.CheckArgumentRange(request.Configuration.Port, 1, 65535, "Port");

            //Trim the strings before saving.
            request.Configuration.AETitle = request.Configuration.AETitle.Trim();
            request.Configuration.FileStoreDirectory = request.Configuration.FileStoreDirectory.Trim();
            request.Configuration.HostName = request.Configuration.HostName.Trim();

            using (var context = new DataAccessContext())
            {
                context.GetConfigurationBroker().SetDataContractValue(_configurationKey, request.Configuration);
                context.Commit();
            }

            //TODO (Marmot): While it doesn't do any harm to do this here, the listener should also poll periodically for configuration changes, just in case.
            try
            {
                DicomServer.RestartListener();
            }
            catch (EndpointNotFoundException)
            {
            }
            catch(Exception e)
            {
                Platform.Log(LogLevel.Warn, e, "Failed to restart the DICOM Server.");
                throw;
            }

            return new UpdateDicomServerConfigurationResult();
        }

        #endregion
    }
}
