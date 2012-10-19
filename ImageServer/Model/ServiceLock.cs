#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model.EntityBrokers;
namespace ClearCanvas.ImageServer.Model
{
	public partial class ServiceLock
	{
		#region Private Members
		private Filesystem _filesystem;
		#endregion

		#region Public Properties
		public Filesystem Filesystem
		{
			get
			{
				if (FilesystemKey == null)
					return null;
				if (_filesystem == null)
					_filesystem = Filesystem.Load(FilesystemKey);
				return _filesystem;
			}
		}
		#endregion

        /// <summary>
        /// Finds all <see cref="ServiceLock"/> of the specified <see cref="ServiceLockTypeEnum"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<ServiceLock> FindServicesOfType(ServiceLockTypeEnum type)
        {
            using (var readCtx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                var broker = readCtx.GetBroker<IServiceLockEntityBroker>();
                var criteria = new ServiceLockSelectCriteria();
                criteria.ServiceLockTypeEnum.EqualTo(type);
                return broker.Find(criteria);
            }
        }
	}
}
