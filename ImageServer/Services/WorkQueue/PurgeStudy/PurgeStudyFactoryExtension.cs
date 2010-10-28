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

namespace ClearCanvas.ImageServer.Services.WorkQueue.PurgeStudy
{

	/// <summary>
	/// Plugin for processing 'DeleteStudy' WorkQueue items.
	/// </summary>
	[ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
	public class PurgeStudyFactoryExtension : IWorkQueueProcessorFactory
	{
		#region Constructors
		public PurgeStudyFactoryExtension()
		{ }
		#endregion

		#region IWorkQueueProcessorFactory Members

		public virtual WorkQueueTypeEnum GetWorkQueueType()
		{
			return WorkQueueTypeEnum.PurgeStudy;
		}

		public virtual IWorkQueueItemProcessor GetItemProcessor()
		{
			PurgeStudyItemProcessor processor =  new PurgeStudyItemProcessor();
            processor.Name = WorkQueueTypeEnum.PurgeStudy.ToString();
            return processor;
		}
		#endregion
	}
		
}
