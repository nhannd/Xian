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
    class UpgradeFrom_5_0_5655_9770 : BaseUpgradeScript
    {
        public UpgradeFrom_5_0_5655_9770()
            : base(new Version(5, 0, 5655, 9770), null, "UpgradeFrom_5_0_5655_9770.sql")
        {
        }
    }
}
