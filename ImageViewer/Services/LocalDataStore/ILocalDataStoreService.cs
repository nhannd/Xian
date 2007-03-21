using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.LocalDataStore
{
	// For reasons of simplification, the local datastore services do not adhere strictly to 
	// the publish-subscribe pattern where there are separate services for publish-only and
	// subscription/callback functionality.

	[ServiceContract(ConfigurationName = "ILocalDataStoreService")]
	public interface ILocalDataStoreService
	{
		// Dicom Server event publishing
		[OperationContract]
		void FileReceived(StoreScpReceivedFileInformation receivedFileInformation);
		
		[OperationContract]
		void FileSent(StoreScuSentFileInformation sentFileInformation);

		// Local DataStore requests.
		[OperationContract]
		void Import(FileImportRequest request);

		[OperationContract]
		void Reindex();
	}
}
