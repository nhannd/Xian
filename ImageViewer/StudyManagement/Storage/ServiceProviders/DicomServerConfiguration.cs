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
using System.Net;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;
using DicomServerConfigurationContract = ClearCanvas.ImageViewer.Common.DicomServer.DicomServerConfiguration;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
{
    // TODO (Marmot): This seems like the best place for this, since it has to be available (in process) whenever the database is present.
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class DicomServerConfigurationServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDicomServerConfiguration))
                return new DicomServerConfigurationProxy(new DicomServerConfiguration());

            return null;
        }

        #endregion
    }

    // TODO (Marmot): Later, maybe implement something a little more robust that can convert an exception
    // to the correct Fault contract object for "in process" services.
    internal class DicomServerConfigurationProxy : IDicomServerConfiguration
    {
        private DicomServerConfiguration _real;

        public DicomServerConfigurationProxy(DicomServerConfiguration real)
        {
            _real = real;
        }

        #region Implementation of IDicomServerConfiguration

        public GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request)
        {
            Platform.CheckForNullReference(request, "request");

            try
            {
                return _real.GetConfiguration(request);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                throw new FaultException();
            }
        }

        public UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckForNullReference(request.Configuration, "request.Configuration");

            try
            {
                return _real.UpdateConfiguration(request);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                throw new FaultException();
            }
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
            get { return Dns.GetHostName(); }
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
                Platform.GetService<IDicomServerService>(s => s.RestartListener(new RestartListenerRequest()));
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
