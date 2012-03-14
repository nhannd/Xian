#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.LocalDataStore
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
