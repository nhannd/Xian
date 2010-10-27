#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Server.ShredHost
{
    internal class ShredStartupInfoList : MarshallableList<ShredStartupInfo>
    {
        public ShredStartupInfoList()
        {

        }

        public ReadOnlyCollection<ShredStartupInfo> AllShredStartupInfo
        {
            get { return this.ContainedObjects; }
        }
    }
}
