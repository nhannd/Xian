using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Selectors;
using System.IdentityModel.Policy;

namespace ClearCanvas.Enterprise.Common
{
    public struct ServiceHostConfigurationArgs
    {
        public ServiceHostConfigurationArgs(Type serviceContract, Uri hostUri, bool authenticated,
			int maxReceivedMessageSize, UserNamePasswordValidator userNamePasswordValidator, IAuthorizationPolicy authorizationPolicy)
        {
            ServiceContract = serviceContract;
            HostUri = hostUri;
            Authenticated = authenticated;
            MaxReceivedMessageSize = maxReceivedMessageSize;
			UserNamePasswordValidator = userNamePasswordValidator;
        	AuthorizationPolicy = authorizationPolicy;
        }

        public Type ServiceContract;
        public Uri HostUri;
        public bool Authenticated;
        public int MaxReceivedMessageSize;
    	public UserNamePasswordValidator UserNamePasswordValidator;
    	public IAuthorizationPolicy AuthorizationPolicy;
    }
}
