#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageServer.Services.ServiceLock
{
    /// <summary>
    /// Interface for processors of <see cref="Model.ServiceLock"/> rows.
    /// </summary>
    public interface IServiceLockItemProcessor : IDisposable
    {
        void Process(Model.ServiceLock item);
    }
}
