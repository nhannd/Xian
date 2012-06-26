using System;
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint), Enabled = true)]
    internal class DicomServerConfigurationServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof(IDicomServerConfiguration))
                return null;

            return new DicomServerConfigurationProxy();
        }

        #endregion
    }

    internal class DicomServerConfigurationProxy : IDicomServerConfiguration
    {
        #region IDicomServerConfiguration Members

        public GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request)
        {
            var settings = new DicomServerSettings();
            return new GetDicomServerConfigurationResult
            {
                Configuration = new DicomServerConfiguration
                {
                    AETitle = settings.AETitle,
                    HostName = settings.HostName,
                    Port = settings.Port
                }
            };
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

            var settings = new DicomServerSettings().GetProxy();
            settings.AETitle = request.Configuration.AETitle;
            settings.HostName = request.Configuration.HostName;
            settings.Port = request.Configuration.Port;

            //TODO (Marmot): While it doesn't do any harm to do this here, the listener should also poll periodically for configuration changes, just in case.
            try
            {
                DicomServer.RestartListener();
            }
            catch (EndpointNotFoundException)
            {
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e, "Failed to restart the DICOM Server listener.");
                throw;
            }

            return new UpdateDicomServerConfigurationResult();
        }

        #endregion
    }
}