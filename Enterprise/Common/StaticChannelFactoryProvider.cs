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
using System.ServiceModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Implementation of <see cref="IChannelFactoryProvider"/> that provides
    /// channel factories based on a single statically defined primary endpoint
    /// and a single statically defined failover endpoint.
    /// </summary>
    internal class StaticChannelFactoryProvider : IChannelFactoryProvider
    {
        #region Node class

		/// <summary>
		/// Represents a remote service instance.
		/// </summary>
        class Node
        {
            private readonly Uri _url;
            private DateTime _blackOutExpiry;

            public Node(string url)
            {
                _url = new Uri(url);
                _blackOutExpiry = DateTime.MinValue;
            }

            public Uri Url
            {
                get { return _url; }
            }

			/// <summary>
			/// Gets a value indicating whether the node is currently marked as offline.
			/// </summary>
            public bool IsBlackedOut
            {
                get { return _blackOutExpiry > DateTime.Now; }
            }

			/// <summary>
			/// Marks the node as offline for the specified timeout period.
			/// No further attempt to contact the node will be made until the offline timeout expires.
			/// </summary>
			/// <param name="timeout"></param>
            public void Blackout(TimeSpan timeout)
            {
                _blackOutExpiry = DateTime.Now + timeout;
            }
        }

        #endregion

        private readonly TimeSpan _blackoutPeriod = TimeSpan.FromSeconds(30);
        private readonly RemoteServiceProviderArgs _args;
        private readonly List<Node> _nodes = new List<Node>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="args"></param>
        public StaticChannelFactoryProvider(RemoteServiceProviderArgs args)
        {
            _args = args;

            // add a node for each URL, ensuring that the primary is the first entry in the list
            _nodes.Add(new Node(_args.BaseUrl));
            if (!string.IsNullOrEmpty(_args.FailoverBaseUrl))
            {
                _nodes.Add(new Node(_args.FailoverBaseUrl));
            }
		}

		#region IChannelFactoryProvider

		/// <summary>
    	/// Gets the primary channel factory for the specified service contract.
    	/// </summary>
    	public ChannelFactory GetPrimary(Type serviceContract)
        {
            ChannelFactory factory = GetFirstLiveChannel(serviceContract);
            if (factory != null)
                return factory;

            // if there were no live nodes in the list, 
            // best thing we can do is return the primary Uri
            return GetChannelFactory(serviceContract, new Uri(_args.BaseUrl));
        }

    	/// <summary>
    	/// Attempts to obtain an alternate channel factory for the specified service
    	/// contract, in the event that the primary channel endpoint is unreachable.
    	/// </summary>
    	public ChannelFactory GetFailover(Type serviceContract, EndpointAddress failedEndpoint)
        {
            // find the failed node and marked it as blacked out
            Node failedNode = CollectionUtils.SelectFirst(_nodes,
                delegate(Node n) { return Equals(failedEndpoint.Uri, GetFullUri(serviceContract, n.Url)); });
            failedNode.Blackout(_blackoutPeriod);

            // get the first live node
            ChannelFactory factory = GetFirstLiveChannel(serviceContract);
            if (factory != null)
                return factory;

            return null;
		}

		#endregion

		#region Helpers

		private ChannelFactory GetFirstLiveChannel(Type serviceContract)
        {
            // find the first non-blacked out node in the list
            Node node = CollectionUtils.SelectFirst(_nodes,
                delegate(Node n) { return !n.IsBlackedOut; });

            return node == null ? null : GetChannelFactory(serviceContract, node.Url);
        }

        private ChannelFactory GetChannelFactory(Type serviceContract, Uri baseUri)
        {
            Uri uri = GetFullUri(serviceContract, baseUri);
            Type channelFactoryClass = typeof(ChannelFactory<>).MakeGenericType(new Type[] { serviceContract });
            return _args.Configuration.ConfigureChannelFactory(
                new ServiceChannelConfigurationArgs(channelFactoryClass,
                                                    uri,
                                                    AuthenticationAttribute.IsAuthenticationRequired(serviceContract),
                                                    _args.MaxReceivedMessageSize,
                                                    _args.CertificateValidationMode,
                                                    _args.RevocationMode));
        }

        private static Uri GetFullUri(Type serviceContract, Uri baseUri)
        {
            return new Uri(baseUri, serviceContract.FullName);
		}

		#endregion
	}
}
