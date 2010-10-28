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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using System.Collections.Generic;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// EmailAddress component
    /// </summary>
	public partial class EmailAddress
	{
        /// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		public bool IsCurrent
		{
			get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }
		}

        /// <summary>
        /// Returns true if the objects represent the same email address, regardless of validity range.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public bool IsSameEmailAddress(EmailAddress that)
        {
            return that != null && this._address == that._address;
        }
	}
}