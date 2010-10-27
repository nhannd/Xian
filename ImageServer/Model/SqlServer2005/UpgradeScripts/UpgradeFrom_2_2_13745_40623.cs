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
    class UpgradeFrom_2_2_13745_40623 : BaseUpgradeScript
    {
        public UpgradeFrom_2_2_13745_40623()
           : base(new Version(2, 2, 13745, 40623), null, "UpgradeFrom_NoOp.sql")
        {
        }
    }
}
