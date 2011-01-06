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
using ClearCanvas.ImageServer.Services.WorkQueue.CompressStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.CompressStudy
{
	/// <summary>
	/// Plugin for processing 'CompressStudy' WorkQueue items.
	/// </summary>
	[ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
	public class CompressStudyFactoryExtension : IWorkQueueProcessorFactory
	{
		#region IWorkQueueProcessorFactory Members

		public WorkQueueTypeEnum GetWorkQueueType()
		{
			return WorkQueueTypeEnum.CompressStudy;
		}

		public IWorkQueueItemProcessor GetItemProcessor()
		{
			CompressStudyItemProcessor processor =  new CompressStudyItemProcessor();
            processor.Name = WorkQueueTypeEnum.CompressStudy.ToString();
            return processor;
		}

		#endregion
	}
}