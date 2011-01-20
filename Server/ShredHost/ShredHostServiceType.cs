#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ClearCanvas.Server.ShredHost
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class ShredHostServiceType : IShredHost
    {
        #region IShredHost Members
        public WcfDataShred[] GetShreds()
        {
            return ShredHost.ShredControllerList.WcfDataShredCollection;
        }

        public bool StartShred(WcfDataShred shred)
        {
            return ShredHost.StartShred(shred);
        }

        public bool StopShred(WcfDataShred shred)
        {
            return ShredHost.StopShred(shred);
        }

        #endregion
    }
}
