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


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// VisitLocation entity
    /// </summary>
	public partial class VisitLocation
	{
        private void CustomInitialize()
        {
        }

        public void CopyFrom(VisitLocation vl)
        {
            this.Role = vl.Role;
            this.Location = vl.Location;
            this.StartTime = vl.StartTime;
            this.EndTime = vl.EndTime;
        }
	}
}
