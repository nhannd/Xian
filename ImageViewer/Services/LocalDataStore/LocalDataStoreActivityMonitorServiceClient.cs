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
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.ImageViewer.Services.LocalDataStore
{
	public class LocalDataStoreActivityMonitorServiceClient : DuplexClientBase<ILocalDataStoreActivityMonitorService>, ILocalDataStoreActivityMonitorService
	{
		public LocalDataStoreActivityMonitorServiceClient(InstanceContext callbackInstance)
			: base(callbackInstance)
		{ 
		}

		public void Subscribe(string eventOperation)
		{
			base.Channel.Subscribe(eventOperation);
		}

		public void Unsubscribe(string eventOperation)
		{
			base.Channel.Unsubscribe(eventOperation);
		}

		public void Cancel(CancelProgressItemInformation information)
		{
			base.Channel.Cancel(information);
		}

		public void ClearInactive()
		{
			base.Channel.ClearInactive();
		}

		public void Refresh()
		{
			base.Channel.Refresh();
		}
	}
}
