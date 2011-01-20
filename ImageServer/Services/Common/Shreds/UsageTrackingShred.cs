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
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageServer.Services.Common.Misc;

namespace ClearCanvas.ImageServer.Services.Common.Shreds
{
    /// <summary>
    /// Plugin to host Usage Tracking service.
    /// </summary>
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class UsageTrackingShred : Shred
    {
        #region Private Members

        private readonly string _className;
        private UsageTrackingService _service;

        #endregion

        #region Constructors

        public UsageTrackingShred()
        {
            _className = GetType().ToString();
        }

        #endregion

        #region IShred Implementation Shred Override

        public override void Start()
        {
            Platform.Log(LogLevel.Debug, "{0}[{1}]: Start invoked", _className, AppDomain.CurrentDomain.FriendlyName);

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
            Platform.Log(LogLevel.Info, "{0}[{1}]: Stop invoked", _className, AppDomain.CurrentDomain.FriendlyName);
            if (_service != null)
                _service.Stop();
        }

        public override string GetDisplayName()
        {
            return SR.UsageTrackingService;
        }

        public override string GetDescription()
        {
            return SR.UsageTrackingServiceDescription;
        }

        #endregion
    }
}