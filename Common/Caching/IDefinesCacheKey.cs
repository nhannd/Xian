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
	/// Defines an interface to allow a class to define its own custom cache key string.
	/// </summary>
	public interface IDefinesCacheKey
	{
		/// <summary>
		/// Gets the cache key defined by this instance.
		/// </summary>
		/// <returns></returns>
		string GetCacheKey();
	}
}
