#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension of the <see cref="ServiceProviderExtensionPoint"/> that allows the client to obtain RIS application
    /// services.
    /// </summary>
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    public class RisServiceProvider : RemoteServiceProviderBase<RisServiceProviderAttribute>
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
