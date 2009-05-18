using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;
using System.Threading;

namespace ClearCanvas.Enterprise.Common
{
    public enum ResponseCachingSite
    {
        None,
        Server,
        Client,
    }

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

        public ResponseCachingDirective()
            : this(false, TimeSpan.Zero, ResponseCachingSite.None)
        {
        }

        public ResponseCachingDirective(bool enableCaching, TimeSpan timeToLive, ResponseCachingSite site)
        {
            _enableCaching = enableCaching;
            _timeToLive = timeToLive;
            _cacheSite = site;
        }

        [DataMember]
        public bool EnableCaching
        {
            get { return _enableCaching; }
            set { _enableCaching = value; }
        }

        [DataMember]
        public TimeSpan TimeToLive
        {
            get { return _timeToLive; }
            set { _timeToLive = value; }
        }

        [DataMember]
        public ResponseCachingSite CacheSite
        {
            get { return _cacheSite; }
            set { _cacheSite = value; }
        }

        public override string ToString()
        {
            return string.Format("EnableCaching = {1} TTL = {2} Site = {3}",
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
