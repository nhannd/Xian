#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core.Upgrade;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.UpgradeScripts
{
	[ExtensionOf(typeof(PersistentStoreUpgradeScriptExtensionPoint))]
	class UpgradeFrom_1_5_10676_32680 : BaseUpgradeScript
	{
		//In versions prior to 1.5 the use of Build and Revision were swapped and so it has to be swapped here in order for the utility to properly detect the older version
		public UpgradeFrom_1_5_10676_32680()
			: base(new Version(1, 5, 10676, 32680), new Version(1, 5, 10852, 32944), "UpgradeFrom_NoOp.sql")
		{
		}
	}
}

