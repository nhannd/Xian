#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy
{
    /// <summary>
    /// Plugin for processing 'WebDeleteStudy' WorkQueue items.
    /// </summary>
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    public class WebDeleteStudyFactoryExtension : DeleteStudyFactoryExtension
    {
        #region Constructors
        public WebDeleteStudyFactoryExtension()
        { }
        #endregion

        #region IWorkQueueProcessorFactory Members

        public override WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.WebDeleteStudy;
        }

        public override IWorkQueueItemProcessor GetItemProcessor()
        {
            WebDeleteStudyItemProcessor processor = new WebDeleteStudyItemProcessor();
            processor.Name = WorkQueueTypeEnum.WebDeleteStudy.ToString();
            return processor;
        }

        #endregion
    }
}
