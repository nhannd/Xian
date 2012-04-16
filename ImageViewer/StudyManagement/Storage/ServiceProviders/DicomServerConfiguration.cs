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
        // TODO (Marmot): How to deal with this? Maybe figure it out from the host name? 
        private const string _defaultServerAE = "AETITLE";
        private const int _defaultPort = 104;

        private static readonly string _configurationKey = typeof(DicomServerConfigurationContract).FullName;

        private string DefaultAE
        {
            //TODO (Marmot): Do something smarter, like determine it from the host name?
            get { return _defaultServerAE; }
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
            get { return _defaultPort; }
        }

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

            using (var context = new DataAccessContext())
            {
                context.GetConfigurationBroker().SetDataContractValue(_configurationKey, request.Configuration);
                context.Commit();
            }

            //TODO (Marmot): This really the right place to do this? Guess it does no harm, but perhaps not
            //obvious that this will happen automatically.
            try
            {
                Platform.GetService<IDicomServer>(s => s.RestartListener(new RestartListenerRequest()));
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
