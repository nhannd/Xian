#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
