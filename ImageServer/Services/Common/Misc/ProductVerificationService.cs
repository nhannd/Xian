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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.ServiceModel;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.Utilities.Manifest;

namespace ClearCanvas.ImageServer.Services.Common.Misc
{
    [ServiceImplementsContract(typeof(IProductVerificationService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint),Enabled=true)]
    public class ProductVerificationService : IApplicationServiceLayer, IProductVerificationService
    {
        private readonly TimeSpan ManifestRecheckTimeSpan = TimeSpan.FromMinutes(15);
        private DateTime? _lastCheckTimestamp;

        public ProductVerificationResponse Verify(ProductVerificationRequest request)
        {
            if (_lastCheckTimestamp == null || (DateTime.Now - _lastCheckTimestamp.Value) > ManifestRecheckTimeSpan)
            {
                ManifestVerification.Invalidate();
            }

            _lastCheckTimestamp = DateTime.Now;

            return new ProductVerificationResponse
                       {
                            IsManifestValid = ManifestVerification.Valid,
                            ComponentName = ProductInformation.Component,
                            Edition = ProductInformation.Edition
                       };
        }
    }
}