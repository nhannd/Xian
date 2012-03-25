#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Net;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;
using DicomServerConfigurationContract = ClearCanvas.ImageViewer.Common.DicomServer.DicomServerConfiguration;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
{
    // TODO (Marmot): This is probably not the best place for this, but not sure where else to put it right now.

    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    [ExtensionOf(typeof(DuplexServiceProviderExtensionPoint))]
    internal class DicomServerConfigurationServiceProvider : IServiceProvider, IDuplexServiceProvider
    {
        #region IDuplexServiceProvider Members

        public object GetService(Type serviceType, object callback)
        {
            if (serviceType == typeof(IDicomServerConfiguration))
                //return new DicomServerConfigurationProxy(new DicomServerConfiguration((IDicomServerConfigurationCallback)callback));
                return new DicomServerConfigurationProxy(new DicomServerConfiguration());

            return null;
        }

        #endregion

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
    internal class DicomServerConfigurationProxy : IDicomServerConfiguration, IDisposable
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

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (_real == null)
                return;

            _real.Dispose();
            _real = null;
        }

        #endregion
    }

    internal partial class DicomServerConfiguration : IDicomServerConfiguration, IDisposable
    {
        // TODO (Marmot): How to deal with this? Settings?
        private const string _defaultServerAE = "AETITLE";
        private const int _defaultPort = 104;

        private const string _configurationKey = "DicomServer";

        //private IDicomServerConfigurationCallback _callback;

        public DicomServerConfiguration()
        {
            //_callback = callback;
            //if (_callback != null)
            //    ChangePublisher.Instance.ConfigurationChanged += OnConfigurationChanged;
        }

        //private void OnConfigurationChanged(object sender, EventArgs e)
        //{
        //    _callback.ConfigurationChanged();
        //}

        private string DefaultAE
        {
            get { return _defaultServerAE; }
        }

        private int DefaultPort
        {
            get { return _defaultPort; }
        }

        #region Implementation of IDicomServerConfiguration

        public GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request)
        {
            DicomServerConfigurationContract configuration;
            using (var scope = new DataAccessScope())
            {
                configuration = scope.GetConfigurationBroker().GetDataContractValue<DicomServerConfigurationContract>(_configurationKey);
                if (configuration == null)
                    configuration = new DicomServerConfigurationContract
                    {
                        AETitle = DefaultAE,
                        HostName = Dns.GetHostName(),
                        Port = DefaultPort
                    };
            }

            return new GetDicomServerConfigurationResult { Configuration = configuration };
        }

        public UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request)
        {
            using (var scope = new DataAccessScope())
            {
                scope.GetConfigurationBroker().SetDataContractValue(_configurationKey, request.Configuration);
            }

            return new UpdateDicomServerConfigurationResult();
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            //if (_callback == null)
            //    return;

            //ChangePublisher.Instance.ConfigurationChanged -= OnConfigurationChanged;
            //_callback = null;
        }

        #endregion
    }
}
