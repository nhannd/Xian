#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.Common.UsageTracking
{
    /// <summary>
    /// Client service for <see cref="IUsageTracking"/>.
    /// </summary>
    internal class UsageTrackingServiceClient : ClientBase<IUsageTracking>
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public UsageTrackingServiceClient()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endpointConfigurationName">Endpoint configuration name.</param>
        public UsageTrackingServiceClient(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="binding">Binding for the service.</param>
        /// <param name="remoteAddress">Remote address.</param>
        public UsageTrackingServiceClient(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endpointConfigurationName">Binding configuration name.</param>
        /// <param name="remoteAddress">Remote address.</param>
        public UsageTrackingServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        #endregion Constructor

        #region IUsageTracking Members

        /// <summary>
        /// Register the application.
        /// </summary>
        /// <param name="request">The register request.</param>
        public RegisterResponse Register(RegisterRequest request)
        {
            return Channel.Register(request);
        }

        #endregion
    }
}
