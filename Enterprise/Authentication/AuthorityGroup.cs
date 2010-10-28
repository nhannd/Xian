#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication {


    /// <summary>
    /// AuthorityGroup entity
    /// </summary>
	public partial class AuthorityGroup : ClearCanvas.Enterprise.Core.Entity
	{
		/// <summary>
		/// Removes all users from this authority group.  This method must be used
		/// instead of clearing the <see cref="Users"/> collection.
		/// </summary>
		public virtual void RemoveAllUsers()
		{
			CollectionUtils.ForEach(_users,
				delegate(User u) { u.AuthorityGroups.Remove(this); });
		}
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}


	}
}