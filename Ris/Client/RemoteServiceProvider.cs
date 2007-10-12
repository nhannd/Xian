#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Reflection;
using System.ServiceModel;

using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension of the <see cref="ServiceProviderExtensionPoint"/> that allows the client to obtain RIS application
    /// services.
    /// </summary>
    [ExtensionOf(typeof(ClearCanvas.Common.ServiceProviderExtensionPoint))]
    public class RemoteServiceProvider : IServiceProvider
    {
        private const int OneMegaByte = 1048576;

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            try
            {
                if (LoginSession.Current == null)
                    throw new InvalidOperationException("User login credentials have not been provided");

				Uri uri = new Uri(new Uri("http://localhost:8000"), serviceType.FullName);
                EndpointAddress endpoint = new EndpointAddress(uri);
                WSHttpBinding binding = new WSHttpBinding();
                binding.Security.Mode = SecurityMode.Message;
                binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
                binding.MaxReceivedMessageSize = OneMegaByte;
                
                // allow individual string content to be same size as entire message
                binding.ReaderQuotas.MaxStringContentLength = OneMegaByte;

                //binding.ReceiveTimeout = new TimeSpan(0, 0 , 20);
                //binding.SendTimeout = new TimeSpan(0, 0, 10);

                // create the channel factory
                Type channelFactoryClass = typeof(ChannelFactory<>).MakeGenericType(new Type[] { serviceType });
                ChannelFactory channelFactory = (ChannelFactory)Activator.CreateInstance(channelFactoryClass, binding, endpoint);
                channelFactory.Credentials.UserName.UserName = LoginSession.Current.UserName;
                channelFactory.Credentials.UserName.Password = LoginSession.Current.Password;
                channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.PeerOrChainTrust;

                // reflection is unfortunately the only way to create the service channel
                MethodInfo createChannelMethod = channelFactoryClass.GetMethod("CreateChannel", Type.EmptyTypes);
                object serviceProxy = createChannelMethod.Invoke(channelFactory, null);

                return serviceProxy;

            }
            catch (Exception e)
            {
                // in keeping with semantics of IServiceProvider, must return null here in order to give other
                // service providers the chance to provide the requested service
                // therefore, we just log the exception and don't rethrow it
                // (this is somewhat unfortunate, maybe we should not use the .NET IServiceProvider interface???)
                Platform.Log(LogLevel.Error, e);

                return null;
            }
        }

        #endregion
    }
}
