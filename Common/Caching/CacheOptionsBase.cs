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
	public abstract class CacheOptionsBase
	{
		protected CacheOptionsBase(string region)
		{
			Platform.CheckForNullReference(region, "region");
			Region = region;
		}

		/// <summary>
		/// Gets or sets the region
		/// </summary>
		public string Region { get; set; }
	}
}