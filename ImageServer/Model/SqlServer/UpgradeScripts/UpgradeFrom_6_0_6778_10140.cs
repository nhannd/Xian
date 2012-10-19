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
    class UpgradeFrom_6_0_6778_10140 : BaseUpgradeScript
    {
        public UpgradeFrom_6_0_6778_10140()
            : base(new Version(6, 0, 6778, 10140), new Version(6, 1, 7081, 10268), "UpgradeFrom_NoOp.sql")
        {
        }
    }
}

