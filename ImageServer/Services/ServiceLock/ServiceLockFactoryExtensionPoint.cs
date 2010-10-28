#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Services.ServiceLock
{
    /// <summary>
    /// Plugin for <see cref="Model.ServiceLock"/> item processors.
    /// </summary>
    [ExtensionPoint()]
    public class ServiceLockFactoryExtensionPoint : ExtensionPoint<IServiceLockProcessorFactory>
    {
    }
}
