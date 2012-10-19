#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using System.Threading;
using ClearCanvas.Common.UsageTracking;
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.Ris.Shreds.UsageTracking
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    class UsageTrackingShred: Shred
    {
        private readonly string _className;
        private UsageTrackingService _service;

        #region Constructors

        public UsageTrackingShred()
        {
            _className = GetType().ToString();
        }

        #endregion

        public override void Start()
        {
            Platform.Log(LogLevel.Info, string.Format(ClearCanvas.Common.SR.ShredStarting, this.GetDisplayName()));

            try
            {
                _service = new UsageTrackingService();
                _service.Start();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Fatal, e, "Unexpected exception starting {0}", GetDisplayName());
            }
        }

        public override void Stop()
        {
            Platform.Log(LogLevel.Info, string.Format(ClearCanvas.Common.SR.ShredStopping, this.GetDisplayName()));
            if (_service != null)
                _service.Stop();
        }

        public override string GetDisplayName()
        {
            return SR.UsageTrackingShred;
        }

        public override string GetDescription()
        {
            return SR.UsageTrackingShred;
        }
    }

   
}
