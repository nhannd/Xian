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
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Defines an interface to an object that provides channel factories for
    /// remote services.
    /// </summary>
    /// <remarks>
    /// This interface is consumed by <see cref="RemoteServiceProviderBase"/>, 
    /// which must obtain channel factories for the services it provides.  
    /// </remarks>
    public interface IChannelFactoryProvider
    {
        /// <summary>
        /// Gets the primary channel factory for the specified service contract.
        /// </summary>
        /// <remarks>
        /// The service provider will call <see cref="GetPrimary"/> every time an instance
        /// of a given service is requested.  The implementation is free to return a 
        /// different channel as the "primary" channel for any given call, in order to 
        /// achieve a load-balancing scheme if desired.
        /// </remarks>
        /// <param name="serviceContract"></param>
        /// <returns></returns>
        ChannelFactory GetPrimary(Type serviceContract);

        /// <summary>
        /// Attempts to obtain an alternate channel factory for the specified service
        /// contract, in the event that the primary channel endpoint is unreachable.
        /// </summary>
        /// <remarks>
        /// This method should only be called if the primary channel is unreachable.
        /// It may be called multiple times, in the event that a returned failover
        /// channel is also unreachable.  With each successive call, the caller must
        /// provide the endpoint address of the failed channel, so that the implementation
        /// can track which channels have failed and avoid returning the same channel
        /// repeatedly.
        /// </remarks>
        /// <param name="serviceContract"></param>
        /// <param name="failedEndpoint"></param>
        /// <returns>An alternate channel factory, or null if no alternate is available.</returns>
        ChannelFactory GetFailover(Type serviceContract, EndpointAddress failedEndpoint);
    }
}
