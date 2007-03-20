using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.LocalDataStore
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

	[ServiceContract(CallbackContract = typeof(ILocalDataStoreActivityMonitorServiceCallback), ConfigurationName="ILocalDataStoreActivityMonitorService")]
	public interface ILocalDataStoreActivityMonitorService
	{
		[OperationContract(IsOneWay = true)]
		void Cancel(CancelProgressItemInformation information);
		
		[OperationContract(IsOneWay = true)]
		void ClearInactive();
		
		[OperationContract(IsOneWay = true)]
		void Refresh();

		[OperationContract(IsOneWay = true)]
		void Subscribe(string eventName);

		[OperationContract(IsOneWay = true)]
		void Unsubscribe(string eventName);
	}
}
