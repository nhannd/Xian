#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;

namespace ClearCanvas.Common.UsageTracking
{
    /// <summary>
    /// Usage tracking service.
    /// </summary>
    [ServiceContract]
    public interface IUsageTracking
    {
        /// <summary>
        /// Register the startup of an application with the tracking service.
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>Returns a response, which may include a message to be displayed.</returns>
        [OperationContract]
        RegisterResponse Register(RegisterRequest request);
    }
}
