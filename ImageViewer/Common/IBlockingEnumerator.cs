#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Common
{
	/// <remarks>
	/// DO NOT USE THIS CLASS, as it will be removed or refactored in the future.
	/// </remarks>
	public interface IBlockingEnumerator<T> : IEnumerable<T>
	{
		bool IsBlocking { get; set; }
	}
}
