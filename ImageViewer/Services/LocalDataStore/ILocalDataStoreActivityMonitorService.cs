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
		void OnServiceStopped();
	}

	[ServiceContract(	//SessionMode = SessionMode.Required,
						CallbackContract = typeof(ILocalDataStoreActivityMonitorServiceCallback), 
						ConfigurationName="ILocalDataStoreActivityMonitorService")]
	public interface ILocalDataStoreActivityMonitorService
	{
		[OperationContract]
		void Cancel(CancelProgressItemInformation information);
		
		[OperationContract]
		void ClearInactive();
		
		[OperationContract]
		void Refresh();

		//[OperationContract(IsInitiating = true)]
		[OperationContract]
		void Subscribe(string eventName);

		//[OperationContract(IsTerminating = true)]
		[OperationContract]
		void Unsubscribe(string eventName);
	}
}
