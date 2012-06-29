using System;
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
            return new GetDicomServerConfigurationResult { Configuration = settings.GetBasicConfiguration() };
        }

        public UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            new DicomServerSettings().UpdateBasicConfiguration(request.Configuration);
            return new UpdateDicomServerConfigurationResult();
        }

        public GetDicomServerExtendedConfigurationResult GetExtendedConfiguration(GetDicomServerExtendedConfigurationRequest request)
        {
            return new GetDicomServerExtendedConfigurationResult {ExtendedConfiguration = new DicomServerSettings().GetExtendedConfiguration()};
        }

        public UpdateDicomServerExtendedConfigurationResult UpdateExtendedConfiguration(UpdateDicomServerExtendedConfigurationRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            new DicomServerSettings().UpdateExtendedConfiguration(request.ExtendedConfiguration);
            return new UpdateDicomServerExtendedConfigurationResult();
        }

        #endregion
    }
}