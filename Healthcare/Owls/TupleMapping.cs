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
	/// References a method that updates a view item based on a given data value.
	/// </summary>
	/// <typeparam name="TViewItem">View item class.</typeparam>
	/// <param name="item">The view item to update.</param>
	/// <param name="value">The data value.</param>
	/// <param name="updateReferences">Indicates whether references should be followed when updating the view item.</param>
	public delegate void UpdateViewItemDelegate<TViewItem>(TViewItem item, object value, bool updateReferences);
	
	/// <summary>
	/// References a method that populates a specified view item from a given data tuple.
	/// </summary>
	/// <param name="viewItem">The view item to populate.</param>
	/// <param name="tuple">The data tuple.</param>
	/// <param name="persistenceContext">Current persistence context.</param>
	public delegate void TupleMappingDelegate(object viewItem, object[] tuple, IPersistenceContext persistenceContext);
}
