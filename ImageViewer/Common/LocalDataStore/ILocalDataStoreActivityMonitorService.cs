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

		[OperationContract(IsOneWay = true)]
		void LocalDataStoreCleared();
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
