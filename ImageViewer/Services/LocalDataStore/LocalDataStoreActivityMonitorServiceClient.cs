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
