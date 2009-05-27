using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Caching
{
	/// <summary>
	/// Encapsulates options for the <see cref="ICacheClient.Put"/> method.
	/// </summary>
	public class CachePutOptions : CacheOptionsBase
    {
        private TimeSpan _expiration;
        private bool _sliding;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="region"></param>
		/// <param name="expiration"></param>
		/// <param name="sliding"></param>
        public CachePutOptions(string region, TimeSpan expiration, bool sliding)
			:base(region)
        {
            _expiration = expiration;
            _sliding = sliding;
        }

		/// <summary>
		/// Gets or sets the expiration time.
		/// </summary>
        public TimeSpan Expiration
        {
            get { return _expiration; }
            set { _expiration = value; }
        }

		/// <summary>
		/// Gets or sets a value indicating whether the expiration is sliding or not.
		/// </summary>
        public bool Sliding
        {
            get { return _sliding; }
            set { _sliding = value; }
        }
    }
}
