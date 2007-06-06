using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.LocalDataStore
{
	public interface ILocalDataStoreActivityMonitorServiceCallback
	{
		[OperationContract(IsOneWay = true)]
		void ReceiveProgressChanged(ReceiveProgressItem progressItem);

		[OperationContract(IsOneWay = true)]
		void SendProgressChanged(SendProgressItem progressItem);

		[OperationContract(IsOneWay = true)]
		void ImportProgressChanged(ImportProgressItem progressItem);

		[OperationContract(IsOneWay = true)]
		void ReindexProgressChanged(ReindexProgressItem progressItem);

		[OperationContract(IsOneWay = true)]
		void SopInstanceImported(ImportedSopInstanceInformation information);

		[OperationContract(IsOneWay = true)]
		void InstanceDeleted(DeletedInstanceInformation information);
	}

	[ServiceContract(	SessionMode = SessionMode.Required,
						CallbackContract = typeof(ILocalDataStoreActivityMonitorServiceCallback), 
						ConfigurationName="ILocalDataStoreActivityMonitorService")]
	public interface ILocalDataStoreActivityMonitorService
	{
		[OperationContract(IsOneWay = true)]
		void Cancel(CancelProgressItemInformation information);

		[OperationContract(IsOneWay = true)]
		void ClearInactive();

		[OperationContract(IsOneWay = true)]
		void Refresh();

		[OperationContract]
		void Subscribe(string eventName);

		[OperationContract]
		void Unsubscribe(string eventName);
	}
}
