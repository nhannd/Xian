#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Arguments for configuration of a service channel.
	/// </summary>
    public struct ServiceChannelConfigurationArgs
    {
        public ServiceChannelConfigurationArgs(
            Type channelFactoryClass,
            Uri serviceUri,
            bool authenticationRequired,
            int maxReceivedMessageSize,
            X509CertificateValidationMode certificateValidationMode,
            X509RevocationMode revocationMode)
        {
            this.ChannelFactoryClass = channelFactoryClass;
            this.ServiceUri = serviceUri;
            this.AuthenticationRequired = authenticationRequired;
            this.MaxReceivedMessageSize = maxReceivedMessageSize;
            this.CertificateValidationMode = certificateValidationMode;
            this.RevocationMode = revocationMode;
        }

		/// <summary>
		/// The class of the channel factory to create.
		/// </summary>
        public Type ChannelFactoryClass;

		/// <summary>
		/// The URI on which the service is hosted.
		/// </summary>
        public Uri ServiceUri;

		/// <summary>
		/// A value indicating whether the service requires authentication.
		/// </summary>
        public bool AuthenticationRequired;

		/// <summary>
		/// The maximum size of received messages to allow, in bytes.
		/// </summary>
        public int MaxReceivedMessageSize;

        /// <summary>
        /// Specifies the mode used for X509 certificate validation.
        /// </summary>
        public X509CertificateValidationMode CertificateValidationMode;

        /// <summary>
        /// Specifies the mode used to check for X509 certificate revocation.
        /// </summary>
        public X509RevocationMode RevocationMode;
    }
}
