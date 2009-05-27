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
