using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Caching
{
	/// <summary>
	/// Encapsulates options for the <see cref="ICacheClient.Get"/> method.
	/// </summary>
    public class CacheGetOptions : CacheOptionsBase
	{
		public CacheGetOptions(string region)
			: base(region)
		{
        }
    }
}
