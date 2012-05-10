#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using DicomServerConfigurationContract = ClearCanvas.ImageViewer.Common.DicomServer.DicomServerConfiguration;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.ServiceProviders
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class DicomServerConfigurationServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof (IDicomServerConfiguration))
                return null;

            return new DicomServerConfigurationProxy();
        }

        #endregion
    }

    internal class DicomServerConfigurationProxy : IDicomServerConfiguration
    {
        private readonly IDicomServerConfiguration _real;

        public DicomServerConfigurationProxy()
        {
            _real = new DicomServerConfiguration();
        }

        public GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request)
        {
            return ServiceProxyHelper.Call(_real.GetConfiguration, request);
        }

        public UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request)
        {
            return ServiceProxyHelper.Call(_real.UpdateConfiguration, request);
        }
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

        private int DefaultPort
        {
            get { return 104; }
        }

        #endregion

        #region Implementation of IDicomServerConfiguration

        public GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request)
        {
            Platform.CheckForNullReference(request, "request");

            var cachedValue = Cache<DicomServerConfigurationContract>.CachedValue;
            if (cachedValue != null)
                return new GetDicomServerConfigurationResult { Configuration = cachedValue.Clone() };

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

            Cache<DicomServerConfigurationContract>.CachedValue = configuration;
            return new GetDicomServerConfigurationResult { Configuration = configuration.Clone() };
        }

        public UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckForNullReference(request.Configuration, "Configuration");
            Platform.CheckForEmptyString(request.Configuration.AETitle, "AETitle");
            Platform.CheckArgumentRange(request.Configuration.Port, 1, 65535, "Port");

            //Trim the strings before saving.
            request.Configuration.AETitle = request.Configuration.AETitle.Trim();
            if (!String.IsNullOrEmpty(request.Configuration.HostName))
                request.Configuration.HostName = request.Configuration.HostName.Trim();

            using (var context = new DataAccessContext())
            {
                context.GetConfigurationBroker().SetDataContractValue(_configurationKey, request.Configuration);
                context.Commit();
                
                //Make a copy because the one in the request is a reference object that the caller could change afterwards.
                Cache<DicomServerConfigurationContract>.CachedValue = request.Configuration.Clone();
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

    internal static class DicomServerConfigurationExtensions
    {
        public static DicomServerConfigurationContract Clone(this DicomServerConfigurationContract dicomServerConfiguration)
        {
            return new DicomServerConfigurationContract
            {
                AETitle = dicomServerConfiguration.AETitle,
                HostName = dicomServerConfiguration.HostName,
                Port = dicomServerConfiguration.Port
            };
        }
    }
}
