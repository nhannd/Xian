using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	public interface ILocalDataStoreActivityMonitorServiceCallback
	{
		void ReceiveProgressChanged(ReceiveProgressItem progressItem);

		void SendProgressChanged(SendProgressItem progressItem);

		void ImportProgressChanged(ImportProgressItem progressItem);

		void ReindexProgressChanged(ReindexProgressItem progressItem);

		void SopInstancesImported(ImportedSopInstanceInformation information);

		void ServiceStopped();
	}

	[ServiceContract(CallbackContract = typeof(ILocalDataStoreActivityMonitorServiceCallback))]
	public interface ILocalDataStoreActivityMonitorService : ISubscriptionService
	{
		[OperationContract(IsOneWay = true)]
		void Cancel(CancelProgressItemInformation information);
		
		[OperationContract(IsOneWay = true)]
		void ClearInactive();
		
		[OperationContract(IsOneWay = true)]
		void Refresh();
	}
}
