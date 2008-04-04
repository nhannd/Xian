using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension of the <see cref="ServiceProviderExtensionPoint"/> that allows the client to obtain core services
    /// services.
    /// </summary>
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    public class CoreServiceProvider : RemoteServiceProviderBase<CoreServiceProviderAttribute>
    {
        private bool _authenticationRequired = false;

        protected override string ApplicationServicesBaseUrl
        {
            get { return WebServicesSettings.Default.ApplicationServicesBaseUrl; }
        }

        protected override bool AuthenticationRequired(Type serviceType)
        {
            AuthenticationAttribute authAttr = AttributeUtils.GetAttribute<AuthenticationAttribute>(serviceType);
            _authenticationRequired = authAttr == null ? true : authAttr.AuthenticationRequired;
            return _authenticationRequired;
        }

        protected override void ValidateAuthentification()
        {
            if (LoginSession.Current == null)
                throw new InvalidOperationException("User login credentials have not been provided.");
        }

        protected override void GetValidationInfo(out string userName, out string password)
        {
            userName = LoginSession.Current.UserName;

            // use session token in place of password
            password = LoginSession.Current.SessionToken;
        }

        protected override Binding GetBinding()
        {
            WSHttpBinding binding = new WSHttpBinding();
            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Message.ClientCredentialType =
                _authenticationRequired ? MessageCredentialType.UserName : MessageCredentialType.None;
            binding.MaxReceivedMessageSize = OneMegaByte;

            // allow individual string content to be same size as entire message
            binding.ReaderQuotas.MaxStringContentLength = OneMegaByte;
            binding.ReaderQuotas.MaxArrayLength = OneMegaByte;

            //binding.ReceiveTimeout = new TimeSpan(0, 0 , 20);
            //binding.SendTimeout = new TimeSpan(0, 0, 10);

            return binding;
        }
    }
}
