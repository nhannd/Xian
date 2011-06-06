#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common.Caching
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
			: base(region)
		{
		}
	}
}
