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
    [ExtensionOf(typeof(PersistentStoreUpgradeScriptExtensionPoint))]
    class UpgradeFrom_3_5_16130_48592 : BaseUpgradeScript
    {
        public UpgradeFrom_3_5_16130_48592()
            : base(new Version(3, 5, 16130, 48592), new Version(4, 0, 5014, 9549), "UpgradeFrom_NoOp.sql")
        {
        }
    }
}