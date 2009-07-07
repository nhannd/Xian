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
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;
using System.Threading;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Defines the set of response cache sites.
	/// </summary>
    public enum ResponseCachingSite
    {
		/// <summary>
		/// The response is not cached.
		/// </summary>
        None,

		/// <summary>
		/// The response is cached on the server.
		/// </summary>
        Server,

		/// <summary>
		/// The response is cached on the client.
		/// </summary>
        Client,
    }

	/// <summary>
	/// Encapsulates information that directs how a client should cache a response.
	/// </summary>
    [DataContract]
    public class ResponseCachingDirective : DataContractBase, IEquatable<ResponseCachingDirective>
    {
        public const string HeaderName = "ResponseCachingDirective";
        public const string HeaderNamespace = "urn:http://www.clearcanvas.ca";

        /// <summary>
        /// Defines a static Do Not Cache directive.
        /// </summary>
        public static ResponseCachingDirective DoNotCacheDirective
            = new ResponseCachingDirective();

        private bool _enableCaching;
        private TimeSpan _timeToLive;
        private ResponseCachingSite _cacheSite;

		/// <summary>
		/// Constructor.
		/// </summary>
        public ResponseCachingDirective()
            : this(false, TimeSpan.Zero, ResponseCachingSite.None)
        {
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="enableCaching"></param>
		/// <param name="timeToLive"></param>
		/// <param name="site"></param>
        public ResponseCachingDirective(bool enableCaching, TimeSpan timeToLive, ResponseCachingSite site)
        {
            _enableCaching = enableCaching;
            _timeToLive = timeToLive;
            _cacheSite = site;
        }

		/// <summary>
		/// Gets or sets a value indicated whether caching of the response is enabled.
		/// </summary>
        [DataMember]
        public bool EnableCaching
        {
            get { return _enableCaching; }
            set { _enableCaching = value; }
        }

		/// <summary>
		/// Gets or sets a value indicating the Time-to-Live for the cached response.
		/// </summary>
        [DataMember]
        public TimeSpan TimeToLive
        {
            get { return _timeToLive; }
            set { _timeToLive = value; }
        }

		/// <summary>
		/// Gets or sets the cache site.
		/// </summary>
        [DataMember]
        public ResponseCachingSite CacheSite
        {
            get { return _cacheSite; }
            set { _cacheSite = value; }
        }

        public override string ToString()
        {
            return string.Format("EnableCaching = {0} TTL = {1} Site = {2}",
                _enableCaching, _timeToLive, _cacheSite);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ResponseCachingDirective);
        }

        public override int GetHashCode()
        {
            return _enableCaching.GetHashCode() ^ _cacheSite.GetHashCode() ^ _timeToLive.GetHashCode();
        }

        #region IEquatable<ResponseCachingDirective> Members

        public bool Equals(ResponseCachingDirective other)
        {
            if (other == null)
                return false;
            return _enableCaching == other._enableCaching
                && _cacheSite == other._cacheSite
                && _timeToLive == other._timeToLive;
        }

        #endregion
    }
}
