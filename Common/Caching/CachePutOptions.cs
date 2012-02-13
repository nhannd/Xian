#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Caching
{
	/// <summary>
	/// Encapsulates options for the <see cref="ICacheClient.Put"/> method.
	/// </summary>
	public class CachePutOptions : CacheOptionsBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="region"></param>
		/// <param name="expiration"></param>
		/// <param name="sliding"></param>
		public CachePutOptions(string region, TimeSpan expiration, bool sliding)
			: base(region)
		{
			Expiration = expiration;
			Sliding = sliding;
		}

		/// <summary>
		/// Gets or sets the expiration time.
		/// </summary>
		public TimeSpan Expiration { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the expiration is sliding or not.
		/// </summary>
		public bool Sliding { get; set; }
	}
}
