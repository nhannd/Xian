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

namespace ClearCanvas.ImageServer.Services.WorkQueue.TierMigrate
{
    /// <summary>
    /// Plugin for processing 'WebDeleteStudy' WorkQueue items.
    /// </summary>
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    public class TierMigrateStudyFactoryExtension  : IWorkQueueProcessorFactory
    {
        #region Constructors
        #endregion

        #region IWorkQueueProcessorFactory Members

        public  WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.MigrateStudy;
        }

        public IWorkQueueItemProcessor GetItemProcessor()
        {
            TierMigrateItemProcessor processor = new TierMigrateItemProcessor();
            processor.Name = WorkQueueTypeEnum.MigrateStudy.ToString();
            return processor;
        }

        #endregion
    }
}
