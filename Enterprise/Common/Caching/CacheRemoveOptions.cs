using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Caching
{
	/// <summary>
	/// Encapsulates options for the <see cref="ICacheClient.Remove"/> method.
	/// </summary>
	public class CacheRemoveOptions : CacheOptionsBase
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="region"></param>
        public CacheRemoveOptions(string region)
			:base(region)
        {
        }
    }
}
