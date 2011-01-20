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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class PartitionArchiveAdaptor : BaseAdaptor<PartitionArchive, IPartitionArchiveEntityBroker, PartitionArchiveSelectCriteria, PartitionArchiveUpdateColumns>
	{
		#region Private Members

		private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();

		#endregion Private Members

		public bool RestoreStudy(Study theStudy)
		{
		    RestoreQueue restore = ServerHelper.InsertRestoreRequest(theStudy.LoadStudyStorage(HttpContextData.Current.ReadContext));
            if (restore==null)
                throw new ApplicationException("Unable to restore the study. See the log file for details.");

			return true;
		}
	}
}
