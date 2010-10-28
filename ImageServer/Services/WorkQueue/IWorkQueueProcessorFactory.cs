#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Interface for factory to create instances of <see cref="IWorkQueueItemProcessor"/> for processing WorkQueue items.
    /// </summary>
    public interface IWorkQueueProcessorFactory
    {
        WorkQueueTypeEnum GetWorkQueueType();

        IWorkQueueItemProcessor GetItemProcessor();
    }
}