#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Shreds.Merge
{
	/// <summary>
	/// Defines an interface for handling an asynchronous merge operation in stages.
	/// </summary>
	public interface IMergeHandler
	{
		/// <summary>
		/// Gets a value indicating whether this handler supports merging of the specified target.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		bool SupportsTarget(Entity target);

		/// <summary>
		/// Asks this handler to perform part of the merge operation, beginning at the specified stage.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="stage"></param>
		/// <param name="context"></param>
		/// <returns>The stage at which the merge operation should continue next.</returns>
		int Merge(Entity target, int stage, IPersistenceContext context);
	}
}
