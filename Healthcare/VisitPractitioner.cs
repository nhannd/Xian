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
    /// VisitPractitioner component
    /// </summary>
	public partial class VisitPractitioner
	{
        private void CustomInitialize()
        {
        }

        public void CopyFrom(VisitPractitioner vp)
        {
            this.Role = vp.Role;
            this.Practitioner = vp.Practitioner;
            this.StartTime = vp.StartTime;
            this.EndTime = vp.EndTime;
        }
	}
}
