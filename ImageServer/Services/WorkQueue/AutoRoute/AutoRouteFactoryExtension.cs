#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.AutoRoute
{
    /// <summary>
    /// Plugin for processing 'AutoRoute' WorkQueue items.
    /// </summary>
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    public class AutoRouteFactoryExtension : IWorkQueueProcessorFactory
    {
        #region Constructors
        public AutoRouteFactoryExtension()
        { }
        #endregion

        #region IWorkQueueProcessorFactory Members

        public WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.AutoRoute;
        }

        public IWorkQueueItemProcessor GetItemProcessor()
        {
            AutoRouteItemProcessor processor = new AutoRouteItemProcessor();
            processor.Name = WorkQueueTypeEnum.AutoRoute.ToString();
            return processor;
        }

        #endregion
    }
}
