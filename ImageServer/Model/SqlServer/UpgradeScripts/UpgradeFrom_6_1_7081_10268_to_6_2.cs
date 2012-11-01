#region License

// Copyright (c) 2012, ClearCanvas Inc.
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
    class UpgradeFrom_6_1_7081_10268_to_6_2 : BaseUpgradeScript
    {
        public UpgradeFrom_6_1_7081_10268()
            : base(new Version(6, 1, 7081, 10268), null, "UpgradeFrom_6_1_7081_10268_to_6_2.sql")
        {
        }
    }
}
