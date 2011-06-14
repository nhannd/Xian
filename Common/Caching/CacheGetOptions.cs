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
