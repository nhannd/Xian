#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core.Upgrade;

namespace ClearCanvas.ImageServer.Model.SqlServer.UpgradeScripts
{
    /// <summary>
    /// Kirk Release upgrade script
    /// </summary>
    [ExtensionOf(typeof(PersistentStoreUpgradeScriptExtensionPoint))]
    public class UpgradeFrom_2_2_14734_44343 : BaseUpgradeScript
    {
        public UpgradeFrom_2_2_14734_44343()
            : base(new Version(2, 2, 14734, 44343), new Version(3, 5, 16130, 48592), "UpgradeFrom_2_2_14734_44343.sql")
        {
        }
    }
}
