using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common
{
    public enum ResponseCachingSite
    {
        Server,
        Client,
    }

    [DataContract]
    public class ResponseCachingDirective : DataContractBase
    {
        public const string HeaderName = "ResponseCachingDirective";
        public const string HeaderNamespace = "urn:http://www.clearcanvas.ca";


        private bool _enableCaching;
        private TimeSpan _timeToLive;
        private ResponseCachingSite _cacheSite;
        private bool _userAffine;

        public ResponseCachingDirective()
        {

        }

        public ResponseCachingDirective(bool enableCaching, TimeSpan timeToLive, ResponseCachingSite site, bool userAffine)
        {
            _enableCaching = enableCaching;
            _timeToLive = timeToLive;
            _cacheSite = site;
            _userAffine = userAffine;
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

        [DataMember]
        public bool UserAffine
        {
            get { return _userAffine; }
            set { _userAffine = value; }
        }
    }
}
