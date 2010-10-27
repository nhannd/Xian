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

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Staff entity
    /// </summary>
	public partial class Staff : Entity
    {
		/// <summary>
		/// Gets only the active <see cref="StaffGroup"/>s to which this staff belongs.
		/// </summary>
		public virtual IList<StaffGroup> ActiveGroups
		{
			get { return CollectionUtils.Select(_groups, delegate(StaffGroup g) { return !g.Deactivated; }); }
		}

		private void CustomInitialize()
        {
        }
	}
}