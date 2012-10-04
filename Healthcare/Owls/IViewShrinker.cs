#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Defines an interface to a class that knows how to shrink a view by removing items
	/// that do not meet inclusion criteria.
	/// </summary>
	public interface IViewShrinker
	{
		/// <summary>
		/// Deletes items from the view, returning the number of items actually deleted.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="maxItems">Maximum number of items to delete.</param>
		/// <returns>Number of items deleted.</returns>
		int DeleteItems(IUpdateContext context, int maxItems);
	}
}
