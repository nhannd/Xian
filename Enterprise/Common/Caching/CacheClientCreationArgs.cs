using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Caching
{
	public class CacheClientCreationArgs
	{
        private string _cacheID;
		private string _region;
		private TimeSpan _expirationTime;
        private bool _slidingExpiration;

        /// <summary>
        /// Constructor
        /// </summary>
        public CacheClientCreationArgs()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cacheID"></param>
        /// <param name="region"></param>
        public CacheClientCreationArgs(string cacheID, string region)
            :this(cacheID, region, TimeSpan.Zero, false)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cacheID"></param>
        /// <param name="region"></param>
        /// <param name="expirationTime"></param>
        /// <param name="slidingExpiration"></param>
		public CacheClientCreationArgs(string cacheID, string region, TimeSpan expirationTime, bool slidingExpiration)
		{
            _cacheID = cacheID;
			_region = region;
			_expirationTime = expirationTime;
            _slidingExpiration = slidingExpiration;
		}

        /// <summary>
        /// Gets or sets the logical ID of the cache instance.
        /// </summary>
        public string CacheID
        {
            get { return _cacheID; }
            set { _cacheID = value; }
        }

        /// <summary>
        /// Gets or sets the cache region within the logical cache.
        /// </summary>
		public string Region
		{
			get { return _region; }
			set { _region = value; }
		}

        /// <summary>
        /// Gets or sets the expiration time.
        /// </summary>
		public TimeSpan ExpirationTime
		{
			get { return _expirationTime; }
			set { _expirationTime = value; }
		}

        /// <summary>
        /// Gets or sets a value indicating whether the expiration time is sliding,
        /// that is, whether it is update each time the cached item is accessed.
        /// </summary>
        public bool SlidingExpiration
        {
            get { return _slidingExpiration; }
            set { _slidingExpiration = value; }
        }
	}
}
