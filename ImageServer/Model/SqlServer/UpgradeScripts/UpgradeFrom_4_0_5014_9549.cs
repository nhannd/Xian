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
    class UpgradeFrom__4_0_5014_9549 : BaseUpgradeScript
    {
        public UpgradeFrom__4_0_5014_9549()
            : base(new Version(4, 0, 5014, 9549), null, "UpgradeFrom_4_0_5014_9549.sql")
        {
        }
    }
}
