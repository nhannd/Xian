#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudyPostProcessing
{
	[Obsolete("This class is obsolete as of the 1.6 release.")]
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    class ReconcilePostProcessingProcessorFactoryExtension : IWorkQueueProcessorFactory
    {
        public WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.ReconcilePostProcess;
        }

        public IWorkQueueItemProcessor GetItemProcessor()
        {
            ReconcilePostProcessingProcessor processor = new ReconcilePostProcessingProcessor();
            processor.Name = WorkQueueTypeEnum.ReconcilePostProcess.ToString();
            return processor;
        }
    }
}
