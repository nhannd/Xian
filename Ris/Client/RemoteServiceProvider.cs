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
using System.Reflection;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension of the <see cref="ServiceProviderExtensionPoint"/> that allows the client to obtain RIS application
    /// services.
    /// </summary>
    [ExtensionOf(typeof(ClearCanvas.Common.ServiceProviderExtensionPoint))]
    public class RemoteServiceProvider : IServiceProvider
    {
        public delegate object CreateChannelDelegate();

        private static readonly Dictionary<Type, CreateChannelDelegate> _channelFactoryMethods = new Dictionary<Type, CreateChannelDelegate>();
        private const int OneMegaByte = 1048576;

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            try
            {
                AuthenticationAttribute authAttr = AttributeUtils.GetAttribute<AuthenticationAttribute>(serviceType);
                bool authenticationRequired = authAttr == null ? true : authAttr.AuthenticationRequired;
                if (authenticationRequired && LoginSession.Current == null)
                    throw new InvalidOperationException("User login credentials have not been provided.");

                // obtain the channel factory method for this service
                CreateChannelDelegate factoryMethod;
                if(!_channelFactoryMethods.TryGetValue(serviceType, out factoryMethod))
                {
                    lock (_channelFactoryMethods)
                    {
                        // don't need double-checked lock pattern here because who cares if multiple threads create the same channel
                        _channelFactoryMethods.Add(serviceType,
                                               factoryMethod = CreateChannelFactory(serviceType, authenticationRequired));
                    }
                }

                // call the factory method to create the channel
                object channel = factoryMethod();

                Platform.Log(LogLevel.Debug, "Created WCF channel instance for service {0}, authenticationRequired={1}.",
                             serviceType.FullName, authenticationRequired);

                return channel;
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

        private static CreateChannelDelegate CreateChannelFactory(Type serviceType, bool authenticationRequired)
        {
            string baseUrl = WebServicesSettings.Default.ApplicationServicesBaseUrl;

            Uri uri = new Uri(new Uri(baseUrl), serviceType.FullName);
            EndpointAddress endpoint = new EndpointAddress(uri);
            WSHttpBinding binding = new WSHttpBinding();
            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Message.ClientCredentialType =
                authenticationRequired ? MessageCredentialType.UserName : MessageCredentialType.None;
            binding.MaxReceivedMessageSize = OneMegaByte;

            // allow individual string content to be same size as entire message
            binding.ReaderQuotas.MaxStringContentLength = OneMegaByte;
            binding.ReaderQuotas.MaxArrayLength = OneMegaByte;

            //binding.ReceiveTimeout = new TimeSpan(0, 0 , 20);
            //binding.SendTimeout = new TimeSpan(0, 0, 10);

            // create the channel factory
            Type channelFactoryClass = typeof(ChannelFactory<>).MakeGenericType(new Type[] { serviceType });
            ChannelFactory channelFactory = (ChannelFactory)Activator.CreateInstance(channelFactoryClass, binding, endpoint);
            channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.PeerOrChainTrust;
            if (authenticationRequired)
            {
                channelFactory.Credentials.UserName.UserName = LoginSession.Current.UserName;

                // use session token in place of password
                channelFactory.Credentials.UserName.Password = LoginSession.Current.SessionToken;
            }

            // create a delegate and bind it to the channelFactory instance
            MethodInfo createChannelMethod = channelFactory.GetType().GetMethod("CreateChannel", Type.EmptyTypes);
            CreateChannelDelegate createChannel = (CreateChannelDelegate)
                Delegate.CreateDelegate(typeof(CreateChannelDelegate), channelFactory, createChannelMethod);
            return createChannel;
        }
    }
}
