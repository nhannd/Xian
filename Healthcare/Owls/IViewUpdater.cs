#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Defines an interface to a class that is responsible for synchronizing a view
	/// in response to an entity change-set.
	/// </summary>
	public interface IViewUpdater
	{
		/// <summary>
		/// Gets a value indicating whether the view requires an update in response
		/// to the specified change-set.
		/// </summary>
		/// <param name="changeSet"></param>
		/// <returns></returns>
		bool IsUpdateRequired(EntityChangeSet changeSet);

		/// <summary>
		/// Updates the view in response to the specified change-set.
		/// </summary>
		/// <param name="changeSet"></param>
		/// <param name="context"></param>
		void Update(EntityChangeSet changeSet, IPersistenceContext context);
	}
}
