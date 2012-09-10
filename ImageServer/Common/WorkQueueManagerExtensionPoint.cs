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
using System.Linq;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common
{
    public interface IWorkQueueManagerExtensionPoint
    {
        /// <summary>
        /// Called when work queue processor is being initialized
        /// </summary>
        /// <exception cref="WorkQueueInitializationException"></exception>
        void OnInitializing(ThreadedService service);
    }

    [ExtensionPoint]
    public class WorkQueueManagerExtensionPoint : ExtensionPoint<IWorkQueueManagerExtensionPoint>
    {
    }
}
