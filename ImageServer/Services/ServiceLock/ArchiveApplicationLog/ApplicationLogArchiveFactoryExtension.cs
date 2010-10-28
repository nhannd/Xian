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

namespace ClearCanvas.ImageServer.Services.ServiceLock.ArchiveApplicationLog
{
	[ExtensionOf(typeof(ServiceLockFactoryExtensionPoint))]
	public class ApplicationLogArchiveFactoryExtension : IServiceLockProcessorFactory
	{
		public ServiceLockTypeEnum GetServiceLockType()
		{
			return ServiceLockTypeEnum.ArchiveApplicationLog;
		}

		public IServiceLockItemProcessor GetItemProcessor()
		{
			return new ApplicationLogArchiveItemProcessor();
		}
	}
}