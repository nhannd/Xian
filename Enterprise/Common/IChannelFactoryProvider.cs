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
