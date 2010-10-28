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
using Iesi.Collections.Generic;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Facility entity
    /// </summary>
	public partial class Facility : Entity
	{
		public Facility(string code, string name, string description, InformationAuthorityEnum informationAuthority)
			:this(code, name, description, informationAuthority, new HashedSet<Department>())
    	{
    		
    	}

        private void CustomInitialize()
        {
        }

        public override string ToString()
        {
            return Name;
        }
    }
}