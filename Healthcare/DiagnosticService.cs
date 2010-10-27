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
using ClearCanvas.Common.Utilities;
using Iesi.Collections.Generic;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// DiagnosticService entity
    /// </summary>
	public partial class DiagnosticService : Entity
	{

        public DiagnosticService(string id, string name)
            :this(id, name, new HashedSet<ProcedureType>())
        {
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